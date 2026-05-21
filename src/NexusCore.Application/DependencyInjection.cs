using Microsoft.Extensions.DependencyInjection;
using NexusCore.Application.Announcements;
using NexusCore.Application.Attendance;
using NexusCore.Application.Employees;
using NexusCore.Application.Expenses;
using NexusCore.Application.Holidays;
using NexusCore.Application.Leave;
using NexusCore.Application.Notifications;
using NexusCore.Application.Organization;
using NexusCore.Application.Onboarding;
using NexusCore.Application.Overtime;
using NexusCore.Application.Payslips;
using NexusCore.Application.Reports;
using NexusCore.Application.Services;
using NexusCore.Application.Transfers;

namespace NexusCore.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<ILeaveService, LeaveService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IHolidayService, HolidayService>();
        services.AddScoped<IAttendanceService, AttendanceService>();
        services.AddScoped<IOvertimeService, OvertimeService>();
        services.AddScoped<IPayslipService, PayslipService>();
        services.AddScoped<ITransferService, TransferService>();
        services.AddScoped<IAnnouncementService, AnnouncementService>();
        services.AddScoped<IOnboardingService, OnboardingService>();
        services.AddScoped<IOrganizationService, OrganizationService>();
        services.AddScoped<IExpenseService, ExpenseService>();
        return services;
    }
}
