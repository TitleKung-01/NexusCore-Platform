using Microsoft.EntityFrameworkCore;
using NexusCore.Domain.Constants;
using NexusCore.Domain.Entities;

namespace NexusCore.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task InitializeAsync(AppDbContext db)
    {
        await db.Database.MigrateAsync();

        Department engineering;
        Department hrDept;

        if (!await db.Departments.AnyAsync())
        {
            engineering = new Department { Id = Guid.NewGuid(), Name = "Engineering", Code = "ENG" };
            hrDept = new Department { Id = Guid.NewGuid(), Name = "Human Resources", Code = "HR" };
            var sales = new Department { Id = Guid.NewGuid(), Name = "Sales", Code = "SAL" };
            db.Departments.AddRange(engineering, hrDept, sales);

            db.LeaveTypes.AddRange(
                new LeaveType { Id = Guid.NewGuid(), Name = "ลาป่วย", Code = "SICK" },
                new LeaveType { Id = Guid.NewGuid(), Name = "ลากิจ", Code = "PERSONAL" },
                new LeaveType { Id = Guid.NewGuid(), Name = "ลาพักร้อน", Code = "ANNUAL" });

            await db.SaveChangesAsync();
        }
        else
        {
            engineering = await db.Departments.FirstOrDefaultAsync(d => d.Code == "ENG")
                ?? await db.Departments.FirstAsync();
            hrDept = await db.Departments.FirstOrDefaultAsync(d => d.Code == "HR")
                ?? engineering;
        }

        if (!await db.LeaveTypes.AnyAsync())
        {
            db.LeaveTypes.AddRange(
                new LeaveType { Id = Guid.NewGuid(), Name = "ลาป่วย", Code = "SICK" },
                new LeaveType { Id = Guid.NewGuid(), Name = "ลากิจ", Code = "PERSONAL" },
                new LeaveType { Id = Guid.NewGuid(), Name = "ลาพักร้อน", Code = "ANNUAL" });
            await db.SaveChangesAsync();
        }

        var adminUser = await db.Users.FirstOrDefaultAsync(u => u.Username == "admin");
        if (adminUser is not null)
        {
            if (adminUser.Role == "Admin" || adminUser.Role == "User")
                adminUser.Role = UserRoles.Hr;

            if (!await db.EmployeeProfiles.AnyAsync(p => p.UserId == adminUser.Id))
            {
                db.EmployeeProfiles.Add(new EmployeeProfile
                {
                    UserId = adminUser.Id,
                    FullName = "ผู้ดูแล HR",
                    Email = "admin@nexuscore.local",
                    DepartmentId = hrDept.Id
                });
            }
        }

        if (!await db.Users.AnyAsync(u => u.Username == "manager"))
        {
            var managerId = Guid.NewGuid();
            var employeeId = Guid.NewGuid();

            db.Users.AddRange(
                new User
                {
                    Id = managerId,
                    Username = "manager",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                    Role = UserRoles.Manager,
                    CreatedAtUtc = DateTime.UtcNow
                },
                new User
                {
                    Id = employeeId,
                    Username = "employee",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                    Role = UserRoles.Employee,
                    CreatedAtUtc = DateTime.UtcNow
                });

            db.EmployeeProfiles.AddRange(
                new EmployeeProfile
                {
                    UserId = managerId,
                    FullName = "สมชาย หัวหน้าทีม",
                    Email = "manager@nexuscore.local",
                    Phone = "081-111-1111",
                    DepartmentId = engineering.Id
                },
                new EmployeeProfile
                {
                    UserId = employeeId,
                    FullName = "สมหญิง พนักงาน",
                    Email = "employee@nexuscore.local",
                    Phone = "082-222-2222",
                    DepartmentId = engineering.Id,
                    ManagerId = managerId
                });
        }

        if (adminUser is null && !await db.Users.AnyAsync())
        {
            var adminId = Guid.NewGuid();
            db.Users.Add(new User
            {
                Id = adminId,
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                Role = UserRoles.Hr,
                CreatedAtUtc = DateTime.UtcNow
            });
            db.EmployeeProfiles.Add(new EmployeeProfile
            {
                UserId = adminId,
                FullName = "ผู้ดูแล HR",
                Email = "admin@nexuscore.local",
                DepartmentId = hrDept.Id
            });
        }

        await SeedLeaveEntitlementsAsync(db);
        await SeedHolidaysAsync(db);
        await db.SaveChangesAsync();
    }

    private static async Task SeedLeaveEntitlementsAsync(AppDbContext db)
    {
        if (await db.LeaveEntitlements.AnyAsync())
            return;

        var year = DateTime.UtcNow.Year;
        var leaveTypes = await db.LeaveTypes.ToListAsync();
        var profiles = await db.EmployeeProfiles.ToListAsync();
        if (leaveTypes.Count == 0 || profiles.Count == 0)
            return;

        foreach (var profile in profiles)
        {
            foreach (var leaveType in leaveTypes)
            {
                var days = leaveType.Code switch
                {
                    "ANNUAL" => 10m,
                    "SICK" => 30m,
                    "PERSONAL" => 5m,
                    _ => 5m
                };

                db.LeaveEntitlements.Add(new LeaveEntitlement
                {
                    Id = Guid.NewGuid(),
                    EmployeeId = profile.UserId,
                    LeaveTypeId = leaveType.Id,
                    Year = year,
                    DaysAllowed = days
                });
            }
        }
    }

    private static async Task SeedHolidaysAsync(AppDbContext db)
    {
        if (await db.CompanyHolidays.AnyAsync())
            return;

        var year = DateTime.UtcNow.Year;
        db.CompanyHolidays.AddRange(
            new CompanyHoliday { Id = Guid.NewGuid(), Date = new DateOnly(year, 1, 1), Name = "วันขึ้นปีใหม่" },
            new CompanyHoliday { Id = Guid.NewGuid(), Date = new DateOnly(year, 4, 13), Name = "วันสงกรานต์" },
            new CompanyHoliday { Id = Guid.NewGuid(), Date = new DateOnly(year, 12, 5), Name = "วันพ่อแห่งชาติ" },
            new CompanyHoliday { Id = Guid.NewGuid(), Date = new DateOnly(year, 12, 10), Name = "วันรัฐธรรมนูญ" },
            new CompanyHoliday { Id = Guid.NewGuid(), Date = new DateOnly(year, 12, 31), Name = "วันสิ้นปี" });
    }
}
