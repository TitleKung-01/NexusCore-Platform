using NexusCore.Application.Common;
using NexusCore.Application.Notifications;
using NexusCore.Domain.Constants;
using NexusCore.Domain.Entities;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Application.Announcements;

/// <summary>
/// ประกาศข่าวสารและแจ้งเตือนพนักงานทั้งหมด
/// </summary>
public class AnnouncementService(
    ICurrentUserService currentUser,
    IAnnouncementRepository announcements,
    IEmployeeProfileRepository profiles,
    INotificationService notifications) : IAnnouncementService
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<AnnouncementResponse>> ListAsync(CancellationToken cancellationToken = default)
    {
        var list = await announcements.ListActiveAsync(cancellationToken);
        return list.Select(Map).ToList();
    }

    /// <inheritdoc />
    public async Task<AnnouncementResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await announcements.FindByIdAsync(id, cancellationToken);
        return item is null ? null : Map(item);
    }

    /// <inheritdoc />
    public async Task<ServiceResult<AnnouncementResponse>> CreateAsync(CreateAnnouncementRequest request, CancellationToken cancellationToken = default)
    {
        if (!currentUser.IsInAnyRole(UserRoles.Hr, UserRoles.Admin))
            return ServiceResult<AnnouncementResponse>.Fail("Forbidden.", 403);
        if (currentUser.UserId is null)
            return ServiceResult<AnnouncementResponse>.Fail("Unauthorized.", 401);

        var entity = new Announcement
        {
            Id = Guid.NewGuid(),
            Title = request.Title.Trim(),
            Body = request.Body.Trim(),
            AuthorId = currentUser.UserId.Value,
            PublishedAtUtc = DateTime.UtcNow,
            IsActive = true
        };

        await announcements.AddAsync(entity, cancellationToken);
        await announcements.SaveChangesAsync(cancellationToken);

        await PublishToAllEmployeesAsync(entity, cancellationToken);
        return ServiceResult<AnnouncementResponse>.Ok(Map(entity));
    }

    /// <inheritdoc />
    public async Task<ServiceResult<AnnouncementResponse>> UpdateAsync(Guid id, UpdateAnnouncementRequest request, CancellationToken cancellationToken = default)
    {
        if (!currentUser.IsInAnyRole(UserRoles.Hr, UserRoles.Admin))
            return ServiceResult<AnnouncementResponse>.Fail("Forbidden.", 403);

        var entity = await announcements.FindByIdTrackedAsync(id, cancellationToken);
        if (entity is null)
            return ServiceResult<AnnouncementResponse>.Fail("Announcement not found.", 404);

        entity.Title = request.Title.Trim();
        entity.Body = request.Body.Trim();
        entity.IsActive = request.IsActive;
        await announcements.SaveChangesAsync(cancellationToken);
        return ServiceResult<AnnouncementResponse>.Ok(Map(entity));
    }

    private async Task PublishToAllEmployeesAsync(Announcement entity, CancellationToken cancellationToken)
    {
        var employees = await profiles.GetAllWithDetailsAsync(cancellationToken);
        foreach (var emp in employees)
        {
            await notifications.NotifyUserAsync(emp.UserId, "announcement.published", entity.Title, entity.Body,
                "/announcements", cancellationToken);
        }
    }

    private static AnnouncementResponse Map(Announcement a) =>
        new(a.Id, a.Title, a.Body, a.AuthorId, a.PublishedAtUtc.ToString("o"), a.IsActive);
}