namespace Domain;

public sealed record SearchResult
{
    public required string Query { get; init; }
    public required string SearchRoot { get; init; }
    public required IReadOnlyList<FileSystemEntry> Matches { get; init; }
    public required int TotalMatches { get; init; }
}
