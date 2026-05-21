using NexusCore.Application.Common;

namespace NexusCore.Application.Overtime;

public interface IOvertimeService
{
    Task<IReadOnlyList<OvertimeRequestResponse>> ListAsync(string scope, CancellationToken cancellationToken = default);
    Task<OvertimeRequestResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ServiceResult<OvertimeRequestResponse>> CreateAsync(CreateOvertimeRequest request, CancellationToken cancellationToken = default);
    Task<ServiceResult<OvertimeRequestResponse>> SubmitAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ServiceResult<OvertimeRequestResponse>> ApproveAsync(Guid id, DecideOvertimeRequest request, CancellationToken cancellationToken = default);
    Task<ServiceResult<OvertimeRequestResponse>> RejectAsync(Guid id, DecideOvertimeRequest request, CancellationToken cancellationToken = default);
    Task<ServiceResult<OvertimeRequestResponse>> CancelAsync(Guid id, CancellationToken cancellationToken = default);
}
