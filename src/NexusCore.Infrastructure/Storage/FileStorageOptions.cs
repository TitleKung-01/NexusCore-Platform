namespace NexusCore.Infrastructure.Storage;

/// <summary>ตัวเลือกการตั้งค่าโฟลเดอร์รากสำหรับเก็บไฟล์อัปโหลด</summary>
public class FileStorageOptions
{
    /// <summary>ชื่อ section ใน appsettings</summary>
    public const string SectionName = "FileStorage";
    /// <summary>เส้นทางโฟลเดอร์ราก (ค่าเริ่มต้น ./uploads)</summary>
    public string RootPath { get; set; } = "./uploads";
}
