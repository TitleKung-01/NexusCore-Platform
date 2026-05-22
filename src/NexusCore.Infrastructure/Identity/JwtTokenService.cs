using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NexusCore.Application.Services;
using NexusCore.Domain.Entities;

namespace NexusCore.Infrastructure.Identity;

/// <summary>สร้าง JWT สำหรับผู้ใช้ที่เข้าสู่ระบบ</summary>
public class JwtTokenService(IConfiguration configuration) : IJwtTokenService
{
    /// <summary>ออกโทเค็น JWT พร้อม claims รหัสผู้ใช้ ชื่อผู้ใช้ และบทบาท</summary>
    public string CreateToken(User user)
    {
        var jwtSettings = configuration.GetSection("Jwt");
        var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(2),
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"],
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var handler = new JwtSecurityTokenHandler();
        return handler.WriteToken(handler.CreateToken(descriptor));
    }
}
