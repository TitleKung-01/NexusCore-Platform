using System.Globalization;
using System.Text;
using NexusCore.Application.Common;
using NexusCore.Domain.Constants;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Application.Reports;

public class ReportService(
    ICurrentUserService currentUser,
    IEmployeeProfileRepository profiles,
    ILeaveEntitlementRepository entitlements,
    ILeaveRequestRepository leaveRequests) : IReportService
{
    private const string CsvHeader =
        "Username,Email,Department,LeaveType,Year,DaysAllowed,DaysUsed,DaysRemaining";

    public async Task<byte[]> ExportLeaveSummaryCsvAsync(int year, CancellationToken cancellationToken = default)
    {
        if (!currentUser.IsInAnyRole(UserRoles.Hr, UserRoles.Admin))
            return Utf8Csv($"{CsvHeader}\n");

        var employees = await profiles.GetAllWithDetailsAsync(cancellationToken);
        var sb = new StringBuilder();
        sb.AppendLine(CsvHeader);

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
