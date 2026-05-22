using NexusCore.Domain.Enums;

namespace NexusCore.Domain.Entities;

/// <summary>
/// ใบเบิกค่าใช้จ่ายของพนักงาน — รวมรายการย่อยและสถานะอนุมัติแบบเดียวกับคำขอลา
/// </summary>
public class ExpenseClaim
{
    /// <summary>รหัสใบเบิก</summary>
    public Guid Id { get; set; }
    /// <summary>รหัสพนักงานผู้เบิก</summary>
    public Guid EmployeeId { get; set; }
    /// <summary>โปรไฟล์ผู้เบิก</summary>
    public EmployeeProfile Employee { get; set; } = null!;
    /// <summary>หัวข้อ/ชื่อใบเบิก</summary>
    public string Title { get; set; } = string.Empty;
    /// <summary>ยอดรวมทั้งใบ (คำนวณจากรายการย่อย)</summary>
    public decimal TotalAmount { get; set; }
    /// <summary>สถานะ workflow ปัจจุบัน</summary>
    public LeaveStatus Status { get; set; } = LeaveStatus.Draft;
    /// <summary>วันเวลาส่งใบเบิก (UTC)</summary>
    public DateTime? SubmittedAtUtc { get; set; }
    /// <summary>วันเวลาที่มีการอนุมัติ/ปฏิเสธ (UTC)</summary>
    public DateTime? DecidedAtUtc { get; set; }
    /// <summary>รหัสผู้ตัดสิน</summary>
    public Guid? DecidedById { get; set; }
    /// <summary>ความเห็นจากผู้อนุมัติ (ไม่บังคับ)</summary>
    public string? ManagerComment { get; set; }
    /// <summary>วันเวลาสร้างใบเบิก (UTC)</summary>
    public DateTime CreatedAtUtc { get; set; }
    /// <summary>รายการค่าใช้จ่ายย่อยในใบเบิกนี้</summary>
    public ICollection<ExpenseLineItem> LineItems { get; set; } = [];
}

/// <summary>
/// รายการย่อยในใบเบิก — แยกคำอธิบายและจำนวนเงินเพื่อให้ตรวจสอบทีละรายการ
/// </summary>
public class ExpenseLineItem
{
    /// <summary>รหัสรายการ</summary>
    public Guid Id { get; set; }
    /// <summary>รหัสใบเบิกแม่</summary>
    public Guid ExpenseClaimId { get; set; }
    /// <summary>ใบเบิกที่สังกัด</summary>
    public ExpenseClaim ExpenseClaim { get; set; } = null!;
    /// <summary>คำอธิบายรายการ (เช่น ค่าแท็กซี่)</summary>
    public string Description { get; set; } = string.Empty;
    /// <summary>จำนวนเงินของรายการนี้</summary>
    public decimal Amount { get; set; }
}

/// <summary>
/// ไฟล์หลักฐานแนบใบเบิก — เก็บเมตาดาตาและตำแหน่งไฟล์
/// </summary>
public class ExpenseAttachment
{
    /// <summary>รหัสไฟล์แนบ</summary>
    public Guid Id { get; set; }
    /// <summary>รหัสใบเบิกที่แนบมากับ</summary>
    public Guid ExpenseClaimId { get; set; }
    /// <summary>ใบเบิกที่เป็นเจ้าของไฟล์</summary>
    public ExpenseClaim ExpenseClaim { get; set; } = null!;
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
