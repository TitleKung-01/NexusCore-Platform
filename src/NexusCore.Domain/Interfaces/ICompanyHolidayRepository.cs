using NexusCore.Domain.Entities;

namespace NexusCore.Domain.Interfaces;

/// <summary>
/// สัญญาเข้าถึงวันหยุดบริษัท — HR จัดการปฏิทินองค์กร
/// </summary>
public interface ICompanyHolidayRepository
{
    /// <summary>รายการวันหยุด (กรองตามปีได้)</summary>
    Task<IReadOnlyList<CompanyHoliday>> ListAsync(int? year, CancellationToken cancellationToken = default);
    /// <summary>ค้นหาวันหยุดตามรหัส (อ่านอย่างเดียว)</summary>
    Task<CompanyHoliday?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    /// <summary>ค้นหาวันหยุดพร้อมติดตามการแก้ไข</summary>
    Task<CompanyHoliday?> FindByIdTrackedAsync(Guid id, CancellationToken cancellationToken = default);
    /// <summary>เพิ่มวันหยุดใหม่</summary>
    Task AddAsync(CompanyHoliday holiday, CancellationToken cancellationToken = default);
    /// <summary>ลบวันหยุดออกจาก context (บันทึกเมื่อ SaveChanges)</summary>
    void Remove(CompanyHoliday holiday);
    /// <summary>บันทึกการเปลี่ยนแปลงลงฐานข้อมูล</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
