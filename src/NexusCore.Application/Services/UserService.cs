using NexusCore.Application.DTOs;
using NexusCore.Domain.Entities;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Application.Services;

public class UserService(IUserRepository users) : IUserService
{
    public async Task<IReadOnlyList<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var list = await users.GetAllOrderedAsync(cancellationToken);
        return list.Select(Map).ToList();
    }

    public async Task<UserResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await users.FindByIdAsync(id, cancellationToken);
        return user is null ? null : Map(user);
    }

    public async Task<UserResponse?> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        if (await users.UsernameExistsAsync(request.Username, cancellationToken))
            return null;

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = request.Role,
            CreatedAtUtc = DateTime.UtcNow
        };

        await users.AddAsync(user, cancellationToken);
        await users.SaveChangesAsync(cancellationToken);

        return Map(user);
    }

    private static UserResponse Map(User user) =>
        new(user.Id, user.Username, user.Role, user.CreatedAtUtc);
}
