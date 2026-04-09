using Application.Common;
using Domain;

namespace Application.Features.FileSystem.DTOs;

public sealed class DirectoryEntryDto
{
    public string Name { get; set; } = "";
    public string RelativePath { get; set; } = "";
    public int FileCount { get; set; }
    public int SubDirectoryCount { get; set; }
    public long TotalSizeBytes { get; set; }
    public string TotalSizeFormatted { get; set; } = "";
    public DateTimeOffset LastModified { get; set; }

    public static DirectoryEntryDto From(DirectoryEntry entry) => new()
    {
        Name = entry.Name,
        RelativePath = entry.RelativePath,
        FileCount = entry.FileCount,
        SubDirectoryCount = entry.SubDirectoryCount,
        TotalSizeBytes = entry.TotalSizeBytes,
        TotalSizeFormatted = FileSizeFormatter.Format(entry.TotalSizeBytes),
        LastModified = entry.LastModified,
    };
}
