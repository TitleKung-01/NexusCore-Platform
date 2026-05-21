namespace NexusCore.Application.Common;

public interface IFileStorage
{
    Task<StoredFileInfo> SaveAsync(string category, string fileName, Stream content, CancellationToken cancellationToken = default);
    Task<Stream?> OpenReadAsync(string storagePath, CancellationToken cancellationToken = default);
    bool Exists(string storagePath);
}

public record StoredFileInfo(string StoragePath, string ContentType, long SizeBytes);
