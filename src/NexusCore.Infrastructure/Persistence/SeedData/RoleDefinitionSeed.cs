using Microsoft.EntityFrameworkCore;
using NexusCore.Domain.Constants;
using NexusCore.Domain.Entities;

namespace NexusCore.Infrastructure.Persistence.SeedData;

/// <summary>Seed บทบาทมาตรฐาน (Employee, Manager, Hr, Admin)</summary>
internal static class RoleDefinitionSeed
{
    /// <summary>ใส่นิยามบทบาทเริ่มต้นถ้ายังไม่มีในฐานข้อมูล</summary>
    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.RoleDefinitions.AnyAsync())
            return;

        db.RoleDefinitions.AddRange(
            new RoleDefinition
            {
                Id = Guid.NewGuid(),
                Name = UserRoles.Employee,
                Description = "พนักงานทั่วไป",
                CanApprove = false,
                IsHrAccess = false,
                IsBuiltIn = true
            },
            new RoleDefinition
            {
                Id = Guid.NewGuid(),
                Name = UserRoles.Manager,
                Description = "หัวหน้างาน — อนุมัติคำขอของทีม",
                CanApprove = true,
                IsHrAccess = false,
                IsBuiltIn = true
            },
            new RoleDefinition
            {
                Id = Guid.NewGuid(),
                Name = UserRoles.Hr,
                Description = "ฝ่าย HR",
                CanApprove = true,
                IsHrAccess = true,
                IsBuiltIn = true
            },
            new RoleDefinition
            {
                Id = Guid.NewGuid(),
                Name = UserRoles.Admin,
                Description = "ผู้ดูแลระบบ",
                CanApprove = true,
                IsHrAccess = true,
                IsBuiltIn = true
            });

        await db.SaveChangesAsync();
    }
}
