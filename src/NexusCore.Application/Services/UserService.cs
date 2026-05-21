using NexusCore.Application.DTOs;
using NexusCore.Domain.Constants;
using NexusCore.Domain.Entities;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Application.Services;

public class UserService(
    IUserRepository users,
    IEmployeeProfileRepository profiles,
    IDepartmentRepository departments) : IUserService
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

        var deptList = await departments.GetAllAsync(cancellationToken);
        var defaultDept = deptList.FirstOrDefault();
        if (defaultDept is null)
            return null;

        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Username = request.Username.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = request.Role,
            CreatedAtUtc = DateTime.UtcNow
        };

        var profile = new EmployeeProfile
        {
            UserId = userId,
            FullName = string.IsNullOrWhiteSpace(request.FullName) ? request.Username.Trim() : request.FullName.Trim(),
            Email = string.IsNullOrWhiteSpace(request.Email) ? $"{request.Username.Trim()}@nexuscore.local" : request.Email.Trim(),
            DepartmentId = defaultDept.Id
        };

        await users.AddAsync(user, cancellationToken);
        await profiles.AddAsync(profile, cancellationToken);
        await users.SaveChangesAsync(cancellationToken);

        return Map(user);
    }

    public async Task<UserResponse?> UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        var user = await users.FindByIdTrackedAsync(id, cancellationToken);
        if (user is null)
            return null;

        if (!UserRoles.All.Contains(request.Role, StringComparer.OrdinalIgnoreCase))
            return null;

        user.Role = request.Role;
        await users.SaveChangesAsync(cancellationToken);
        return Map(user);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await users.FindByIdTrackedAsync(id, cancellationToken);
        if (user is null)
            return false;

        users.Remove(user);
        await users.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static UserResponse Map(User user) =>
        new(user.Id, user.Username, user.Role, user.CreatedAtUtc);
}
