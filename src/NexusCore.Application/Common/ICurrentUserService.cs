namespace NexusCore.Application.Common;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? Role { get; }
    bool IsInRole(string role);
    bool IsInAnyRole(params string[] roles);
}
