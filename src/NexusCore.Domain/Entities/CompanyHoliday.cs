namespace NexusCore.Domain.Entities;

/// <summary>
/// วันหยุดบริษัท — ใช้คำนวณวันลาและแสดงปฏิทิน HR
/// </summary>
public class CompanyHoliday
{
    /// <summary>รหัสวันหยุด</summary>
    public Guid Id { get; set; }
    /// <summary>วันที่เป็นวันหยุด</summary>
    public DateOnly Date { get; set; }
    /// <summary>ชื่อวันหยุด (เช่น วันสงกรานต์)</summary>
    public string Name { get; set; } = string.Empty;
}
