using Microsoft.EntityFrameworkCore;
using NexusCore.Domain.Entities;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Infrastructure.Persistence;

public class AnnouncementRepository(AppDbContext db) : IAnnouncementRepository
{
    public async Task<IReadOnlyList<Announcement>> ListActiveAsync(CancellationToken cancellationToken = default) =>
        await db.Announcements
            .AsNoTracking()
            .Where(a => a.IsActive)
            .OrderByDescending(a => a.PublishedAtUtc)
            .ToListAsync(cancellationToken);

    public Task<Announcement?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.Announcements.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public Task<Announcement?> FindByIdTrackedAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.Announcements.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task AddAsync(Announcement announcement, CancellationToken cancellationToken = default) =>
        await db.Announcements.AddAsync(announcement, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
