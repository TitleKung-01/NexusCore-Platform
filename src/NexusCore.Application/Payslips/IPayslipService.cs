using NexusCore.Application.Common;

namespace NexusCore.Application.Payslips;

public interface IPayslipService
{
    Task<IReadOnlyList<PayslipResponse>> ListAsync(Guid? employeeId, CancellationToken cancellationToken = default);
    Task<(Stream Stream, string ContentType, string FileName)?> DownloadAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ServiceResult<PayslipResponse>> UploadAsync(UploadPayslipRequest request, string fileName, Stream content, CancellationToken cancellationToken = default);
}
