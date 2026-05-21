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

    public Task<Department?> FindByCodeAsync(string code, CancellationToken cancellationToken = default) =>
        db.Departments.AsNoTracking()
            .FirstOrDefaultAsync(d => d.Code.ToLower() == code.ToLower(), cancellationToken);

    public async Task AddAsync(Department department, CancellationToken cancellationToken = default) =>
        await db.Departments.AddAsync(department, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
