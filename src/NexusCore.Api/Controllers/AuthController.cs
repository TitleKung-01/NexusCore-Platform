using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusCore.Application.DTOs;
using NexusCore.Application.Services;

namespace NexusCore.Api.Controllers;

/// <summary>กลุ่ม API การยืนยันตัวตน — เข้าสู่ระบบและเปลี่ยนรหัสผ่าน</summary>
[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>เข้าสู่ระบบด้วยชื่อผู้ใช้และรหัสผ่าน คืนค่า JWT</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.LoginAsync(request, cancellationToken);
        if (result is null)
            return Unauthorized(new { Message = "Username หรือ Password ไม่ถูกต้อง" });

        return Ok(result);
    }

    /// <summary>เปลี่ยนรหัสผ่านของผู้ใช้ที่ล็อกอินอยู่</summary>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.ChangePasswordAsync(request, cancellationToken);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { Message = result.Error });
        return NoContent();
    }
}
