using NexusCore.Domain.Entities;

namespace NexusCore.Domain.Interfaces;

public interface IDepartmentRepository
{
    Task<IReadOnlyList<Department>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Department?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Department?> FindByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task AddAsync(Department department, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
