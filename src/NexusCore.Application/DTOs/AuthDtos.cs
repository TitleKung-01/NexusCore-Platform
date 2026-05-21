namespace NexusCore.Application.DTOs;

public record LoginRequest(string Username, string Password);

public record LoginResponse(string Token, string Message);

public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
