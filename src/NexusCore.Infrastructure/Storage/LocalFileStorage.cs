using Microsoft.Extensions.Options;
using NexusCore.Application.Common;

namespace NexusCore.Infrastructure.Storage;

/// <summary>บริการเก็บไฟล์บนดิสก์ภายในเครื่อง แยกตามหมวดและปี/เดือน</summary>
public class LocalFileStorage(IOptions<FileStorageOptions> options) : IFileStorage
{
    private string Root => Path.GetFullPath(options.Value.RootPath);

    /// <summary>บันทึกไฟล์ลงโฟลเดอร์ {Root}/{category}/{yyyy/MM} พร้อมชื่อไฟล์สุ่ม</summary>
    public async Task<StoredFileInfo> SaveAsync(string category, string fileName, Stream content, CancellationToken cancellationToken = default)
    {
        var safeName = Path.GetFileName(fileName);
        var dir = Path.Combine(Root, category, DateTime.UtcNow.ToString("yyyy/MM"));
        Directory.CreateDirectory(dir);
        var storagePath = Path.Combine(dir, $"{Guid.NewGuid():N}_{safeName}");
        await using var fs = File.Create(storagePath);
        await content.CopyToAsync(fs, cancellationToken);
        var info = new FileInfo(storagePath);
        return new StoredFileInfo(storagePath, GetContentType(safeName), info.Length);
    }

    /// <summary>เปิดสตรีมอ่านไฟล์ตามเส้นทางที่บันทึกไว้</summary>
    public Task<Stream?> OpenReadAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(storagePath))
            return Task.FromResult<Stream?>(null);
        return Task.FromResult<Stream?>(File.OpenRead(storagePath));
    }

    /// <summary>ตรวจว่าไฟล์มีอยู่บนดิสก์หรือไม่</summary>
    public bool Exists(string storagePath) => File.Exists(storagePath);

    /// <summary>กำหนด MIME type จากนามสกุลไฟล์</summary>
    private static string GetContentType(string fileName)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        return ext switch
        {
            ".pdf" => "application/pdf",
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            _ => "application/octet-stream"
        };
    }
}
