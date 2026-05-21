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
    public async Task<byte[]> ExportLeaveSummaryCsvAsync(int year, CancellationToken cancellationToken = default)
    {
        if (!currentUser.IsInAnyRole(UserRoles.Hr, UserRoles.Admin))
            return Encoding.UTF8.GetBytes("EmployeeName,LeaveType,Year,DaysAllowed,DaysUsed,DaysRemaining\n");

        var employees = await profiles.GetAllWithDetailsAsync(cancellationToken);
        var sb = new StringBuilder();
        sb.AppendLine("EmployeeName,LeaveType,Year,DaysAllowed,DaysUsed,DaysRemaining");

        foreach (var emp in employees)
        {
            var ents = await entitlements.ListForEmployeeAsync(emp.UserId, year, cancellationToken);
            foreach (var e in ents)
            {
                var used = await leaveRequests.SumApprovedDaysAsync(emp.UserId, e.LeaveTypeId, year, cancellationToken);
                var remaining = e.DaysAllowed - used;
                sb.AppendLine(string.Join(",",
                    Csv(emp.FullName),
                    Csv(e.LeaveType.Name),
                    year.ToString(CultureInfo.InvariantCulture),
                    e.DaysAllowed.ToString(CultureInfo.InvariantCulture),
                    used.ToString(CultureInfo.InvariantCulture),
                    remaining.ToString(CultureInfo.InvariantCulture)));
            }
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    private static string Csv(string value) =>
        $"\"{value.Replace("\"", "\"\"", StringComparison.Ordinal)}\"";
}
