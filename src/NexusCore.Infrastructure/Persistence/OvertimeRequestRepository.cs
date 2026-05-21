using Microsoft.EntityFrameworkCore;
using NexusCore.Domain.Entities;
using NexusCore.Domain.Enums;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Infrastructure.Persistence;

public class OvertimeRequestRepository(AppDbContext db) : IOvertimeRequestRepository
{
    private IQueryable<OvertimeRequest> WithIncludes() =>
        db.OvertimeRequests
            .AsNoTracking()
            .Include(o => o.Employee)
                .ThenInclude(e => e!.User)
            .Include(o => o.Employee)
                .ThenInclude(e => e!.Department)
            .Include(o => o.Employee)
                .ThenInclude(e => e!.Manager);

    public Task<OvertimeRequest?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        WithIncludes().FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

    public Task<OvertimeRequest?> FindByIdTrackedAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.OvertimeRequests
            .Include(o => o.Employee)
                .ThenInclude(e => e!.User)
            .Include(o => o.Employee)
                .ThenInclude(e => e!.Department)
            .Include(o => o.Employee)
                .ThenInclude(e => e!.Manager)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

    public async Task<IReadOnlyList<OvertimeRequest>> GetByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default) =>
        await WithIncludes()
            .Where(o => o.EmployeeId == employeeId)
            .OrderByDescending(o => o.CreatedAtUtc)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<OvertimeRequest>> GetPendingForManagerAsync(Guid managerUserId, CancellationToken cancellationToken = default) =>
        await WithIncludes()
            .Where(o => o.Status == LeaveStatus.Pending && o.Employee.ManagerId == managerUserId)
            .OrderBy(o => o.SubmittedAtUtc)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<OvertimeRequest>> GetAllPendingAsync(CancellationToken cancellationToken = default) =>
        await WithIncludes()
            .Where(o => o.Status == LeaveStatus.Pending)
            .OrderBy(o => o.SubmittedAtUtc)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<OvertimeRequest>> GetApprovalHistoryAsync(Guid? managerId, CancellationToken cancellationToken = default)
    {
        var query = WithIncludes()
            .Where(o => o.Status == LeaveStatus.Approved || o.Status == LeaveStatus.Rejected);

        if (managerId.HasValue)
            query = query.Where(o => o.Employee.ManagerId == managerId.Value);

        return await query.OrderByDescending(o => o.DecidedAtUtc).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(OvertimeRequest request, CancellationToken cancellationToken = default) =>
        await db.OvertimeRequests.AddAsync(request, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
