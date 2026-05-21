using NexusCore.Application.DTOs;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Application.Services;

public class AuthService(IUserRepository users, IJwtTokenService jwtTokenService) : IAuthService
{
    public async Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await users.FindByUsernameAsync(request.Username, cancellationToken);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return null;

        var token = jwtTokenService.CreateToken(user);
        return new LoginResponse(token, "เข้าสู่ระบบสำเร็จ");
    }
}
