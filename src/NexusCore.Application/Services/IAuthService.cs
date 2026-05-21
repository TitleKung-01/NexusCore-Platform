using NexusCore.Application.DTOs;

namespace NexusCore.Application.Services;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
