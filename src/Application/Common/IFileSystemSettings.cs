namespace Application.Common;

public interface IFileSystemSettings
{
    string RootPath { get; }
    long MaxUploadSizeBytes { get; }
    IReadOnlyList<string> AllowedExtensions { get; }
}
