namespace Domain;

public sealed record DirectoryEntry : FileSystemEntry
{
    public required int FileCount { get; init; }
    public required int SubDirectoryCount { get; init; }
    public required long TotalSizeBytes { get; init; }
}
