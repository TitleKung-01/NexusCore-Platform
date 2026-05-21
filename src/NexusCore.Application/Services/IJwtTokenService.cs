using NexusCore.Domain.Entities;

namespace NexusCore.Application.Services;

public interface IJwtTokenService
{
    string CreateToken(User user);
}
