using Application.Common;

namespace Infrastructure.FileSystem;

public sealed class FileSystemSettings : IFileSystemSettings
{
    public string RootPath { get; set; } = "";
    public long MaxUploadSizeBytes { get; set; } = 104_857_600; // 100 MB
    public List<string> AllowedExtensionsList { get; set; } = [];

    IReadOnlyList<string> IFileSystemSettings.AllowedExtensions => AllowedExtensionsList;
}
