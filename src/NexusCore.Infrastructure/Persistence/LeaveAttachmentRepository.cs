using Microsoft.EntityFrameworkCore;
using NexusCore.Domain.Entities;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Infrastructure.Persistence;

public class LeaveAttachmentRepository(AppDbContext db) : ILeaveAttachmentRepository
{
    public Task<LeaveAttachment?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.LeaveAttachments.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task<IReadOnlyList<LeaveAttachment>> ListByLeaveRequestIdAsync(Guid leaveRequestId, CancellationToken cancellationToken = default) =>
        await db.LeaveAttachments
            .AsNoTracking()
            .Where(a => a.LeaveRequestId == leaveRequestId)
            .OrderByDescending(a => a.UploadedAtUtc)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(LeaveAttachment attachment, CancellationToken cancellationToken = default) =>
        await db.LeaveAttachments.AddAsync(attachment, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
