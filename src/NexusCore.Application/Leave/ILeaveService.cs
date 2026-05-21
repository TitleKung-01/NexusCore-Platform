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
    Task<IReadOnlyList<LeaveBalanceResponse>> GetBalancesAsync(int? year, Guid? employeeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LeaveCalendarEntryResponse>> GetCalendarAsync(string from, string to, Guid? departmentId, CancellationToken cancellationToken = default);
    Task<ServiceResult<LeaveAttachmentResponse>> UploadAttachmentAsync(Guid leaveRequestId, string fileName, Stream content, CancellationToken cancellationToken = default);
    Task<(Stream Stream, string ContentType, string FileName)?> DownloadAttachmentAsync(Guid attachmentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LeaveAttachmentResponse>> ListAttachmentsAsync(Guid leaveRequestId, CancellationToken cancellationToken = default);
}
