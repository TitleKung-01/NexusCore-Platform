using NexusCore.Application.DTOs;

namespace NexusCore.Application.Services;

/// <summary>
/// บริการจัดการบัญชีผู้ใช้ระบบ (CRUD) สำหรับผู้ดูแล
/// </summary>
public interface IUserService
{
    /// <summary>ดึงรายการผู้ใช้ทั้งหมด</summary>
    Task<IReadOnlyList<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>ดึงผู้ใช้ตามรหัส</summary>
    Task<UserResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>สร้างผู้ใช้และโปรไฟล์พนักงานเริ่มต้น</summary>
    Task<UserResponse?> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default);

    /// <summary>อัปเดตบทบาทผู้ใช้</summary>
    Task<UserResponse?> UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken cancellationToken = default);

    /// <summary>ลบผู้ใช้ออกจากระบบ</summary>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
