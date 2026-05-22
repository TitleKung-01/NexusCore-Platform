namespace NexusCore.Application.DTOs;

/// <summary>ข้อมูลผู้ใช้ระบบสำหรับแสดงในรายการ/รายละเอียด</summary>
public record UserResponse(Guid Id, string Username, string Role, DateTime CreatedAtUtc);

/// <summary>คำขอสร้างผู้ใช้ใหม่พร้อมโปรไฟล์พนักงานเริ่มต้น</summary>
public record CreateUserRequest(string Username, string Password, string Role = "Employee", string? FullName = null, string? Email = null);

/// <summary>คำขออัปเดตบทบาทผู้ใช้</summary>
public record UpdateUserRequest(string Role);
