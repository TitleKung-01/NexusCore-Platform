using NexusCore.Application.Common;
using NexusCore.Application.DTOs;

namespace NexusCore.Application.Services;

/// <summary>
/// บริการยืนยันตัวตน: เข้าสู่ระบบและเปลี่ยนรหัสผ่าน
/// </summary>
public interface IAuthService
{
    /// <summary>ตรวจสอบข้อมูลล็อกอินและออก JWT</summary>
    Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

    /// <summary>เปลี่ยนรหัสผ่านของผู้ใช้ที่ล็อกอินอยู่</summary>
    Task<ServiceResult<bool>> ChangePasswordAsync(ChangePasswordRequest request, CancellationToken cancellationToken = default);
}
