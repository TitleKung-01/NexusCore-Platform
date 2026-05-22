using Microsoft.EntityFrameworkCore;
using NexusCore.Domain.Entities;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Infrastructure.Persistence;

/// <summary>เข้าถึงไฟล์แนบคำขอลา (LeaveAttachments)</summary>
public class LeaveAttachmentRepository(AppDbContext db) : ILeaveAttachmentRepository
{
    /// <summary>ค้นหาไฟล์แนบจากรหัส</summary>
    public Task<LeaveAttachment?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.LeaveAttachments.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    /// <summary>ดึงไฟล์แนบทั้งหมดของคำขอลาหนึ่งรายการ</summary>
    public async Task<IReadOnlyList<LeaveAttachment>> ListByLeaveRequestIdAsync(Guid leaveRequestId, CancellationToken cancellationToken = default) =>
        await db.LeaveAttachments
            .AsNoTracking()
            .Where(a => a.LeaveRequestId == leaveRequestId)
            .OrderByDescending(a => a.UploadedAtUtc)
            .ToListAsync(cancellationToken);

    /// <summary>เพิ่มไฟล์แนบใหม่</summary>
    public async Task AddAsync(LeaveAttachment attachment, CancellationToken cancellationToken = default) =>
        await db.LeaveAttachments.AddAsync(attachment, cancellationToken);

    /// <summary>บันทึกการเปลี่ยนแปลงลงฐานข้อมูล</summary>
    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
