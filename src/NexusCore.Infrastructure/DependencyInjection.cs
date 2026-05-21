using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NexusCore.Application.Services;
using NexusCore.Domain.Interfaces;
using NexusCore.Infrastructure.Identity;
using NexusCore.Infrastructure.Persistence;

namespace NexusCore.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Host=localhost;Port=5432;Database=nexuscore;Username=nexuscore;Password=nexuscore_dev";

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();

        return services;
    }
}
