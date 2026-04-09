using Application.Common;
using Domain;

namespace Application.Features.FileSystem.DTOs;

public sealed class DirectoryListingDto
{
    public string CurrentPath { get; set; } = "";
    public string? ParentPath { get; set; }
    public List<DirectoryEntryDto> Directories { get; set; } = [];
    public List<FileEntryDto> Files { get; set; } = [];
    public int TotalFileCount { get; set; }
    public int TotalDirectoryCount { get; set; }
    public long TotalSizeBytes { get; set; }
    public string TotalSizeFormatted { get; set; } = "";

    public static DirectoryListingDto From(DirectoryListing listing) => new()
    {
        CurrentPath = listing.CurrentPath,
        ParentPath = listing.ParentPath,
        Directories = listing.Directories.Select(DirectoryEntryDto.From).ToList(),
        Files = listing.Files.Select(FileEntryDto.From).ToList(),
        TotalFileCount = listing.TotalFileCount,
        TotalDirectoryCount = listing.TotalDirectoryCount,
        TotalSizeBytes = listing.TotalSizeBytes,
        TotalSizeFormatted = FileSizeFormatter.Format(listing.TotalSizeBytes),
    };
}
