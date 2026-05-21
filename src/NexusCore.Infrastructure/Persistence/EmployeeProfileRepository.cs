using Microsoft.EntityFrameworkCore;
using NexusCore.Domain.Entities;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Infrastructure.Persistence;

public class EmployeeProfileRepository(AppDbContext db) : IEmployeeProfileRepository
{
    public Task<EmployeeProfile?> FindByUserIdAsync(Guid userId, CancellationToken cancellationToken = default) =>
        db.EmployeeProfiles
            .AsNoTracking()
            .Include(e => e.Department)
            .Include(e => e.User)
            .Include(e => e.Manager)
            .FirstOrDefaultAsync(e => e.UserId == userId, cancellationToken);

    public Task<EmployeeProfile?> FindByUserIdTrackedAsync(Guid userId, CancellationToken cancellationToken = default) =>
        db.EmployeeProfiles
            .Include(e => e.Department)
            .Include(e => e.User)
            .FirstOrDefaultAsync(e => e.UserId == userId, cancellationToken);

    public async Task<IReadOnlyList<EmployeeProfile>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default) =>
        await db.EmployeeProfiles
            .AsNoTracking()
            .Include(e => e.Department)
            .Include(e => e.User)
            .Include(e => e.Manager)
            .OrderBy(e => e.FullName)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(EmployeeProfile profile, CancellationToken cancellationToken = default) =>
        await db.EmployeeProfiles.AddAsync(profile, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
