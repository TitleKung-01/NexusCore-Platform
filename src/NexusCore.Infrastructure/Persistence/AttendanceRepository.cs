using Microsoft.EntityFrameworkCore;
using NexusCore.Domain.Entities;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Infrastructure.Persistence;

public class AttendanceRepository(AppDbContext db) : IAttendanceRepository
{
    public Task<AttendanceRecord?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.AttendanceRecords
            .AsNoTracking()
            .Include(a => a.Employee)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public Task<AttendanceRecord?> FindByEmployeeAndDateTrackedAsync(Guid employeeId, DateOnly workDate, CancellationToken cancellationToken = default) =>
        db.AttendanceRecords.FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.WorkDate == workDate, cancellationToken);

    public async Task<IReadOnlyList<AttendanceRecord>> ListAsync(Guid? employeeId, DateOnly from, DateOnly to, CancellationToken cancellationToken = default)
    {
        var query = db.AttendanceRecords
            .AsNoTracking()
            .Include(a => a.Employee)
            .Where(a => a.WorkDate >= from && a.WorkDate <= to);

        if (employeeId.HasValue)
            query = query.Where(a => a.EmployeeId == employeeId.Value);

        return await query.OrderBy(a => a.WorkDate).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(AttendanceRecord record, CancellationToken cancellationToken = default) =>
        await db.AttendanceRecords.AddAsync(record, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
