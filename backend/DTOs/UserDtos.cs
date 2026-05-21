namespace backend.DTOs;

public record UserResponse(Guid Id, string Username, string Role, DateTime CreatedAtUtc);

public record CreateUserRequest(string Username, string Password, string Role = "User");
