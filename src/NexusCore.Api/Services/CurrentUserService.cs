using System.Security.Claims;
using NexusCore.Application.Common;

namespace NexusCore.Api.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public Guid? UserId
    {
        get
        {
            var id = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(id, out var guid) ? guid : null;
        }
    }

    public string? Role => User?.FindFirstValue(ClaimTypes.Role);

    public bool IsInRole(string role) =>
        string.Equals(Role, role, StringComparison.OrdinalIgnoreCase);

    public bool IsInAnyRole(params string[] roles) =>
        roles.Any(r => IsInRole(r));
}
