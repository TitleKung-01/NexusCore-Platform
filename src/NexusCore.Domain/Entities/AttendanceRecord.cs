namespace NexusCore.Domain.Entities;

public class AttendanceRecord
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public EmployeeProfile Employee { get; set; } = null!;
    public DateOnly WorkDate { get; set; }
    public DateTime? CheckInUtc { get; set; }
    public DateTime? CheckOutUtc { get; set; }
    /// <summary>สรุปงานวันนี้ (ไม่บังคับ) — มักกรอกตอนลงเวลาออก</summary>
    public string? WorkSummary { get; set; }
}
