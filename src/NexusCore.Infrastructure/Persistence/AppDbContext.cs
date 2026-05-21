using Microsoft.EntityFrameworkCore;
using NexusCore.Domain.Entities;
using NexusCore.Domain.Enums;

namespace NexusCore.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<LeaveType> LeaveTypes => Set<LeaveType>();
    public DbSet<EmployeeProfile> EmployeeProfiles => Set<EmployeeProfile>();
    public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Username).IsUnique();
            entity.Property(u => u.Username).HasMaxLength(64);
            entity.Property(u => u.Role).HasMaxLength(32);
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(d => d.Id);
            entity.HasIndex(d => d.Code).IsUnique();
            entity.Property(d => d.Name).HasMaxLength(128);
            entity.Property(d => d.Code).HasMaxLength(32);
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
    }
}
