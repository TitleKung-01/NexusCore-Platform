using Microsoft.EntityFrameworkCore;
using NexusCore.Domain.Entities;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Infrastructure.Persistence;

public class LeaveTypeRepository(AppDbContext db) : ILeaveTypeRepository
{
    public async Task<IReadOnlyList<LeaveType>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await db.LeaveTypes.AsNoTracking().OrderBy(l => l.Name).ToListAsync(cancellationToken);

    public Task<LeaveType?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.LeaveTypes.AsNoTracking().FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
}
