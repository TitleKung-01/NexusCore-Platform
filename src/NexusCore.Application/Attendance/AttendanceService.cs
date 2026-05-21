using NexusCore.Application.Common;
using NexusCore.Domain.Constants;
using NexusCore.Domain.Entities;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Application.Attendance;

public class AttendanceService(
    ICurrentUserService currentUser,
    IAttendanceRepository attendance,
    IEmployeeProfileRepository profiles) : IAttendanceService
{
    public async Task<IReadOnlyList<AttendanceResponse>> ListAsync(string from, string to, Guid? employeeId, CancellationToken cancellationToken = default)
    {
        if (!DateOnly.TryParse(from, out var fromDate) || !DateOnly.TryParse(to, out var toDate))
            return [];

        var targetEmployee = employeeId ?? currentUser.UserId;
        if (targetEmployee is null)
            return [];

        if (targetEmployee != currentUser.UserId && !currentUser.IsInAnyRole(UserRoles.Hr, UserRoles.Admin, UserRoles.Manager))
            return [];

        if (targetEmployee != currentUser.UserId && currentUser.IsInRole(UserRoles.Manager))
        {
            var profile = await profiles.FindByUserIdAsync(targetEmployee.Value, cancellationToken);
            if (profile?.ManagerId != currentUser.UserId && !currentUser.IsInAnyRole(UserRoles.Hr, UserRoles.Admin))
                return [];
        }

        var list = await attendance.ListAsync(targetEmployee, fromDate, toDate, cancellationToken);
        return list.Select(Map).ToList();
    }

    public async Task<ServiceResult<AttendanceResponse>> CheckInAsync(CheckInRequest request, CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is null)
            return ServiceResult<AttendanceResponse>.Fail("Unauthorized.", 401);

        if (!DateOnly.TryParse(request.WorkDate, out var workDate))
            return ServiceResult<AttendanceResponse>.Fail("Invalid date format. Use yyyy-MM-dd.", 400);

        var record = await attendance.FindByEmployeeAndDateTrackedAsync(currentUser.UserId.Value, workDate, cancellationToken);
        if (record is null)
        {
            record = new AttendanceRecord
            {
                Id = Guid.NewGuid(),
                EmployeeId = currentUser.UserId.Value,
                WorkDate = workDate,
                CheckInUtc = DateTime.UtcNow
            };
            await attendance.AddAsync(record, cancellationToken);
        }
        else if (record.CheckInUtc is not null)
            return ServiceResult<AttendanceResponse>.Fail("Already checked in for this date.", 409);
        else
            record.CheckInUtc = DateTime.UtcNow;

        await attendance.SaveChangesAsync(cancellationToken);
        var updated = await attendance.FindByIdAsync(record.Id, cancellationToken);
        return ServiceResult<AttendanceResponse>.Ok(Map(updated!));
    }

    public async Task<ServiceResult<AttendanceResponse>> CheckOutAsync(CheckOutRequest request, CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is null)
            return ServiceResult<AttendanceResponse>.Fail("Unauthorized.", 401);

        if (!DateOnly.TryParse(request.WorkDate, out var workDate))
            return ServiceResult<AttendanceResponse>.Fail("Invalid date format. Use yyyy-MM-dd.", 400);

        var record = await attendance.FindByEmployeeAndDateTrackedAsync(currentUser.UserId.Value, workDate, cancellationToken);
        if (record is null || record.CheckInUtc is null)
            return ServiceResult<AttendanceResponse>.Fail("Check in first.", 400);
        if (record.CheckOutUtc is not null)
            return ServiceResult<AttendanceResponse>.Fail("Already checked out for this date.", 409);

        record.CheckOutUtc = DateTime.UtcNow;
        await attendance.SaveChangesAsync(cancellationToken);
        var updated = await attendance.FindByIdAsync(record.Id, cancellationToken);
        return ServiceResult<AttendanceResponse>.Ok(Map(updated!));
    }

    private static AttendanceResponse Map(AttendanceRecord a) =>
        new(
            a.Id,
            a.EmployeeId,
            a.Employee?.FullName ?? string.Empty,
            a.WorkDate.ToString("yyyy-MM-dd"),
            a.CheckInUtc?.ToString("o"),
            a.CheckOutUtc?.ToString("o"));
}
