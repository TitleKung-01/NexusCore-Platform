using System.Security.Claims;
using NexusCore.Application.Common;

namespace NexusCore.Api.Services;

/// <summary>อ่านข้อมูลผู้ใช้ที่ล็อกอินจาก JWT claims ใน HttpContext</summary>
public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    /// <summary>รหัสผู้ใช้จาก claim NameIdentifier</summary>
    public Guid? UserId
    {
        get
        {
            var id = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(id, out var guid) ? guid : null;
        }
    }

    /// <summary>บทบาทจาก claim Role</summary>
    public string? Role => User?.FindFirstValue(ClaimTypes.Role);

    /// <summary>ตรวจว่าผู้ใช้อยู่ในบทบาทที่ระบุหรือไม่</summary>
    public bool IsInRole(string role) =>
        string.Equals(Role, role, StringComparison.OrdinalIgnoreCase);

    /// <summary>ตรวจว่าผู้ใช้อยู่ในบทบาทใดบทบาทหนึ่งที่ระบุหรือไม่</summary>
    public bool IsInAnyRole(params string[] roles) =>
        roles.Any(r => IsInRole(r));
}
