namespace NexusCore.Application.Reports;

public interface IReportService
{
    Task<byte[]> ExportLeaveSummaryCsvAsync(int year, CancellationToken cancellationToken = default);
}
