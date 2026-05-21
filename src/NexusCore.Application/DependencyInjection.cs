using Microsoft.Extensions.DependencyInjection;
using NexusCore.Application.Employees;
using NexusCore.Application.Leave;
using NexusCore.Application.Services;

namespace NexusCore.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<ILeaveService, LeaveService>();
        return services;
    }
}
