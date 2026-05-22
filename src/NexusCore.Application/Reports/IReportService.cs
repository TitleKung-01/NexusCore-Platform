namespace NexusCore.Application.Reports;

/// <summary>
/// บริการส่งออกรายงาน CSV สำหรับ HR (สรุปลาและการลงเวลา)
/// </summary>
public interface IReportService
{
    /// <summary>ส่งออกสรุปยอดวันลาตามปีเป็น CSV</summary>
    Task<byte[]> ExportLeaveSummaryCsvAsync(int year, CancellationToken cancellationToken = default);

    /// <summary>ส่งออกประวัติลงเวลาในช่วงวันที่เป็น CSV</summary>
    Task<byte[]> ExportAttendanceCsvAsync(string from, string to, Guid? employeeId, CancellationToken cancellationToken = default);
}
