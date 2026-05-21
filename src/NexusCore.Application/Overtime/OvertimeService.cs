using NexusCore.Application.Common;
using NexusCore.Application.Notifications;
using NexusCore.Domain.Constants;
using NexusCore.Domain.Entities;
using NexusCore.Domain.Enums;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Application.Overtime;

public class OvertimeService(
    ICurrentUserService currentUser,
    IOvertimeRequestRepository overtimeRequests,
    IEmployeeProfileRepository profiles,
    INotificationService notifications) : IOvertimeService
{
    public async Task<IReadOnlyList<OvertimeRequestResponse>> ListAsync(string scope, CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is null)
            return [];

        IReadOnlyList<OvertimeRequest> list;
        if (string.Equals(scope, "pending-approval", StringComparison.OrdinalIgnoreCase))
        {
            if (currentUser.IsInAnyRole(UserRoles.Hr, UserRoles.Admin))
                list = await overtimeRequests.GetAllPendingAsync(cancellationToken);
            else if (currentUser.IsInRole(UserRoles.Manager))
                list = await overtimeRequests.GetPendingForManagerAsync(currentUser.UserId.Value, cancellationToken);
            else
                return [];
        }
        else
            list = await overtimeRequests.GetByEmployeeIdAsync(currentUser.UserId.Value, cancellationToken);

        return list.Select(Map).ToList();
    }

    public async Task<OvertimeRequestResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await overtimeRequests.FindByIdAsync(id, cancellationToken);
        if (item is null || !CanView(item))
            return null;
        return Map(item);
    }

    public async Task<ServiceResult<OvertimeRequestResponse>> CreateAsync(CreateOvertimeRequest request, CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is null)
            return ServiceResult<OvertimeRequestResponse>.Fail("Unauthorized.", 401);

        if (!DateOnly.TryParse(request.WorkDate, out var workDate))
            return ServiceResult<OvertimeRequestResponse>.Fail("Invalid date format. Use yyyy-MM-dd.", 400);

        if (request.Hours <= 0)
            return ServiceResult<OvertimeRequestResponse>.Fail("Hours must be greater than zero.", 400);

        var entity = new OvertimeRequest
        {
            Id = Guid.NewGuid(),
            EmployeeId = currentUser.UserId.Value,
            WorkDate = workDate,
            Hours = request.Hours,
            Reason = request.Reason.Trim(),
            Status = LeaveStatus.Draft,
            CreatedAtUtc = DateTime.UtcNow
        };

        await overtimeRequests.AddAsync(entity, cancellationToken);
        await overtimeRequests.SaveChangesAsync(cancellationToken);
        var created = await overtimeRequests.FindByIdAsync(entity.Id, cancellationToken);
        return ServiceResult<OvertimeRequestResponse>.Ok(Map(created!));
    }

    public async Task<ServiceResult<OvertimeRequestResponse>> SubmitAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await overtimeRequests.FindByIdTrackedAsync(id, cancellationToken);
        if (item is null)
            return ServiceResult<OvertimeRequestResponse>.Fail("Overtime request not found.", 404);
        if (!IsOwner(item))
            return ServiceResult<OvertimeRequestResponse>.Fail("Forbidden.", 403);
        if (item.Status != LeaveStatus.Draft)
            return ServiceResult<OvertimeRequestResponse>.Fail("Only draft requests can be submitted.", 400);

        item.Status = LeaveStatus.Pending;
        item.SubmittedAtUtc = DateTime.UtcNow;
        await overtimeRequests.SaveChangesAsync(cancellationToken);

        await PublishOvertimeEventAsync(item, "overtime.submitted", "Overtime request submitted",
            $"{item.Employee.FullName} submitted overtime on {item.WorkDate:yyyy-MM-dd}.", cancellationToken);

        var updated = await overtimeRequests.FindByIdAsync(id, cancellationToken);
        return ServiceResult<OvertimeRequestResponse>.Ok(Map(updated!));
    }

    public async Task<ServiceResult<OvertimeRequestResponse>> ApproveAsync(Guid id, DecideOvertimeRequest request, CancellationToken cancellationToken = default)
    {
        var item = await overtimeRequests.FindByIdTrackedAsync(id, cancellationToken);
        if (item is null)
            return ServiceResult<OvertimeRequestResponse>.Fail("Overtime request not found.", 404);
        if (!CanApprove(item))
            return ServiceResult<OvertimeRequestResponse>.Fail("Forbidden.", 403);
        if (item.Status != LeaveStatus.Pending)
            return ServiceResult<OvertimeRequestResponse>.Fail("Only pending requests can be approved.", 400);

        item.Status = LeaveStatus.Approved;
        item.DecidedAtUtc = DateTime.UtcNow;
        item.DecidedById = currentUser.UserId;
        item.ManagerComment = request.Comment?.Trim();
        await overtimeRequests.SaveChangesAsync(cancellationToken);

        await NotifyEmployeeAsync(item, "overtime.approved", "Overtime approved",
            $"Your overtime on {item.WorkDate:yyyy-MM-dd} was approved.", cancellationToken);

        var updated = await overtimeRequests.FindByIdAsync(id, cancellationToken);
        return ServiceResult<OvertimeRequestResponse>.Ok(Map(updated!));
    }

    public async Task<ServiceResult<OvertimeRequestResponse>> RejectAsync(Guid id, DecideOvertimeRequest request, CancellationToken cancellationToken = default)
    {
        var item = await overtimeRequests.FindByIdTrackedAsync(id, cancellationToken);
        if (item is null)
            return ServiceResult<OvertimeRequestResponse>.Fail("Overtime request not found.", 404);
        if (!CanApprove(item))
            return ServiceResult<OvertimeRequestResponse>.Fail("Forbidden.", 403);
        if (item.Status != LeaveStatus.Pending)
            return ServiceResult<OvertimeRequestResponse>.Fail("Only pending requests can be rejected.", 400);

        item.Status = LeaveStatus.Rejected;
        item.DecidedAtUtc = DateTime.UtcNow;
        item.DecidedById = currentUser.UserId;
        item.ManagerComment = request.Comment?.Trim();
        await overtimeRequests.SaveChangesAsync(cancellationToken);

        await NotifyEmployeeAsync(item, "overtime.rejected", "Overtime rejected",
            $"Your overtime on {item.WorkDate:yyyy-MM-dd} was rejected.", cancellationToken);

        var updated = await overtimeRequests.FindByIdAsync(id, cancellationToken);
        return ServiceResult<OvertimeRequestResponse>.Ok(Map(updated!));
    }

    public async Task<ServiceResult<OvertimeRequestResponse>> CancelAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await overtimeRequests.FindByIdTrackedAsync(id, cancellationToken);
        if (item is null)
            return ServiceResult<OvertimeRequestResponse>.Fail("Overtime request not found.", 404);
        if (!IsOwner(item))
            return ServiceResult<OvertimeRequestResponse>.Fail("Forbidden.", 403);
        if (item.Status is not LeaveStatus.Draft and not LeaveStatus.Pending)
            return ServiceResult<OvertimeRequestResponse>.Fail("Only draft or pending requests can be cancelled.", 400);

        item.Status = LeaveStatus.Cancelled;
        await overtimeRequests.SaveChangesAsync(cancellationToken);
        var updated = await overtimeRequests.FindByIdAsync(id, cancellationToken);
        return ServiceResult<OvertimeRequestResponse>.Ok(Map(updated!));
    }

    private async Task PublishOvertimeEventAsync(OvertimeRequest item, string eventType, string title, string body, CancellationToken cancellationToken)
    {
        if (item.Employee.ManagerId is Guid managerId)
        {
            var manager = await profiles.FindByUserIdAsync(managerId, cancellationToken);
            if (manager is not null)
            {
                await notifications.NotifyUserAsync(managerId, eventType, title, body, $"/overtime-requests/{item.Id}", cancellationToken);
            }
        }
    }

    private async Task NotifyEmployeeAsync(OvertimeRequest item, string eventType, string title, string body, CancellationToken cancellationToken)
    {
        var employee = item.Employee ?? await profiles.FindByUserIdAsync(item.EmployeeId, cancellationToken);
        if (employee is null)
            return;

        await notifications.NotifyUserAsync(item.EmployeeId, eventType, title, body, $"/overtime-requests/{item.Id}", cancellationToken);
    }

    private bool IsOwner(OvertimeRequest item) => currentUser.UserId == item.EmployeeId;

    private bool CanView(OvertimeRequest item)
    {
        if (currentUser.UserId is null) return false;
        if (IsOwner(item)) return true;
        if (currentUser.IsInAnyRole(UserRoles.Hr, UserRoles.Admin)) return true;
        if (currentUser.IsInRole(UserRoles.Manager) && item.Employee.ManagerId == currentUser.UserId) return true;
        return false;
    }

    private bool CanApprove(OvertimeRequest item)
    {
        if (currentUser.UserId is null) return false;
        if (currentUser.IsInAnyRole(UserRoles.Hr, UserRoles.Admin)) return true;
        if (currentUser.IsInRole(UserRoles.Manager) && item.Employee.ManagerId == currentUser.UserId) return true;
        return false;
    }

    private static OvertimeRequestResponse Map(OvertimeRequest o) =>
        new(o.Id, o.EmployeeId, o.Employee.FullName, o.WorkDate.ToString("yyyy-MM-dd"), o.Hours, o.Reason,
            o.Status.ToString(), o.SubmittedAtUtc?.ToString("o"), o.DecidedAtUtc?.ToString("o"), o.ManagerComment);
}
