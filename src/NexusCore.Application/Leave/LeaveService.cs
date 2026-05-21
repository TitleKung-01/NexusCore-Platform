using NexusCore.Application.Common;
using NexusCore.Domain.Constants;
using NexusCore.Domain.Entities;
using NexusCore.Domain.Enums;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Application.Leave;

public class LeaveService(
    ICurrentUserService currentUser,
    ILeaveRequestRepository leaveRequests,
    ILeaveTypeRepository leaveTypes) : ILeaveService
{
    public async Task<IReadOnlyList<LeaveRequestResponse>> ListAsync(string scope, CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is null)
            return [];

        IReadOnlyList<LeaveRequest> list;
        if (string.Equals(scope, "pending-approval", StringComparison.OrdinalIgnoreCase))
        {
            if (currentUser.IsInAnyRole(UserRoles.Hr, UserRoles.Admin))
                list = await leaveRequests.GetAllPendingAsync(cancellationToken);
            else if (currentUser.IsInRole(UserRoles.Manager))
                list = await leaveRequests.GetPendingForManagerAsync(currentUser.UserId.Value, cancellationToken);
            else
                return [];
        }
        else
            list = await leaveRequests.GetByEmployeeIdAsync(currentUser.UserId.Value, cancellationToken);

        return list.Select(Map).ToList();
    }

    public async Task<LeaveRequestResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var leave = await leaveRequests.FindByIdAsync(id, cancellationToken);
        if (leave is null || !CanView(leave))
            return null;
        return Map(leave);
    }

    public async Task<ServiceResult<LeaveRequestResponse>> CreateAsync(CreateLeaveRequest request, CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is null)
            return ServiceResult<LeaveRequestResponse>.Fail("Unauthorized.", 401);

        if (!DateOnly.TryParse(request.StartDate, out var start) || !DateOnly.TryParse(request.EndDate, out var end))
            return ServiceResult<LeaveRequestResponse>.Fail("Invalid date format. Use yyyy-MM-dd.", 400);

        if (end < start)
            return ServiceResult<LeaveRequestResponse>.Fail("End date must be on or after start date.", 400);

        var type = await leaveTypes.FindByIdAsync(request.LeaveTypeId, cancellationToken);
        if (type is null)
            return ServiceResult<LeaveRequestResponse>.Fail("Leave type not found.", 404);

        if (await leaveRequests.HasOverlappingAsync(currentUser.UserId.Value, start, end, null, cancellationToken))
            return ServiceResult<LeaveRequestResponse>.Fail("Leave dates overlap with an existing request.", 409);

        var entity = new LeaveRequest
        {
            Id = Guid.NewGuid(),
            EmployeeId = currentUser.UserId.Value,
            LeaveTypeId = request.LeaveTypeId,
            StartDate = start,
            EndDate = end,
            Reason = request.Reason.Trim(),
            Status = LeaveStatus.Draft,
            CreatedAtUtc = DateTime.UtcNow
        };

        await leaveRequests.AddAsync(entity, cancellationToken);
        await leaveRequests.SaveChangesAsync(cancellationToken);

        var created = await leaveRequests.FindByIdAsync(entity.Id, cancellationToken);
        return ServiceResult<LeaveRequestResponse>.Ok(Map(created!));
    }

    public async Task<ServiceResult<LeaveRequestResponse>> SubmitAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var leave = await leaveRequests.FindByIdTrackedAsync(id, cancellationToken);
        if (leave is null)
            return ServiceResult<LeaveRequestResponse>.Fail("Leave request not found.", 404);
        if (!IsOwner(leave))
            return ServiceResult<LeaveRequestResponse>.Fail("Forbidden.", 403);
        if (leave.Status != LeaveStatus.Draft)
            return ServiceResult<LeaveRequestResponse>.Fail("Only draft requests can be submitted.", 400);

        if (await leaveRequests.HasOverlappingAsync(leave.EmployeeId, leave.StartDate, leave.EndDate, leave.Id, cancellationToken))
            return ServiceResult<LeaveRequestResponse>.Fail("Leave dates overlap with an existing request.", 409);

        leave.Status = LeaveStatus.Pending;
        leave.SubmittedAtUtc = DateTime.UtcNow;
        await leaveRequests.SaveChangesAsync(cancellationToken);

        var updated = await leaveRequests.FindByIdAsync(id, cancellationToken);
        return ServiceResult<LeaveRequestResponse>.Ok(Map(updated!));
    }

    public async Task<ServiceResult<LeaveRequestResponse>> ApproveAsync(Guid id, DecideLeaveRequest request, CancellationToken cancellationToken = default)
    {
        var leave = await leaveRequests.FindByIdTrackedAsync(id, cancellationToken);
        if (leave is null)
            return ServiceResult<LeaveRequestResponse>.Fail("Leave request not found.", 404);
        if (!CanApprove(leave))
            return ServiceResult<LeaveRequestResponse>.Fail("Forbidden.", 403);
        if (leave.Status != LeaveStatus.Pending)
            return ServiceResult<LeaveRequestResponse>.Fail("Only pending requests can be approved.", 400);

        leave.Status = LeaveStatus.Approved;
        leave.DecidedAtUtc = DateTime.UtcNow;
        leave.DecidedById = currentUser.UserId;
        leave.ManagerComment = request.Comment?.Trim();
        await leaveRequests.SaveChangesAsync(cancellationToken);

        var updated = await leaveRequests.FindByIdAsync(id, cancellationToken);
        return ServiceResult<LeaveRequestResponse>.Ok(Map(updated!));
    }

    public async Task<ServiceResult<LeaveRequestResponse>> RejectAsync(Guid id, DecideLeaveRequest request, CancellationToken cancellationToken = default)
    {
        var leave = await leaveRequests.FindByIdTrackedAsync(id, cancellationToken);
        if (leave is null)
            return ServiceResult<LeaveRequestResponse>.Fail("Leave request not found.", 404);
        if (!CanApprove(leave))
            return ServiceResult<LeaveRequestResponse>.Fail("Forbidden.", 403);
        if (leave.Status != LeaveStatus.Pending)
            return ServiceResult<LeaveRequestResponse>.Fail("Only pending requests can be rejected.", 400);

        leave.Status = LeaveStatus.Rejected;
        leave.DecidedAtUtc = DateTime.UtcNow;
        leave.DecidedById = currentUser.UserId;
        leave.ManagerComment = request.Comment?.Trim();
        await leaveRequests.SaveChangesAsync(cancellationToken);

        var updated = await leaveRequests.FindByIdAsync(id, cancellationToken);
        return ServiceResult<LeaveRequestResponse>.Ok(Map(updated!));
    }

    public async Task<ServiceResult<LeaveRequestResponse>> CancelAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var leave = await leaveRequests.FindByIdTrackedAsync(id, cancellationToken);
        if (leave is null)
            return ServiceResult<LeaveRequestResponse>.Fail("Leave request not found.", 404);
        if (!IsOwner(leave))
            return ServiceResult<LeaveRequestResponse>.Fail("Forbidden.", 403);
        if (leave.Status is not LeaveStatus.Draft and not LeaveStatus.Pending)
            return ServiceResult<LeaveRequestResponse>.Fail("Only draft or pending requests can be cancelled.", 400);

        leave.Status = LeaveStatus.Cancelled;
        await leaveRequests.SaveChangesAsync(cancellationToken);

        var updated = await leaveRequests.FindByIdAsync(id, cancellationToken);
        return ServiceResult<LeaveRequestResponse>.Ok(Map(updated!));
    }

    private bool IsOwner(LeaveRequest leave) =>
        currentUser.UserId == leave.EmployeeId;

    private bool CanView(LeaveRequest leave)
    {
        if (currentUser.UserId is null)
            return false;
        if (IsOwner(leave))
            return true;
        if (currentUser.IsInAnyRole(UserRoles.Hr, UserRoles.Admin))
            return true;
        if (currentUser.IsInRole(UserRoles.Manager) && leave.Employee.ManagerId == currentUser.UserId)
            return true;
        return false;
    }

    private bool CanApprove(LeaveRequest leave)
    {
        if (currentUser.UserId is null)
            return false;
        if (currentUser.IsInAnyRole(UserRoles.Hr, UserRoles.Admin))
            return true;
        if (currentUser.IsInRole(UserRoles.Manager) && leave.Employee.ManagerId == currentUser.UserId)
            return true;
        return false;
    }

    private static LeaveRequestResponse Map(LeaveRequest l) =>
        new(
            l.Id,
            l.EmployeeId,
            l.Employee.FullName,
            l.LeaveTypeId,
            l.LeaveType.Name,
            l.StartDate.ToString("yyyy-MM-dd"),
            l.EndDate.ToString("yyyy-MM-dd"),
            l.Reason,
            l.Status.ToString(),
            l.SubmittedAtUtc?.ToString("o"),
            l.DecidedAtUtc?.ToString("o"),
            l.ManagerComment);
}
