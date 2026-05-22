using NexusCore.Domain.Entities;

namespace NexusCore.Application.Services;

/// <summary>
/// ออกโทเคน JWT สำหรับผู้ใช้ที่ยืนยันตัวตนแล้ว
/// </summary>
public interface IJwtTokenService
{
    /// <summary>สร้าง JWT จากข้อมูลผู้ใช้และบทบาท</summary>
    string CreateToken(User user);
}
