using Microsoft.EntityFrameworkCore;
using NexusCore.Domain.Entities;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Infrastructure.Persistence;

public class RoleDefinitionRepository(AppDbContext db) : IRoleDefinitionRepository
{
    public async Task<IReadOnlyList<RoleDefinition>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await db.RoleDefinitions.AsNoTracking().OrderBy(r => r.Name).ToListAsync(cancellationToken);

    public Task<RoleDefinition?> FindByNameAsync(string name, CancellationToken cancellationToken = default) =>
        db.RoleDefinitions.AsNoTracking()
            .FirstOrDefaultAsync(r => r.Name.ToLower() == name.ToLower(), cancellationToken);

    public async Task AddAsync(RoleDefinition role, CancellationToken cancellationToken = default) =>
        await db.RoleDefinitions.AddAsync(role, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
