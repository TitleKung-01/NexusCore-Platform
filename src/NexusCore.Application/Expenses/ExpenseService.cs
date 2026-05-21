using NexusCore.Application.Common;
using NexusCore.Application.Notifications;
using NexusCore.Domain.Constants;
using NexusCore.Domain.Entities;
using NexusCore.Domain.Enums;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Application.Expenses;

public class ExpenseService(
    ICurrentUserService currentUser,
    IExpenseRepository expenses,
    IEmployeeProfileRepository profiles,
    INotificationService notifications) : IExpenseService
{
    public async Task<IReadOnlyList<ExpenseClaimResponse>> ListAsync(string scope, CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is null)
            return [];

        IReadOnlyList<ExpenseClaim> list;
        if (string.Equals(scope, "pending-approval", StringComparison.OrdinalIgnoreCase))
        {
            if (currentUser.IsInAnyRole(UserRoles.Hr, UserRoles.Admin))
                list = await expenses.GetAllPendingAsync(cancellationToken);
            else if (currentUser.IsInRole(UserRoles.Manager))
                list = await expenses.GetPendingForManagerAsync(currentUser.UserId.Value, cancellationToken);
            else
                return [];
        }
        else
            list = await expenses.GetByEmployeeIdAsync(currentUser.UserId.Value, cancellationToken);

        return list.Select(Map).ToList();
    }

    public async Task<ExpenseClaimResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var claim = await expenses.FindByIdAsync(id, cancellationToken);
        if (claim is null || !CanView(claim))
            return null;
        return Map(claim);
    }

    public async Task<ServiceResult<ExpenseClaimResponse>> CreateAsync(CreateExpenseClaimRequest request, CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is null)
            return ServiceResult<ExpenseClaimResponse>.Fail("Unauthorized.", 401);

        if (request.LineItems.Count == 0)
            return ServiceResult<ExpenseClaimResponse>.Fail("At least one line item is required.", 400);

        var lineItems = request.LineItems.Select(l => new ExpenseLineItem
        {
            Id = Guid.NewGuid(),
            Description = l.Description.Trim(),
            Amount = l.Amount
        }).ToList();

        var entity = new ExpenseClaim
        {
            Id = Guid.NewGuid(),
            EmployeeId = currentUser.UserId.Value,
            Title = request.Title.Trim(),
            TotalAmount = lineItems.Sum(l => l.Amount),
            Status = LeaveStatus.Draft,
            CreatedAtUtc = DateTime.UtcNow,
            LineItems = lineItems
        };

        foreach (var item in lineItems)
            item.ExpenseClaimId = entity.Id;

        await expenses.AddAsync(entity, cancellationToken);
        await expenses.SaveChangesAsync(cancellationToken);
        var created = await expenses.FindByIdAsync(entity.Id, cancellationToken);
        return ServiceResult<ExpenseClaimResponse>.Ok(Map(created!));
    }

    public async Task<ServiceResult<ExpenseClaimResponse>> SubmitAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var claim = await expenses.FindByIdTrackedAsync(id, cancellationToken);
        if (claim is null)
            return ServiceResult<ExpenseClaimResponse>.Fail("Expense claim not found.", 404);
        if (!IsOwner(claim))
            return ServiceResult<ExpenseClaimResponse>.Fail("Forbidden.", 403);
        if (claim.Status != LeaveStatus.Draft)
            return ServiceResult<ExpenseClaimResponse>.Fail("Only draft claims can be submitted.", 400);

        claim.Status = LeaveStatus.Pending;
        claim.SubmittedAtUtc = DateTime.UtcNow;
        await expenses.SaveChangesAsync(cancellationToken);

        await NotifyManagerAsync(claim, "expense.submitted", "Expense claim submitted",
            $"{claim.Employee.FullName} submitted expense: {claim.Title}.", cancellationToken);

        var updated = await expenses.FindByIdAsync(id, cancellationToken);
        return ServiceResult<ExpenseClaimResponse>.Ok(Map(updated!));
    }

    public async Task<ServiceResult<ExpenseClaimResponse>> ApproveAsync(Guid id, DecideExpenseRequest request, CancellationToken cancellationToken = default)
    {
        var claim = await expenses.FindByIdTrackedAsync(id, cancellationToken);
        if (claim is null)
            return ServiceResult<ExpenseClaimResponse>.Fail("Expense claim not found.", 404);
        if (!CanApprove(claim))
            return ServiceResult<ExpenseClaimResponse>.Fail("Forbidden.", 403);
        if (claim.Status != LeaveStatus.Pending)
            return ServiceResult<ExpenseClaimResponse>.Fail("Only pending claims can be approved.", 400);

        claim.Status = LeaveStatus.Approved;
        claim.DecidedAtUtc = DateTime.UtcNow;
        claim.DecidedById = currentUser.UserId;
        claim.ManagerComment = request.Comment?.Trim();
        await expenses.SaveChangesAsync(cancellationToken);

        await NotifyEmployeeAsync(claim, "expense.approved", "Expense approved",
            $"Your expense claim \"{claim.Title}\" was approved.", cancellationToken);

        var updated = await expenses.FindByIdAsync(id, cancellationToken);
        return ServiceResult<ExpenseClaimResponse>.Ok(Map(updated!));
    }

    public async Task<ServiceResult<ExpenseClaimResponse>> RejectAsync(Guid id, DecideExpenseRequest request, CancellationToken cancellationToken = default)
    {
        var claim = await expenses.FindByIdTrackedAsync(id, cancellationToken);
        if (claim is null)
            return ServiceResult<ExpenseClaimResponse>.Fail("Expense claim not found.", 404);
        if (!CanApprove(claim))
            return ServiceResult<ExpenseClaimResponse>.Fail("Forbidden.", 403);
        if (claim.Status != LeaveStatus.Pending)
            return ServiceResult<ExpenseClaimResponse>.Fail("Only pending claims can be rejected.", 400);

        claim.Status = LeaveStatus.Rejected;
        claim.DecidedAtUtc = DateTime.UtcNow;
        claim.DecidedById = currentUser.UserId;
        claim.ManagerComment = request.Comment?.Trim();
        await expenses.SaveChangesAsync(cancellationToken);

        await NotifyEmployeeAsync(claim, "expense.rejected", "Expense rejected",
            $"Your expense claim \"{claim.Title}\" was rejected.", cancellationToken);

        var updated = await expenses.FindByIdAsync(id, cancellationToken);
        return ServiceResult<ExpenseClaimResponse>.Ok(Map(updated!));
    }

    public async Task<ServiceResult<ExpenseClaimResponse>> CancelAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var claim = await expenses.FindByIdTrackedAsync(id, cancellationToken);
        if (claim is null)
            return ServiceResult<ExpenseClaimResponse>.Fail("Expense claim not found.", 404);
        if (!IsOwner(claim))
            return ServiceResult<ExpenseClaimResponse>.Fail("Forbidden.", 403);
        if (claim.Status is not LeaveStatus.Draft and not LeaveStatus.Pending)
            return ServiceResult<ExpenseClaimResponse>.Fail("Only draft or pending claims can be cancelled.", 400);

        claim.Status = LeaveStatus.Cancelled;
        await expenses.SaveChangesAsync(cancellationToken);
        var updated = await expenses.FindByIdAsync(id, cancellationToken);
        return ServiceResult<ExpenseClaimResponse>.Ok(Map(updated!));
    }

    private async Task NotifyManagerAsync(ExpenseClaim claim, string eventType, string title, string body, CancellationToken cancellationToken)
    {
        if (claim.Employee.ManagerId is Guid managerId)
        {
            var manager = await profiles.FindByUserIdAsync(managerId, cancellationToken);
            if (manager is not null)
            {
                await notifications.NotifyUserAsync(managerId, eventType, title, body, $"/expense-claims/{claim.Id}", cancellationToken);
            }
        }
    }

    private async Task NotifyEmployeeAsync(ExpenseClaim claim, string eventType, string title, string body, CancellationToken cancellationToken)
    {
        var employee = claim.Employee ?? await profiles.FindByUserIdAsync(claim.EmployeeId, cancellationToken);
        if (employee is null)
            return;

        await notifications.NotifyUserAsync(claim.EmployeeId, eventType, title, body, $"/expense-claims/{claim.Id}", cancellationToken);
    }

    private bool IsOwner(ExpenseClaim claim) => currentUser.UserId == claim.EmployeeId;

    private bool CanView(ExpenseClaim claim)
    {
        if (currentUser.UserId is null) return false;
        if (IsOwner(claim)) return true;
        if (currentUser.IsInAnyRole(UserRoles.Hr, UserRoles.Admin)) return true;
        if (currentUser.IsInRole(UserRoles.Manager) && claim.Employee.ManagerId == currentUser.UserId) return true;
        return false;
    }

    private bool CanApprove(ExpenseClaim claim)
    {
        if (currentUser.UserId is null) return false;
        if (currentUser.IsInAnyRole(UserRoles.Hr, UserRoles.Admin)) return true;
        if (currentUser.IsInRole(UserRoles.Manager) && claim.Employee.ManagerId == currentUser.UserId) return true;
        return false;
    }

    private static ExpenseClaimResponse Map(ExpenseClaim e) =>
        new(e.Id, e.EmployeeId, e.Employee.FullName, e.Title, e.TotalAmount, e.Status.ToString(),
            e.SubmittedAtUtc?.ToString("o"), e.DecidedAtUtc?.ToString("o"), e.ManagerComment,
            e.LineItems.Select(l => new ExpenseLineItemDto(l.Id, l.Description, l.Amount)).ToList());
}
