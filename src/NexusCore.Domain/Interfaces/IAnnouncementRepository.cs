using NexusCore.Domain.Entities;

namespace NexusCore.Domain.Interfaces;

public interface IAnnouncementRepository
{
    Task<IReadOnlyList<Announcement>> ListActiveAsync(CancellationToken cancellationToken = default);
    Task<Announcement?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Announcement?> FindByIdTrackedAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Announcement announcement, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
