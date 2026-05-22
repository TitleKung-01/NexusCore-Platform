using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NexusCore.Application.Common;
using NexusCore.Application.Services;
using NexusCore.Domain.Interfaces;
using NexusCore.Infrastructure.Identity;
using NexusCore.Infrastructure.Persistence;
using NexusCore.Infrastructure.Storage;

namespace NexusCore.Infrastructure;

/// <summary>ลงทะเบียนบริการชั้น Infrastructure (ฐานข้อมูล, repository, JWT, ที่เก็บไฟล์)</summary>
public static class DependencyInjection
{
    /// <summary>เพิ่ม DbContext PostgreSQL, repository ทั้งหมด, JWT และ LocalFileStorage</summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Host=localhost;Port=5432;Database=nexuscore;Username=nexuscore;Password=nexuscore_dev";

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<IRoleDefinitionRepository, RoleDefinitionRepository>();
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
        services.AddScoped<IExpenseRepository, ExpenseRepository>();
        services.AddScoped<IAppNotificationRepository, AppNotificationRepository>();

        services.Configure<FileStorageOptions>(configuration.GetSection(FileStorageOptions.SectionName));
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddSingleton<IFileStorage, LocalFileStorage>();

        return services;
    }
}
