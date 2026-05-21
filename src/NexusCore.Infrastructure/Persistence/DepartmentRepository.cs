using Microsoft.EntityFrameworkCore;
using NexusCore.Domain.Entities;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Infrastructure.Persistence;

public class DepartmentRepository(AppDbContext db) : IDepartmentRepository
{
    public async Task<IReadOnlyList<Department>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await db.Departments.AsNoTracking().OrderBy(d => d.Name).ToListAsync(cancellationToken);

    public Task<Department?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.Departments.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
}
