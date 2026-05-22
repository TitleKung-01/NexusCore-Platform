using NexusCore.Application.Common;
using NexusCore.Application.DTOs;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Application.Services;

/// <summary>
/// ดำเนินการเข้าสู่ระบบและเปลี่ยนรหัสผ่านตามนโยบายความปลอดภัย
/// </summary>
public class AuthService(
    IUserRepository users,
    IJwtTokenService jwtTokenService,
    ICurrentUserService currentUser) : IAuthService
{
    /// <inheritdoc />
    public async Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await users.FindByUsernameAsync(request.Username, cancellationToken);

        if (user is null || !user.IsActive || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return null;

        var token = jwtTokenService.CreateToken(user);
        return new LoginResponse(token, "เข้าสู่ระบบสำเร็จ");
    }

    /// <inheritdoc />
    public async Task<ServiceResult<bool>> ChangePasswordAsync(ChangePasswordRequest request, CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is null)
            return ServiceResult<bool>.Fail("Unauthorized.", 401);

        var user = await users.FindByIdTrackedAsync(currentUser.UserId.Value, cancellationToken);
        if (user is null)
            return ServiceResult<bool>.Fail("User not found.", 404);

        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            return ServiceResult<bool>.Fail("Current password is incorrect.", 400);

        if (request.NewPassword.Length < 8)
            return ServiceResult<bool>.Fail("New password must be at least 8 characters.", 400);

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        await users.SaveChangesAsync(cancellationToken);
        return ServiceResult<bool>.Ok(true);
    }
}
