using Microsoft.EntityFrameworkCore;
using NexusCore.Domain.Constants;
using NexusCore.Domain.Entities;

namespace NexusCore.Infrastructure.Persistence.SeedData;

/// <summary>Seed พนักงานตัวอย่าง ~30 คน — รหัสผ่านทุกบัญชี: password123</summary>
internal static class MockEmployeeSeed
{
    private const string DemoPassword = "password123";
    private const string EmailDomain = "hr-lite.local";
    /// <summary>เป้าหมาย 3 หัวหน้า + 27 พนักงาน (admin seed แยก)</summary>
    private const int TargetWorkforceCount = 30;

    private sealed record MockPerson(
        string Username,
        string FullName,
        string EmailLocalPart,
        string DepartmentCode,
        string Role,
        string? ManagerUsername,
        string Phone);

    private static readonly MockPerson[] People =
    [
        new("mgr.eng", "วิชัย จันทร์แจ้ง", "wichai.j", "ENG", UserRoles.Manager, null, "081-100-0001"),
        new("mgr.sales", "สุดา รักษ์ดี", "suda.r", "SAL", UserRoles.Manager, null, "081-100-0002"),
        new("mgr.hr", "ประภา มีสุข", "prapa.m", "HR", UserRoles.Manager, null, "081-100-0003"),
        new("emp001", "สมชาย ใจดี", "somchai.j", "ENG", UserRoles.Employee, "mgr.eng", "081-200-0001"),
        new("emp002", "สมหญิง พรหมมา", "somying.p", "ENG", UserRoles.Employee, "mgr.eng", "081-200-0002"),
        new("emp003", "อนุชา แสงทอง", "anucha.s", "ENG", UserRoles.Employee, "mgr.eng", "081-200-0003"),
        new("emp004", "ปิยะ กล้าหาญ", "piya.k", "ENG", UserRoles.Employee, "mgr.eng", "081-200-0004"),
        new("emp005", "นภา วัฒนา", "napa.w", "ENG", UserRoles.Employee, "mgr.eng", "081-200-0005"),
        new("emp006", "กิตติ ศรีสุข", "kitti.s", "ENG", UserRoles.Employee, "mgr.eng", "081-200-0006"),
        new("emp007", "ชลธิชา มณี", "chonticha.m", "ENG", UserRoles.Employee, "mgr.eng", "081-200-0007"),
        new("emp008", "ธนกร บุญเรือง", "thanakorn.b", "ENG", UserRoles.Employee, "mgr.eng", "081-200-0008"),
        new("emp009", "วราภรณ์ ทองคำ", "waraporn.t", "ENG", UserRoles.Employee, "mgr.eng", "081-200-0009"),
        new("emp010", "พงศกร ดีมาก", "pongsakorn.d", "ENG", UserRoles.Employee, "mgr.eng", "081-200-0010"),
        new("emp011", "รัตนา สุขใจ", "rattana.s", "SAL", UserRoles.Employee, "mgr.sales", "081-300-0001"),
        new("emp012", "เจษฎา คำดี", "jetsada.k", "SAL", UserRoles.Employee, "mgr.sales", "081-300-0002"),
        new("emp013", "มณีรัตน์ แก้วมา", "mneerat.k", "SAL", UserRoles.Employee, "mgr.sales", "081-300-0003"),
        new("emp014", "อภิชาติ รุ่งเรือง", "apichat.r", "SAL", UserRoles.Employee, "mgr.sales", "081-300-0004"),
        new("emp015", "ศิริพร นาคทอง", "siriporn.n", "SAL", UserRoles.Employee, "mgr.sales", "081-300-0005"),
        new("emp016", "กมลชนก ใจงาม", "kamonchanok.j", "SAL", UserRoles.Employee, "mgr.sales", "081-300-0006"),
        new("emp017", "ณัฐพล สว่าง", "nattapon.s", "SAL", UserRoles.Employee, "mgr.sales", "081-300-0007"),
        new("emp018", "พิมพ์ชนก อ่อนหวาน", "pimchanok.o", "SAL", UserRoles.Employee, "mgr.sales", "081-300-0008"),
        new("emp019", "สุทธิพงษ์ มั่นคง", "sutthipong.m", "SAL", UserRoles.Employee, "mgr.sales", "081-300-0009"),
        new("emp020", "อรทัย ฟ้าใส", "orathai.f", "HR", UserRoles.Employee, "mgr.hr", "081-400-0001"),
        new("emp021", "ชัยวัฒน์ มีชัย", "chaiwat.m", "HR", UserRoles.Employee, "mgr.hr", "081-400-0002"),
        new("emp022", "ดวงพร แสงจันทร์", "duangporn.s", "HR", UserRoles.Employee, "mgr.hr", "081-400-0003"),
        new("emp023", "เกียรติศักดิ์ ทองดี", "kiattisak.t", "ENG", UserRoles.Employee, "mgr.eng", "081-200-0011"),
        new("emp024", "ลัดดา พูลสวัสดิ์", "ladda.p", "SAL", UserRoles.Employee, "mgr.sales", "081-300-0010"),
        new("emp025", "วีระชัย ปานดี", "weerachai.p", "ENG", UserRoles.Employee, "mgr.eng", "081-200-0012"),
        new("emp026", "ยุพิน ศรีทอง", "yupin.s", "SAL", UserRoles.Employee, "mgr.sales", "081-300-0011"),
        new("emp027", "หทัยรัตน์ จิตรดี", "hathairat.j", "HR", UserRoles.Employee, "mgr.hr", "081-400-0004"),
        new("emp028", "ณัฐวุฒิ ชูใจ", "natthawut.c", "ENG", UserRoles.Employee, "mgr.eng", "081-200-0013"),
        new("emp029", "พรทิพย์ แก้วใส", "porntip.k", "SAL", UserRoles.Employee, "mgr.sales", "081-300-0012"),
        new("emp030", "ศศิธร ดำรง", "sasithorn.d", "HR", UserRoles.Employee, "mgr.hr", "081-400-0005"),
    ];

    /// <summary>สร้างผู้ใช้และโปรไฟล์พนักงาน demo ถ้ายังไม่ครบจำนวนเป้าหมาย</summary>
    public static async Task SeedAsync(AppDbContext db)
    {
        var workforceCount = await db.EmployeeProfiles.CountAsync();
        if (workforceCount >= TargetWorkforceCount)
            return;

        var departments = await db.Departments.ToDictionaryAsync(d => d.Code);
        if (!departments.ContainsKey("ENG") || !departments.ContainsKey("SAL") || !departments.ContainsKey("HR"))
            return;

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(DemoPassword);
        var now = DateTime.UtcNow;
        var userIds = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);

        foreach (var existing in await db.Users.Select(u => new { u.Id, u.Username }).ToListAsync())
            userIds[existing.Username] = existing.Id;

        foreach (var person in People)
        {
            if (userIds.ContainsKey(person.Username))
                continue;

            var id = Guid.NewGuid();
            db.Users.Add(new User
            {
                Id = id,
                Username = person.Username,
                PasswordHash = passwordHash,
                Role = person.Role,
                IsActive = true,
                CreatedAtUtc = now
            });
            userIds[person.Username] = id;
        }

        await db.SaveChangesAsync();

        foreach (var person in People)
        {
            if (!userIds.TryGetValue(person.Username, out var userId))
                continue;

            if (await db.EmployeeProfiles.AnyAsync(p => p.UserId == userId))
                continue;

            Guid? managerId = null;
            if (!string.IsNullOrEmpty(person.ManagerUsername) &&
                userIds.TryGetValue(person.ManagerUsername, out var mid))
                managerId = mid;

            db.EmployeeProfiles.Add(new EmployeeProfile
            {
                UserId = userId,
                FullName = person.FullName,
                Email = $"{person.EmailLocalPart}@{EmailDomain}",
                Phone = person.Phone,
                DepartmentId = departments[person.DepartmentCode].Id,
                ManagerId = managerId
            });
        }

        await db.SaveChangesAsync();
    }
}
