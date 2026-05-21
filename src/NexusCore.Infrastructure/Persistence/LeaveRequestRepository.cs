using Microsoft.EntityFrameworkCore;
using NexusCore.Domain.Entities;
using NexusCore.Domain.Enums;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Infrastructure.Persistence;

public class LeaveRequestRepository(AppDbContext db) : ILeaveRequestRepository
{
    private IQueryable<LeaveRequest> WithIncludes() =>
        db.LeaveRequests
            .AsNoTracking()
            .Include(l => l.LeaveType)
            .Include(l => l.Employee)
            .ThenInclude(e => e!.User);

    public Task<LeaveRequest?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        WithIncludes().FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

    public Task<LeaveRequest?> FindByIdTrackedAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.LeaveRequests
            .Include(l => l.Employee)
            .Include(l => l.LeaveType)
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

    public async Task<IReadOnlyList<LeaveRequest>> GetByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default) =>
        await WithIncludes()
            .Where(l => l.EmployeeId == employeeId)
            .OrderByDescending(l => l.CreatedAtUtc)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<LeaveRequest>> GetPendingForManagerAsync(Guid managerUserId, CancellationToken cancellationToken = default) =>
        await WithIncludes()
            .Where(l => l.Status == LeaveStatus.Pending && l.Employee.ManagerId == managerUserId)
            .OrderBy(l => l.SubmittedAtUtc)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<LeaveRequest>> GetAllPendingAsync(CancellationToken cancellationToken = default) =>
        await WithIncludes()
            .Where(l => l.Status == LeaveStatus.Pending)
            .OrderBy(l => l.SubmittedAtUtc)
            .ToListAsync(cancellationToken);

    public Task<bool> HasOverlappingAsync(Guid employeeId, DateOnly start, DateOnly end, Guid? excludeId, CancellationToken cancellationToken = default)
    {
        var query = db.LeaveRequests
            .Where(l => l.EmployeeId == employeeId)
            .Where(l => l.Status == LeaveStatus.Pending || l.Status == LeaveStatus.Approved)
            .Where(l => l.StartDate <= end && l.EndDate >= start);

        if (excludeId.HasValue)
            query = query.Where(l => l.Id != excludeId.Value);

        return query.AnyAsync(cancellationToken);
    }

    public async Task AddAsync(LeaveRequest request, CancellationToken cancellationToken = default) =>
        await db.LeaveRequests.AddAsync(request, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
