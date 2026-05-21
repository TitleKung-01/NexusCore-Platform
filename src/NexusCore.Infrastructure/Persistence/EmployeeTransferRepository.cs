using Microsoft.EntityFrameworkCore;
using NexusCore.Domain.Entities;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Infrastructure.Persistence;

public class EmployeeTransferRepository(AppDbContext db) : IEmployeeTransferRepository
{
    public async Task<IReadOnlyList<EmployeeTransfer>> ListAsync(Guid? employeeId, CancellationToken cancellationToken = default)
    {
        var query = db.EmployeeTransfers
            .AsNoTracking()
            .Include(t => t.FromDepartment)
            .Include(t => t.ToDepartment)
            .Include(t => t.Employee)
            .AsQueryable();

        if (employeeId.HasValue)
            query = query.Where(t => t.EmployeeId == employeeId.Value);

        return await query.OrderByDescending(t => t.CreatedAtUtc).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(EmployeeTransfer transfer, CancellationToken cancellationToken = default) =>
        await db.EmployeeTransfers.AddAsync(transfer, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
