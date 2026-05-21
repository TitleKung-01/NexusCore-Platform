using NexusCore.Application.DTOs;

namespace NexusCore.Application.Services;

public interface IUserService
{
    Task<IReadOnlyList<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UserResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserResponse?> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
    Task<UserResponse?> UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
