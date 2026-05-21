using NexusCore.Domain.Entities;

namespace NexusCore.Domain.Interfaces;

public interface ILeaveAttachmentRepository
{
    Task<LeaveAttachment?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LeaveAttachment>> ListByLeaveRequestIdAsync(Guid leaveRequestId, CancellationToken cancellationToken = default);
    Task AddAsync(LeaveAttachment attachment, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
