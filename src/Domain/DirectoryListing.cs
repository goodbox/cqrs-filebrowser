namespace Domain;

public sealed record DirectoryListing
{
    public required string CurrentPath { get; init; }
    public required string? ParentPath { get; init; }
    public required IReadOnlyList<DirectoryEntry> Directories { get; init; }
    public required IReadOnlyList<FileEntry> Files { get; init; }
    public required int TotalFileCount { get; init; }
    public required int TotalDirectoryCount { get; init; }
    public required long TotalSizeBytes { get; init; }
}
