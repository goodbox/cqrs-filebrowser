using Application.Common;
using Domain;

namespace Application.Features.FileSystem.DTOs;

public sealed class SearchResultDto
{
    public string Query { get; set; } = "";
    public string SearchRoot { get; set; } = "";
    public int TotalMatches { get; set; }
    public List<SearchMatchDto> Matches { get; set; } = [];

    public static SearchResultDto From(SearchResult result) => new()
    {
        Query = result.Query,
        SearchRoot = result.SearchRoot,
        TotalMatches = result.TotalMatches,
        Matches = result.Matches.Select(SearchMatchDto.From).ToList(),
    };
}

public sealed class SearchMatchDto
{
    public string Name { get; set; } = "";
    public string RelativePath { get; set; } = "";
    public bool IsDirectory { get; set; }
    public long? SizeBytes { get; set; }
    public string? SizeFormatted { get; set; }
    public DateTimeOffset LastModified { get; set; }

    public static SearchMatchDto From(FileSystemEntry entry)
    {
        if (entry is FileEntry file)
        {
            return new SearchMatchDto
            {
                Name = file.Name,
                RelativePath = file.RelativePath,
                IsDirectory = false,
                SizeBytes = file.SizeBytes,
                SizeFormatted = FileSizeFormatter.Format(file.SizeBytes),
                LastModified = file.LastModified,
            };
        }
        return new SearchMatchDto
        {
            Name = entry.Name,
            RelativePath = entry.RelativePath,
            IsDirectory = true,
            LastModified = entry.LastModified,
        };
    }
}
