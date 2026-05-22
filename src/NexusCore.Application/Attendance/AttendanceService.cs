using NexusCore.Application.Common;
using NexusCore.Domain.Constants;
using NexusCore.Domain.Entities;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Application.Attendance;

/// <summary>
/// ลงเวลาเข้า-ออก ตรวจสาย/ออกก่อนเวลา และดูประวัติตามสิทธิ์
/// </summary>
public class AttendanceService(
    ICurrentUserService currentUser,
    IAttendanceRepository attendance,
    IEmployeeProfileRepository profiles) : IAttendanceService
{
    private const int MaxWorkSummaryLength = 2000;

    /// <inheritdoc />
    public async Task<AttendanceResponse?> GetTodayAsync(CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is null)
            return null;

        var today = DateOnly.FromDateTime(AttendanceTimeRules.ToBangkokLocal(DateTime.UtcNow));
        var record = await attendance.FindByEmployeeAndDateAsync(currentUser.UserId.Value, today, cancellationToken);
        return record is null ? null : Map(record);
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
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
            return ServiceResult<AttendanceResponse>.Fail("เธฅเธเน€เธงเธฅเธฒเน€เธเนเธฒเนเธฅเนเธงเธชเธณเธซเธฃเธฑเธเธงเธฑเธเธเธตเน", 409);
        else
            record.CheckInUtc = DateTime.UtcNow;

        await attendance.SaveChangesAsync(cancellationToken);
        var updated = await attendance.FindByIdAsync(record.Id, cancellationToken);
        return ServiceResult<AttendanceResponse>.Ok(Map(updated!));
    }

    /// <inheritdoc />
    public async Task<ServiceResult<AttendanceResponse>> CheckOutAsync(CheckOutRequest request, CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is null)
            return ServiceResult<AttendanceResponse>.Fail("Unauthorized.", 401);

        if (!DateOnly.TryParse(request.WorkDate, out var workDate))
            return ServiceResult<AttendanceResponse>.Fail("Invalid date format. Use yyyy-MM-dd.", 400);

        var record = await attendance.FindByEmployeeAndDateTrackedAsync(currentUser.UserId.Value, workDate, cancellationToken);
        if (record is null || record.CheckInUtc is null)
            return ServiceResult<AttendanceResponse>.Fail("เธ•เนเธญเธเธฅเธเน€เธงเธฅเธฒเน€เธเนเธฒเธเนเธญเธ", 400);
        if (record.CheckOutUtc is not null)
            return ServiceResult<AttendanceResponse>.Fail("เธฅเธเน€เธงเธฅเธฒเธญเธญเธเนเธฅเนเธงเธชเธณเธซเธฃเธฑเธเธงเธฑเธเธเธตเน", 409);

        record.CheckOutUtc = DateTime.UtcNow;
        record.WorkSummary = NormalizeWorkSummary(request.WorkSummary);

        await attendance.SaveChangesAsync(cancellationToken);
        var updated = await attendance.FindByIdAsync(record.Id, cancellationToken);
        return ServiceResult<AttendanceResponse>.Ok(Map(updated!));
    }

    private static string? NormalizeWorkSummary(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;
        var trimmed = value.Trim();
        return trimmed.Length > MaxWorkSummaryLength ? trimmed[..MaxWorkSummaryLength] : trimmed;
    }

    private static AttendanceResponse Map(AttendanceRecord a)
    {
        var checkInLocal = a.CheckInUtc is null ? null : AttendanceTimeRules.FormatLocalTime(a.CheckInUtc.Value);
        var checkOutLocal = a.CheckOutUtc is null ? null : AttendanceTimeRules.FormatLocalTime(a.CheckOutUtc.Value);
        var isLate = a.CheckInUtc is not null && AttendanceTimeRules.IsLateCheckIn(a.CheckInUtc.Value);
        var lateMinutes = a.CheckInUtc is null ? 0 : AttendanceTimeRules.LateMinutes(a.CheckInUtc.Value);
        var isEarly = a.CheckOutUtc is not null && AttendanceTimeRules.IsEarlyCheckOut(a.CheckOutUtc.Value);

        return new AttendanceResponse(
            a.Id,
            a.EmployeeId,
            a.Employee?.FullName ?? string.Empty,
            a.WorkDate.ToString("yyyy-MM-dd"),
            a.CheckInUtc?.ToString("o"),
            a.CheckOutUtc?.ToString("o"),
            checkInLocal,
            checkOutLocal,
            isLate,
            lateMinutes,
            isEarly,
            AttendanceTimeRules.BuildStatusLabel(a.CheckInUtc, a.CheckOutUtc),
            a.WorkSummary,
            a.CheckInUtc is null,
            a.CheckInUtc is not null && a.CheckOutUtc is null);
    }
}