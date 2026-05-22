using Microsoft.EntityFrameworkCore;
using NexusCore.Domain.Entities;
using NexusCore.Domain.Enums;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Infrastructure.Persistence;

/// <summary>เข้าถึงคำขอทำงานล่วงเวลา (OvertimeRequests)</summary>
public class OvertimeRequestRepository(AppDbContext db) : IOvertimeRequestRepository
{
    /// <summary>Query พื้นฐานพร้อม Include ที่จำเป็น</summary>
    private IQueryable<OvertimeRequest> WithIncludes() =>
        db.OvertimeRequests
            .AsNoTracking()
            .Include(o => o.Employee)
                .ThenInclude(e => e!.User)
            .Include(o => o.Employee)
                .ThenInclude(e => e!.Department)
            .Include(o => o.Employee)
                .ThenInclude(e => e!.Manager);

    /// <summary>ค้นหาคำขอ OT จากรหัส (อ่านอย่างเดียว)</summary>
    public Task<OvertimeRequest?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        WithIncludes().FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

    /// <summary>ค้นหาคำขอ OT จากรหัส (ติดตามการเปลี่ยนแปลง)</summary>
    public Task<OvertimeRequest?> FindByIdTrackedAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.OvertimeRequests
            .Include(o => o.Employee)
                .ThenInclude(e => e!.User)
            .Include(o => o.Employee)
                .ThenInclude(e => e!.Department)
            .Include(o => o.Employee)
                .ThenInclude(e => e!.Manager)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

    /// <summary>ดึงคำขอ OT ของพนักงานคนหนึ่ง</summary>
    public async Task<IReadOnlyList<OvertimeRequest>> GetByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default) =>
        await WithIncludes()
            .Where(o => o.EmployeeId == employeeId)
            .OrderByDescending(o => o.CreatedAtUtc)
            .ToListAsync(cancellationToken);

    /// <summary>ดึงคำขอ OT ที่รออนุมัติของทีมภายใต้หัวหน้าคนนี้</summary>
    public async Task<IReadOnlyList<OvertimeRequest>> GetPendingForManagerAsync(Guid managerUserId, CancellationToken cancellationToken = default) =>
        await WithIncludes()
            .Where(o => o.Status == LeaveStatus.Pending && o.Employee.ManagerId == managerUserId)
            .OrderBy(o => o.SubmittedAtUtc)
            .ToListAsync(cancellationToken);

    /// <summary>ดึงคำขอ OT ที่รออนุมัติทั้งองค์กร</summary>
    public async Task<IReadOnlyList<OvertimeRequest>> GetAllPendingAsync(CancellationToken cancellationToken = default) =>
        await WithIncludes()
            .Where(o => o.Status == LeaveStatus.Pending)
            .OrderBy(o => o.SubmittedAtUtc)
            .ToListAsync(cancellationToken);

    /// <summary>ดึงประวัติการอนุมัติ/ปฏิเสธคำขอ OT</summary>
    public async Task<IReadOnlyList<OvertimeRequest>> GetApprovalHistoryAsync(Guid? managerId, CancellationToken cancellationToken = default)
    {
        var query = WithIncludes()
            .Where(o => o.Status == LeaveStatus.Approved || o.Status == LeaveStatus.Rejected);

        if (managerId.HasValue)
            query = query.Where(o => o.Employee.ManagerId == managerId.Value);

        return await query.OrderByDescending(o => o.DecidedAtUtc).ToListAsync(cancellationToken);
    }

    /// <summary>เพิ่มคำขอ OT ใหม่</summary>
    public async Task AddAsync(OvertimeRequest request, CancellationToken cancellationToken = default) =>
        await db.OvertimeRequests.AddAsync(request, cancellationToken);

    /// <summary>บันทึกการเปลี่ยนแปลงลงฐานข้อมูล</summary>
    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
