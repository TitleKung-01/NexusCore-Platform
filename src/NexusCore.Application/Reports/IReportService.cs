namespace NexusCore.Application.Reports;

public interface IReportService
{
    Task<byte[]> ExportLeaveSummaryCsvAsync(int year, CancellationToken cancellationToken = default);
    Task<byte[]> ExportAttendanceCsvAsync(string from, string to, Guid? employeeId, CancellationToken cancellationToken = default);
}
