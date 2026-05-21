using Microsoft.EntityFrameworkCore;
using NexusCore.Domain.Entities;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Infrastructure.Persistence;

public class LeaveEntitlementRepository(AppDbContext db) : ILeaveEntitlementRepository
{
    public Task<LeaveEntitlement?> GetAsync(Guid employeeId, Guid leaveTypeId, int year, CancellationToken cancellationToken = default) =>
        db.LeaveEntitlements
            .AsNoTracking()
            .Include(e => e.LeaveType)
            .FirstOrDefaultAsync(e => e.EmployeeId == employeeId && e.LeaveTypeId == leaveTypeId && e.Year == year, cancellationToken);

    public async Task<IReadOnlyList<LeaveEntitlement>> ListForEmployeeAsync(Guid employeeId, int? year, CancellationToken cancellationToken = default)
    {
        var query = db.LeaveEntitlements
            .AsNoTracking()
            .Include(e => e.LeaveType)
            .Where(e => e.EmployeeId == employeeId);

        if (year.HasValue)
            query = query.Where(e => e.Year == year.Value);

        return await query.OrderBy(e => e.Year).ThenBy(e => e.LeaveType.Name).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(LeaveEntitlement entitlement, CancellationToken cancellationToken = default) =>
        await db.LeaveEntitlements.AddAsync(entitlement, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
