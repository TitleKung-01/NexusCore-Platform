using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NexusCore.Application.Common;
using NexusCore.Application.Notifications;
using NexusCore.Application.Services;
using NexusCore.Domain.Interfaces;
using NexusCore.Infrastructure.Identity;
using NexusCore.Infrastructure.Notifications;
using NexusCore.Infrastructure.Persistence;
using NexusCore.Infrastructure.Storage;

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
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<ILeaveTypeRepository, LeaveTypeRepository>();
        services.AddScoped<IEmployeeProfileRepository, EmployeeProfileRepository>();
        services.AddScoped<ILeaveRequestRepository, LeaveRequestRepository>();
        services.AddScoped<ILeaveEntitlementRepository, LeaveEntitlementRepository>();
        services.AddScoped<ILeaveAttachmentRepository, LeaveAttachmentRepository>();
        services.AddScoped<ICompanyHolidayRepository, CompanyHolidayRepository>();
        services.AddScoped<IAttendanceRepository, AttendanceRepository>();
        services.AddScoped<IOvertimeRequestRepository, OvertimeRequestRepository>();
        services.AddScoped<IPayslipRepository, PayslipRepository>();
        services.AddScoped<IEmployeeTransferRepository, EmployeeTransferRepository>();
        services.AddScoped<IAnnouncementRepository, AnnouncementRepository>();
        services.AddScoped<IOnboardingRepository, OnboardingRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<IExpenseRepository, ExpenseRepository>();
        services.AddScoped<IAppNotificationRepository, AppNotificationRepository>();

        services.Configure<N8nOptions>(configuration.GetSection(N8nOptions.SectionName));
        services.Configure<FileStorageOptions>(configuration.GetSection(FileStorageOptions.SectionName));
        services.AddHttpClient("n8n");
        services.AddScoped<INotificationPublisher, N8nWebhookNotificationPublisher>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddSingleton<IFileStorage, LocalFileStorage>();

        return services;
    }
}
