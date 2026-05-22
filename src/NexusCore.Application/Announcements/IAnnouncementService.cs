using NexusCore.Application.Common;

namespace NexusCore.Application.Announcements;

/// <summary>
/// บริการประกาศข่าวสารภายใน: แสดง สร้าง และแก้ไข (HR) พร้อมแจ้งเตือนพนักงาน
/// </summary>
public interface IAnnouncementService
{
    /// <summary>รายการประกาศที่เปิดใช้งาน</summary>
    Task<IReadOnlyList<AnnouncementResponse>> ListAsync(CancellationToken cancellationToken = default);

    /// <summary>ดึงประกาศตามรหัส</summary>
    Task<AnnouncementResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>สร้างประกาศและแจ้งเตือนทุกคน</summary>
    Task<ServiceResult<AnnouncementResponse>> CreateAsync(CreateAnnouncementRequest request, CancellationToken cancellationToken = default);

    /// <summary>แก้ไขประกาศและสถานะ</summary>
    Task<ServiceResult<AnnouncementResponse>> UpdateAsync(Guid id, UpdateAnnouncementRequest request, CancellationToken cancellationToken = default);
}
