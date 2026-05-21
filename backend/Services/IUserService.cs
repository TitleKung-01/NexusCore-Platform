using backend.DTOs;

namespace backend.Services;

public interface IUserService
{
    Task<IReadOnlyList<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UserResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserResponse?> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
}
