namespace NexusCore.Infrastructure.Storage;

public class FileStorageOptions
{
    public const string SectionName = "FileStorage";
    public string RootPath { get; set; } = "./uploads";
}
