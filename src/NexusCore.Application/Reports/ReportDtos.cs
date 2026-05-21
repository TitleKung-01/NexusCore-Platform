namespace NexusCore.Application.Reports;

public record LeaveSummaryRow(
    string EmployeeName,
    string LeaveTypeName,
    int Year,
    decimal DaysAllowed,
    decimal DaysUsed,
    decimal DaysRemaining);
