namespace NexusCore.Application.Common;

/// <summary>
/// บริการจัดเก็บไฟล์แนบ (ลา สลิปเงินเดือน ฯลฯ) แยกตามหมวดหมู่
/// </summary>
public interface IFileStorage
{
    /// <summary>บันทึกไฟล์ลงที่เก็บและคืนข้อมูลเมตาดาต้า</summary>
    Task<StoredFileInfo> SaveAsync(string category, string fileName, Stream content, CancellationToken cancellationToken = default);

    /// <summary>เปิดสตรีมอ่านไฟล์จาก path ที่เก็บ</summary>
    Task<Stream?> OpenReadAsync(string storagePath, CancellationToken cancellationToken = default);

    /// <summary>ตรวจสอบว่าไฟล์มีอยู่ในระบบจัดเก็บหรือไม่</summary>
    bool Exists(string storagePath);
}

/// <summary>ข้อมูลไฟล์หลังบันทึกสำเร็จ (path, ชนิดเนื้อหา, ขนาด)</summary>
public record StoredFileInfo(string StoragePath, string ContentType, long SizeBytes);
