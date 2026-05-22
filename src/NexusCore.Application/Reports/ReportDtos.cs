namespace NexusCore.Application.Reports;

/// <summary>แถวสรุปยอดวันลาในส่งออกรายงาน (ถ้าใช้ในอนาคต)</summary>
public record LeaveSummaryRow(
    string EmployeeName,
    string LeaveTypeName,
    int Year,
    decimal DaysAllowed,
    decimal DaysUsed,
    decimal DaysRemaining);
