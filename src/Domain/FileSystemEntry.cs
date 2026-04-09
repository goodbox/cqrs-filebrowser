namespace Domain;

public abstract record FileSystemEntry
{
    public required string Name { get; init; }
    public required string FullPath { get; init; }
    public required string RelativePath { get; init; }
    public required DateTimeOffset LastModified { get; init; }
}
