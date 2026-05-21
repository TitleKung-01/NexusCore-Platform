using NexusCore.Domain.Entities;

namespace NexusCore.Domain.Interfaces;

public interface IRoleDefinitionRepository
{
    Task<IReadOnlyList<RoleDefinition>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<RoleDefinition?> FindByNameAsync(string name, CancellationToken cancellationToken = default);
    Task AddAsync(RoleDefinition role, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
