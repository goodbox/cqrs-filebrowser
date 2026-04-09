namespace Domain;

public sealed record FileEntry : FileSystemEntry
{
    public required long SizeBytes { get; init; }
    public required string Extension { get; init; }
}
