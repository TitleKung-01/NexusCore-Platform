using NexusCore.Domain.Entities;

namespace NexusCore.Domain.Interfaces;

/// <summary>
/// สัญญาเข้าถึงนิยามบทบาท — กำหนดสิทธิ์อนุมัติและการเข้าถึง HR แบบปรับได้
/// </summary>
public interface IRoleDefinitionRepository
{
    /// <summary>รายการบทบาททั้งหมดในระบบ</summary>
    Task<IReadOnlyList<RoleDefinition>> GetAllAsync(CancellationToken cancellationToken = default);
    /// <summary>ค้นหาบทบาทตามชื่อ</summary>
    Task<RoleDefinition?> FindByNameAsync(string name, CancellationToken cancellationToken = default);
    /// <summary>เพิ่มนิยามบทบาทใหม่</summary>
    Task AddAsync(RoleDefinition role, CancellationToken cancellationToken = default);
    /// <summary>บันทึกการเปลี่ยนแปลงลงฐานข้อมูล</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
