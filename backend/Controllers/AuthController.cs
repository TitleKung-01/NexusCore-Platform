using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
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

    [HttpGet("secret-data")]
    [Authorize]
    [ProducesResponseType(typeof(SecretDataResponse), StatusCodes.Status200OK)]
    public IActionResult GetSecretData()
    {
        return Ok(new SecretDataResponse("นี่คือข้อมูลลับระดับองค์กรที่คนมีตั๋ว JWT เท่านั้นที่มองเห็น"));
    }
}
