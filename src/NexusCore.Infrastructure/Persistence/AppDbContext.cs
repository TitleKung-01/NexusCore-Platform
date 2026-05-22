using Microsoft.EntityFrameworkCore;

using NexusCore.Domain.Entities;

using NexusCore.Domain.Enums;



namespace NexusCore.Infrastructure.Persistence;

/// <summary>DbContext หลักของระบบ HR — แมป entity ทั้งหมดและกฎความสัมพันธ์/ดัชนี</summary>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)

{

    public DbSet<User> Users => Set<User>();

    public DbSet<Department> Departments => Set<Department>();

    public DbSet<RoleDefinition> RoleDefinitions => Set<RoleDefinition>();

    public DbSet<LeaveType> LeaveTypes => Set<LeaveType>();

    public DbSet<EmployeeProfile> EmployeeProfiles => Set<EmployeeProfile>();

    public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();

    public DbSet<AppNotification> AppNotifications => Set<AppNotification>();

    public DbSet<LeaveEntitlement> LeaveEntitlements => Set<LeaveEntitlement>();

    public DbSet<LeaveAttachment> LeaveAttachments => Set<LeaveAttachment>();

    public DbSet<CompanyHoliday> CompanyHolidays => Set<CompanyHoliday>();

    public DbSet<AttendanceRecord> AttendanceRecords => Set<AttendanceRecord>();

    public DbSet<OvertimeRequest> OvertimeRequests => Set<OvertimeRequest>();

    public DbSet<Payslip> Payslips => Set<Payslip>();

    public DbSet<EmployeeTransfer> EmployeeTransfers => Set<EmployeeTransfer>();

    public DbSet<Announcement> Announcements => Set<Announcement>();

    public DbSet<OnboardingTemplate> OnboardingTemplates => Set<OnboardingTemplate>();

    public DbSet<OnboardingTemplateTask> OnboardingTemplateTasks => Set<OnboardingTemplateTask>();

    public DbSet<EmployeeOnboardingTask> EmployeeOnboardingTasks => Set<EmployeeOnboardingTask>();

    public DbSet<ExpenseClaim> ExpenseClaims => Set<ExpenseClaim>();

    public DbSet<ExpenseLineItem> ExpenseLineItems => Set<ExpenseLineItem>();

    public DbSet<ExpenseAttachment> ExpenseAttachments => Set<ExpenseAttachment>();



    /// <summary>กำหนดคีย์ ดัชนี ความยาวฟิลด์ และความสัมพันธ์ระหว่างตาราง</summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)

    {

        modelBuilder.Entity<User>(entity =>

        {

            entity.HasKey(u => u.Id);

            entity.HasIndex(u => u.Username).IsUnique();

            entity.Property(u => u.Username).HasMaxLength(64);

            entity.Property(u => u.Role).HasMaxLength(32);

            entity.Property(u => u.IsActive).HasDefaultValue(true);

        });



        modelBuilder.Entity<Department>(entity =>

        {

            entity.HasKey(d => d.Id);

            entity.HasIndex(d => d.Code).IsUnique();

            entity.Property(d => d.Name).HasMaxLength(128);

            entity.Property(d => d.Code).HasMaxLength(32);

        });



        modelBuilder.Entity<RoleDefinition>(entity =>

        {

            entity.HasKey(r => r.Id);

            entity.HasIndex(r => r.Name).IsUnique();

            entity.Property(r => r.Name).HasMaxLength(64);

            entity.Property(r => r.Description).HasMaxLength(256);

        });



        modelBuilder.Entity<LeaveType>(entity =>

        {

            entity.HasKey(l => l.Id);

            entity.HasIndex(l => l.Code).IsUnique();

            entity.Property(l => l.Name).HasMaxLength(128);

            entity.Property(l => l.Code).HasMaxLength(32);

        });



        modelBuilder.Entity<EmployeeProfile>(entity =>

        {

            entity.HasKey(e => e.UserId);

            entity.Property(e => e.FullName).HasMaxLength(128);

            entity.Property(e => e.Email).HasMaxLength(256);

            entity.Property(e => e.Phone).HasMaxLength(32);



            entity.HasOne(e => e.User)

                .WithOne(u => u.Profile)

                .HasForeignKey<EmployeeProfile>(e => e.UserId)

                .OnDelete(DeleteBehavior.Cascade);



            entity.HasOne(e => e.Department)

                .WithMany()

                .HasForeignKey(e => e.DepartmentId)

                .OnDelete(DeleteBehavior.Restrict);



            entity.HasOne(e => e.Manager)

                .WithMany()

                .HasForeignKey(e => e.ManagerId)

                .OnDelete(DeleteBehavior.SetNull);

        });



        modelBuilder.Entity<LeaveRequest>(entity =>

        {

            entity.HasKey(l => l.Id);

            entity.Property(l => l.Reason).HasMaxLength(1000);

            entity.Property(l => l.ManagerComment).HasMaxLength(1000);

            entity.Property(l => l.Status).HasConversion<string>().HasMaxLength(32);



            entity.HasOne(l => l.Employee)

                .WithMany()

                .HasForeignKey(l => l.EmployeeId)

                .HasPrincipalKey(e => e.UserId)

                .OnDelete(DeleteBehavior.Cascade);



            entity.HasOne(l => l.LeaveType)

                .WithMany()

                .HasForeignKey(l => l.LeaveTypeId)

                .OnDelete(DeleteBehavior.Restrict);

        });



        modelBuilder.Entity<AppNotification>(entity =>

        {

            entity.HasKey(n => n.Id);

            entity.Property(n => n.EventType).HasMaxLength(64);

            entity.Property(n => n.Title).HasMaxLength(256);

            entity.Property(n => n.Body).HasMaxLength(2000);

            entity.Property(n => n.LinkPath).HasMaxLength(256);

            entity.HasIndex(n => new { n.UserId, n.IsRead });

        });



        modelBuilder.Entity<LeaveEntitlement>(entity =>

        {

            entity.HasKey(e => e.Id);

            entity.HasIndex(e => new { e.EmployeeId, e.LeaveTypeId, e.Year }).IsUnique();

            entity.Property(e => e.DaysAllowed).HasPrecision(5, 1);

        });



        modelBuilder.Entity<LeaveAttachment>(entity =>

        {

            entity.HasKey(a => a.Id);

            entity.Property(a => a.FileName).HasMaxLength(256);

            entity.Property(a => a.StoragePath).HasMaxLength(512);

            entity.Property(a => a.ContentType).HasMaxLength(128);

        });



        modelBuilder.Entity<CompanyHoliday>(entity =>

        {

            entity.HasKey(h => h.Id);

            entity.HasIndex(h => h.Date).IsUnique();

            entity.Property(h => h.Name).HasMaxLength(128);

        });



        modelBuilder.Entity<AttendanceRecord>(entity =>

        {

            entity.HasKey(a => a.Id);

            entity.HasIndex(a => new { a.EmployeeId, a.WorkDate }).IsUnique();

            entity.Property(a => a.WorkSummary).HasMaxLength(2000);

        });



        modelBuilder.Entity<OvertimeRequest>(entity =>

        {

            entity.HasKey(o => o.Id);

            entity.Property(o => o.Reason).HasMaxLength(1000);

            entity.Property(o => o.ManagerComment).HasMaxLength(1000);

            entity.Property(o => o.Status).HasConversion<string>().HasMaxLength(32);

            entity.Property(o => o.Hours).HasPrecision(5, 1);

        });



        modelBuilder.Entity<Payslip>(entity =>

        {

            entity.HasKey(p => p.Id);

            entity.HasIndex(p => new { p.EmployeeId, p.Year, p.Month }).IsUnique();

            entity.Property(p => p.FileName).HasMaxLength(256);

            entity.Property(p => p.StoragePath).HasMaxLength(512);

        });



        modelBuilder.Entity<EmployeeTransfer>(entity =>

        {

            entity.HasKey(t => t.Id);

            entity.Property(t => t.Note).HasMaxLength(500);

        });



        modelBuilder.Entity<Announcement>(entity =>

        {

            entity.HasKey(a => a.Id);

            entity.Property(a => a.Title).HasMaxLength(256);

            entity.Property(a => a.Body).HasMaxLength(4000);

        });



        modelBuilder.Entity<OnboardingTemplate>(entity =>

        {

            entity.HasKey(t => t.Id);

            entity.Property(t => t.Name).HasMaxLength(128);

        });



        modelBuilder.Entity<OnboardingTemplateTask>(entity =>

        {

            entity.HasKey(t => t.Id);

            entity.Property(t => t.Title).HasMaxLength(256);

        });



        modelBuilder.Entity<EmployeeOnboardingTask>(entity =>

        {

            entity.HasKey(t => t.Id);

            entity.Property(t => t.Title).HasMaxLength(256);

        });



        modelBuilder.Entity<ExpenseClaim>(entity =>

        {

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Title).HasMaxLength(256);

            entity.Property(e => e.ManagerComment).HasMaxLength(1000);

            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(32);

            entity.Property(e => e.TotalAmount).HasPrecision(12, 2);

        });



        modelBuilder.Entity<ExpenseLineItem>(entity =>

        {

            entity.HasKey(l => l.Id);

            entity.Property(l => l.Description).HasMaxLength(500);

            entity.Property(l => l.Amount).HasPrecision(12, 2);

        });



        modelBuilder.Entity<ExpenseAttachment>(entity =>

        {

            entity.HasKey(a => a.Id);

            entity.Property(a => a.FileName).HasMaxLength(256);

            entity.Property(a => a.StoragePath).HasMaxLength(512);

        });

    }

}


