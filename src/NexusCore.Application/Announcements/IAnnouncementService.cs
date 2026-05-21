using NexusCore.Application.Common;

namespace NexusCore.Application.Announcements;

public interface IAnnouncementService
{
    Task<IReadOnlyList<AnnouncementResponse>> ListAsync(CancellationToken cancellationToken = default);
    Task<AnnouncementResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ServiceResult<AnnouncementResponse>> CreateAsync(CreateAnnouncementRequest request, CancellationToken cancellationToken = default);
    Task<ServiceResult<AnnouncementResponse>> UpdateAsync(Guid id, UpdateAnnouncementRequest request, CancellationToken cancellationToken = default);
}
