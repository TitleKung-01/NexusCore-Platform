using backend.Data;
using backend.DTOs;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class UserService(AppDbContext db) : IUserService
{
    public async Task<IReadOnlyList<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await db.Users
            .AsNoTracking()
            .OrderBy(u => u.Username)
            .Select(u => new UserResponse(u.Id, u.Username, u.Role, u.CreatedAtUtc))
            .ToListAsync(cancellationToken);
    }

    public async Task<UserResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        return user is null ? null : new UserResponse(user.Id, user.Username, user.Role, user.CreatedAtUtc);
    }

    public async Task<UserResponse?> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        var exists = await db.Users.AnyAsync(u => u.Username == request.Username, cancellationToken);
        if (exists)
            return null;

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = request.Role,
            CreatedAtUtc = DateTime.UtcNow
        };

        db.Users.Add(user);
        await db.SaveChangesAsync(cancellationToken);

        return new UserResponse(user.Id, user.Username, user.Role, user.CreatedAtUtc);
    }
}
