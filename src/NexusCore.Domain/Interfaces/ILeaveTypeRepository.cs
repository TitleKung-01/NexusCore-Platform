using NexusCore.Domain.Entities;

namespace NexusCore.Domain.Interfaces;

public interface ILeaveTypeRepository
{
    Task<IReadOnlyList<LeaveType>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<LeaveType?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
