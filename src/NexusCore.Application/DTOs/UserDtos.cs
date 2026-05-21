namespace NexusCore.Application.DTOs;

public record UserResponse(Guid Id, string Username, string Role, DateTime CreatedAtUtc);

public record CreateUserRequest(string Username, string Password, string Role = "Employee", string? FullName = null, string? Email = null);

public record UpdateUserRequest(string Role);
