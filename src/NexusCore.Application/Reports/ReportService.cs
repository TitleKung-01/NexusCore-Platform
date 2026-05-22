using System.Globalization;
using System.Text;
using NexusCore.Application.Attendance;
using NexusCore.Application.Common;
using NexusCore.Domain.Constants;
using NexusCore.Domain.Entities;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Application.Reports;

/// <summary>
/// ส่งออกรายงาน CSV สรุปลาและการลงเวลา
/// </summary>
public class ReportService(
    ICurrentUserService currentUser,
    IEmployeeProfileRepository profiles,
    ILeaveEntitlementRepository entitlements,
    ILeaveRequestRepository leaveRequests,
    IAttendanceRepository attendance) : IReportService
{
    private const string LeaveCsvHeader =
        "Username,Email,Department,LeaveType,Year,DaysAllowed,DaysUsed,DaysRemaining";

    private const string AttendanceCsvHeader =
        "Username,Email,Department,FullName,WorkDate,CheckIn,CheckOut,Status,LateMinutes,IsLate,IsEarlyCheckout,WorkSummary";

    /// <inheritdoc />
    public async Task<byte[]> ExportLeaveSummaryCsvAsync(int year, CancellationToken cancellationToken = default)
    {
        if (!currentUser.IsInAnyRole(UserRoles.Hr, UserRoles.Admin))
            return Utf8Csv($"{LeaveCsvHeader}\n");

        var employees = await profiles.GetAllWithDetailsAsync(cancellationToken);
        var sb = new StringBuilder();
        sb.AppendLine(LeaveCsvHeader);

        foreach (var emp in employees)
        {
            var department = emp.Department?.Code ?? "";
            var username = emp.User?.Username ?? "";
            var email = emp.Email;

            var ents = await entitlements.ListForEmployeeAsync(emp.UserId, year, cancellationToken);
            foreach (var e in ents)
            {
                var used = await leaveRequests.SumApprovedDaysAsync(emp.UserId, e.LeaveTypeId, year, cancellationToken);
                var remaining = e.DaysAllowed - used;
                sb.AppendLine(string.Join(",",
                    Csv(username),
                    Csv(email),
                    Csv(department),
                    Csv(EnglishLeaveType(e.LeaveType.Code)),
                    year.ToString(CultureInfo.InvariantCulture),
                    e.DaysAllowed.ToString(CultureInfo.InvariantCulture),
                    used.ToString(CultureInfo.InvariantCulture),
                    remaining.ToString(CultureInfo.InvariantCulture)));
            }
        }

        return Utf8Csv(sb.ToString());
    }

    /// <inheritdoc />
    public async Task<byte[]> ExportAttendanceCsvAsync(string from, string to, Guid? employeeId, CancellationToken cancellationToken = default)
    {
        if (!currentUser.IsInAnyRole(UserRoles.Hr, UserRoles.Admin))
            return Utf8Csv($"{AttendanceCsvHeader}\n");

        if (!DateOnly.TryParse(from, out var fromDate) || !DateOnly.TryParse(to, out var toDate) || fromDate > toDate)
            return Utf8Csv($"{AttendanceCsvHeader}\n");

        var records = await attendance.ListAsync(employeeId, fromDate, toDate, cancellationToken);
        var sb = new StringBuilder();
        sb.AppendLine(AttendanceCsvHeader);

        foreach (var a in records.OrderBy(r => r.WorkDate).ThenBy(r => r.Employee.FullName))
        {
            var emp = a.Employee;
            var username = emp.User?.Username ?? "";
            var department = emp.Department?.Code ?? "";
            var checkIn = a.CheckInUtc is null ? "" : AttendanceTimeRules.FormatLocalTime(a.CheckInUtc.Value);
            var checkOut = a.CheckOutUtc is null ? "" : AttendanceTimeRules.FormatLocalTime(a.CheckOutUtc.Value);
            var isLate = a.CheckInUtc is not null && AttendanceTimeRules.IsLateCheckIn(a.CheckInUtc.Value);
            var lateMinutes = a.CheckInUtc is null ? 0 : AttendanceTimeRules.LateMinutes(a.CheckInUtc.Value);
            var isEarly = a.CheckOutUtc is not null && AttendanceTimeRules.IsEarlyCheckOut(a.CheckOutUtc.Value);

            sb.AppendLine(string.Join(",",
                Csv(username),
                Csv(emp.Email),
                Csv(department),
                Csv(emp.FullName),
                a.WorkDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                Csv(checkIn),
                Csv(checkOut),
                Csv(EnglishAttendanceStatus(a)),
                lateMinutes.ToString(CultureInfo.InvariantCulture),
                isLate ? "Yes" : "No",
                isEarly ? "Yes" : "No",
                Csv(a.WorkSummary ?? "")));
        }

        return Utf8Csv(sb.ToString());
    }

    private static string EnglishAttendanceStatus(AttendanceRecord a)
    {
        if (a.CheckInUtc is null)
            return "No check-in";
        if (a.CheckOutUtc is null)
            return "Checked in only";

        var late = AttendanceTimeRules.IsLateCheckIn(a.CheckInUtc.Value);
        var early = AttendanceTimeRules.IsEarlyCheckOut(a.CheckOutUtc.Value);
        if (late && early)
            return "Late and early checkout";
        if (late)
            return "Late";
        if (early)
            return "Early checkout";
        return "On time";
    }

    private static string EnglishLeaveType(string code) =>
        code.ToUpperInvariant() switch
        {
            "SICK" => "Sick Leave",
            "PERSONAL" => "Personal Leave",
            "ANNUAL" => "Annual Leave",
            _ => code
        };

    private static byte[] Utf8Csv(string content)
    {
        var preamble = Encoding.UTF8.GetPreamble();
        var body = Encoding.UTF8.GetBytes(content);
        var result = new byte[preamble.Length + body.Length];
        preamble.CopyTo(result, 0);
        body.CopyTo(result, preamble.Length);
        return result;
    }

    private static string Csv(string value) =>
        $"\"{value.Replace("\"", "\"\"", StringComparison.Ordinal)}\"";
}