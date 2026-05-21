using NexusCore.Application.Common;

namespace NexusCore.Application.Leave;

public interface ILeaveService
{
    Task<IReadOnlyList<LeaveRequestResponse>> ListAsync(string scope, CancellationToken cancellationToken = default);
    Task<LeaveRequestResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ServiceResult<LeaveRequestResponse>> CreateAsync(CreateLeaveRequest request, CancellationToken cancellationToken = default);
    Task<ServiceResult<LeaveRequestResponse>> SubmitAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ServiceResult<LeaveRequestResponse>> ApproveAsync(Guid id, DecideLeaveRequest request, CancellationToken cancellationToken = default);
    Task<ServiceResult<LeaveRequestResponse>> RejectAsync(Guid id, DecideLeaveRequest request, CancellationToken cancellationToken = default);
    Task<ServiceResult<LeaveRequestResponse>> CancelAsync(Guid id, CancellationToken cancellationToken = default);
}
