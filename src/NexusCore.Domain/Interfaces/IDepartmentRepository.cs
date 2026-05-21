using NexusCore.Domain.Entities;

namespace NexusCore.Domain.Interfaces;

public interface IDepartmentRepository
{
    Task<IReadOnlyList<Department>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Department?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
