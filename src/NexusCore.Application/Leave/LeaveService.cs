using NexusCore.Application.Common;
using NexusCore.Application.Notifications;
using NexusCore.Domain.Constants;
using NexusCore.Domain.Entities;
using NexusCore.Domain.Enums;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Application.Leave;

public class LeaveService(
    ICurrentUserService currentUser,
    ILeaveRequestRepository leaveRequests,
    ILeaveTypeRepository leaveTypes,
    ILeaveEntitlementRepository entitlements,
    ILeaveAttachmentRepository attachments,
    IEmployeeProfileRepository profiles,
    IFileStorage fileStorage,
    INotificationService notifications) : ILeaveService
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
        else if (string.Equals(scope, "approval-history", StringComparison.OrdinalIgnoreCase))
        {
            Guid? managerFilter = currentUser.IsInAnyRole(UserRoles.Hr, UserRoles.Admin)
                ? null
                : currentUser.IsInRole(UserRoles.Manager) ? currentUser.UserId : null;
            if (managerFilter is null && !currentUser.IsInAnyRole(UserRoles.Hr, UserRoles.Admin, UserRoles.Manager))
                return [];
            list = await leaveRequests.GetApprovalHistoryAsync(managerFilter, cancellationToken);
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

        var entitlementError = await ValidateEntitlementAsync(leave, cancellationToken);
        if (entitlementError is not null)
            return ServiceResult<LeaveRequestResponse>.Fail(entitlementError, 400);

        leave.Status = LeaveStatus.Pending;
        leave.SubmittedAtUtc = DateTime.UtcNow;
        await leaveRequests.SaveChangesAsync(cancellationToken);

        await NotifyApproversAsync(leave, "leave.submitted", "Leave request submitted",
            $"{leave.Employee.FullName} submitted leave ({leave.StartDate:yyyy-MM-dd} – {leave.EndDate:yyyy-MM-dd}).",
            $"/leave-requests/{leave.Id}", cancellationToken);

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

        await NotifyEmployeeAsync(leave, "leave.approved", "Leave request approved",
            $"Your leave ({leave.StartDate:yyyy-MM-dd} – {leave.EndDate:yyyy-MM-dd}) was approved.",
            $"/leave-requests/{leave.Id}", cancellationToken);

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

        await NotifyEmployeeAsync(leave, "leave.rejected", "Leave request rejected",
            $"Your leave ({leave.StartDate:yyyy-MM-dd} – {leave.EndDate:yyyy-MM-dd}) was rejected.",
            $"/leave-requests/{leave.Id}", cancellationToken);

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

        if (leave.Status == LeaveStatus.Cancelled)
        {
            await NotifyEmployeeAsync(leave, "leave.cancelled", "Leave request cancelled",
                $"Your leave request was cancelled.", $"/leave-requests/{leave.Id}", cancellationToken);
        }

        var updated = await leaveRequests.FindByIdAsync(id, cancellationToken);
        return ServiceResult<LeaveRequestResponse>.Ok(Map(updated!));
    }

    public async Task<IReadOnlyList<LeaveBalanceResponse>> GetBalancesAsync(int? year, Guid? employeeId, CancellationToken cancellationToken = default)
    {
        var targetYear = year ?? DateTime.UtcNow.Year;
        var targetEmployee = employeeId ?? currentUser.UserId;
        if (targetEmployee is null)
            return [];

        if (targetEmployee != currentUser.UserId && !currentUser.IsInAnyRole(UserRoles.Hr, UserRoles.Admin, UserRoles.Manager))
            return [];

        if (targetEmployee != currentUser.UserId && currentUser.IsInRole(UserRoles.Manager))
        {
            var profile = await profiles.FindByUserIdAsync(targetEmployee.Value, cancellationToken);
            if (profile?.ManagerId != currentUser.UserId)
                return [];
        }

        var list = await entitlements.ListForEmployeeAsync(targetEmployee.Value, targetYear, cancellationToken);
        var results = new List<LeaveBalanceResponse>();
        foreach (var e in list)
        {
            var used = await leaveRequests.SumApprovedDaysAsync(targetEmployee.Value, e.LeaveTypeId, targetYear, cancellationToken);
            var pending = await SumPendingDaysAsync(targetEmployee.Value, e.LeaveTypeId, targetYear, cancellationToken);
            results.Add(new LeaveBalanceResponse(
                e.LeaveTypeId,
                e.LeaveType.Name,
                e.Year,
                e.DaysAllowed,
                used + pending,
                e.DaysAllowed - used - pending));
        }

        return results;
    }

    public async Task<IReadOnlyList<LeaveCalendarEntryResponse>> GetCalendarAsync(string from, string to, Guid? departmentId, CancellationToken cancellationToken = default)
    {
        if (!DateOnly.TryParse(from, out var fromDate) || !DateOnly.TryParse(to, out var toDate))
            return [];

        if (!currentUser.IsInAnyRole(UserRoles.Hr, UserRoles.Admin, UserRoles.Manager))
            departmentId = null;

        var list = await leaveRequests.GetCalendarAsync(fromDate, toDate, departmentId, cancellationToken);
        return list.Select(l => new LeaveCalendarEntryResponse(
            l.Id,
            l.EmployeeId,
            l.Employee.FullName,
            l.LeaveType.Name,
            l.StartDate.ToString("yyyy-MM-dd"),
            l.EndDate.ToString("yyyy-MM-dd"),
            l.Status.ToString())).ToList();
    }

    public async Task<ServiceResult<LeaveAttachmentResponse>> UploadAttachmentAsync(Guid leaveRequestId, string fileName, Stream content, CancellationToken cancellationToken = default)
    {
        var leave = await leaveRequests.FindByIdAsync(leaveRequestId, cancellationToken);
        if (leave is null)
            return ServiceResult<LeaveAttachmentResponse>.Fail("Leave request not found.", 404);
        if (!IsOwner(leave) && !currentUser.IsInAnyRole(UserRoles.Hr, UserRoles.Admin))
            return ServiceResult<LeaveAttachmentResponse>.Fail("Forbidden.", 403);

        var stored = await fileStorage.SaveAsync("leave-attachments", fileName, content, cancellationToken);
        var attachment = new LeaveAttachment
        {
            Id = Guid.NewGuid(),
            LeaveRequestId = leaveRequestId,
            FileName = Path.GetFileName(fileName),
            StoragePath = stored.StoragePath,
            ContentType = stored.ContentType,
            SizeBytes = stored.SizeBytes,
            UploadedAtUtc = DateTime.UtcNow
        };
        await attachments.AddAsync(attachment, cancellationToken);
        await attachments.SaveChangesAsync(cancellationToken);
        return ServiceResult<LeaveAttachmentResponse>.Ok(MapAttachment(attachment));
    }

    public async Task<(Stream Stream, string ContentType, string FileName)?> DownloadAttachmentAsync(Guid attachmentId, CancellationToken cancellationToken = default)
    {
        var attachment = await attachments.FindByIdAsync(attachmentId, cancellationToken);
        if (attachment is null)
            return null;

        var leave = await leaveRequests.FindByIdAsync(attachment.LeaveRequestId, cancellationToken);
        if (leave is null || !CanView(leave))
            return null;

        var stream = await fileStorage.OpenReadAsync(attachment.StoragePath, cancellationToken);
        if (stream is null)
            return null;

        return (stream, attachment.ContentType, attachment.FileName);
    }

    public async Task<IReadOnlyList<LeaveAttachmentResponse>> ListAttachmentsAsync(Guid leaveRequestId, CancellationToken cancellationToken = default)
    {
        var leave = await leaveRequests.FindByIdAsync(leaveRequestId, cancellationToken);
        if (leave is null || !CanView(leave))
            return [];

        var list = await attachments.ListByLeaveRequestIdAsync(leaveRequestId, cancellationToken);
        return list.Select(MapAttachment).ToList();
    }

    private async Task<string?> ValidateEntitlementAsync(LeaveRequest leave, CancellationToken cancellationToken)
    {
        var year = leave.StartDate.Year;
        var entitlement = await entitlements.GetAsync(leave.EmployeeId, leave.LeaveTypeId, year, cancellationToken);
        if (entitlement is null)
            return "No leave entitlement configured for this leave type and year.";

        var requestedDays = LeaveDayCalculator.CountInclusiveDays(leave.StartDate, leave.EndDate);
        var used = await leaveRequests.SumApprovedDaysAsync(leave.EmployeeId, leave.LeaveTypeId, year, cancellationToken);
        var pending = await SumPendingDaysAsync(leave.EmployeeId, leave.LeaveTypeId, year, cancellationToken);
        if (used + pending + requestedDays > entitlement.DaysAllowed)
            return $"Insufficient leave balance. Remaining: {entitlement.DaysAllowed - used - pending} day(s).";

        return null;
    }

    private async Task<decimal> SumPendingDaysAsync(Guid employeeId, Guid leaveTypeId, int year, CancellationToken cancellationToken)
    {
        var pending = await leaveRequests.GetByEmployeeIdAsync(employeeId, cancellationToken);
        decimal total = 0;
        foreach (var l in pending.Where(l => l.LeaveTypeId == leaveTypeId && l.Status == LeaveStatus.Pending && (l.StartDate.Year == year || l.EndDate.Year == year)))
            total += LeaveDayCalculator.CountInclusiveDays(l.StartDate, l.EndDate);
        return total;
    }

    private async Task NotifyApproversAsync(LeaveRequest leave, string eventType, string title, string body, string linkPath, CancellationToken cancellationToken)
    {
        if (leave.Employee.ManagerId is Guid managerId)
        {
            var manager = await profiles.FindByUserIdAsync(managerId, cancellationToken);
            if (manager is not null)
            {
                await notifications.NotifyUserAsync(managerId, eventType, title, body, linkPath, cancellationToken);
            }
        }
    }

    private async Task NotifyEmployeeAsync(LeaveRequest leave, string eventType, string title, string body, string linkPath, CancellationToken cancellationToken)
    {
        var employee = leave.Employee ?? await profiles.FindByUserIdAsync(leave.EmployeeId, cancellationToken);
        if (employee is null)
            return;

        await notifications.NotifyUserAsync(leave.EmployeeId, eventType, title, body, linkPath, cancellationToken);
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

    private static LeaveRequestResponse Map(LeaveRequest l)
    {
        var emp = RequestEmployeeFields.From(l.Employee);
        return new(
            l.Id,
            l.EmployeeId,
            l.Employee.FullName,
            emp.Username,
            emp.Email,
            emp.DepartmentName,
            emp.Role,
            emp.ManagerName,
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

    private static LeaveAttachmentResponse MapAttachment(LeaveAttachment a) =>
        new(a.Id, a.LeaveRequestId, a.FileName, a.ContentType, a.SizeBytes, a.UploadedAtUtc.ToString("o"));
}
