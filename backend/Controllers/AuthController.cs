using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // โมเดลรับข้อมูล Login
    public class LoginRequest { public string Username { get; set; } = ""; public string Password { get; set; } = ""; }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // จำลองการเช็ก User (เดี๋ยวสเต็ปถัดๆ ไปเราค่อยต่อ DB จริงงับ)
        if (request.Username == "admin" && request.Password == "password123")
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

            // สลักชื่อและสิทธิ์ (Role) ลงบนตั๋ว
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, request.Username),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(2), // ตั๋วหมดอายุใน 2 ชั่วโมง
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { Token = tokenString, Message = "เข้าสู่ระบบสำเร็จงับ!" });
        }

        return Unauthorized(new { Message = "Username หรือ Password ไม่ถูกต้อง!" });
    }

    // Endpoint ลับ ลองของ! คนมีตั๋ว VIP เท่านั้นที่เข้าได้
    [HttpGet("secret-data")]
    [Microsoft.AspNetCore.Authorization.Authorize] // ล็อกไว้ด้วยกำแพงความปลอดภัย
    public IActionResult GetSecretData()
    {
        return Ok(new { Data = "นี่คือข้อมูลลับระดับองค์กรที่คนมีตั๋ว JWT เท่านั้นที่มองเห็น! 🤫" });
    }
}