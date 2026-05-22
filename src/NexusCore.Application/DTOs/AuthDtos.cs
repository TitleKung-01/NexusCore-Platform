namespace NexusCore.Application.DTOs;

/// <summary>คำขอเข้าสู่ระบบด้วยชื่อผู้ใช้และรหัสผ่าน</summary>
public record LoginRequest(string Username, string Password);

/// <summary>ผลลัพธ์เข้าสู่ระบบพร้อม JWT และข้อความ</summary>
public record LoginResponse(string Token, string Message);

/// <summary>คำขอเปลี่ยนรหัสผ่าน (รหัสเดิมและรหัสใหม่)</summary>
public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
