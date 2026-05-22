namespace NexusCore.Domain.Entities;

/// <summary>
/// ไฟล์แนบของคำขอลา (เช่น ใบรับรองแพทย์) — เก็บเมตาดาตาและตำแหน่งไฟล์
/// </summary>
public class LeaveAttachment
{
    /// <summary>รหัสไฟล์แนบ</summary>
    public Guid Id { get; set; }
    /// <summary>รหัสคำขอลาที่แนบมากับ</summary>
    public Guid LeaveRequestId { get; set; }
    /// <summary>คำขอลาที่เป็นเจ้าของไฟล์</summary>
    public LeaveRequest LeaveRequest { get; set; } = null!;
    /// <summary>ชื่อไฟล์ต้นฉบับ</summary>
    public string FileName { get; set; } = string.Empty;
    /// <summary>เส้นทางเก็บไฟล์ในระบบจัดเก็บ</summary>
    public string StoragePath { get; set; } = string.Empty;
    /// <summary>MIME type ของไฟล์</summary>
    public string ContentType { get; set; } = "application/octet-stream";
    /// <summary>ขนาดไฟล์เป็นไบต์</summary>
    public long SizeBytes { get; set; }
    /// <summary>วันเวลาอัปโหลด (UTC)</summary>
    public DateTime UploadedAtUtc { get; set; }
}
