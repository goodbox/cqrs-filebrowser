using Application.Common;
using Domain;

namespace Application.Features.FileSystem.DTOs;

public sealed class FileEntryDto
{
    public string Name { get; set; } = "";
    public string RelativePath { get; set; } = "";
    public long SizeBytes { get; set; }
    public string SizeFormatted { get; set; } = "";
    public string Extension { get; set; } = "";
    public DateTimeOffset LastModified { get; set; }

    public static FileEntryDto From(FileEntry entry) => new()
    {
        Name = entry.Name,
        RelativePath = entry.RelativePath,
        SizeBytes = entry.SizeBytes,
        SizeFormatted = FileSizeFormatter.Format(entry.SizeBytes),
        Extension = entry.Extension,
        LastModified = entry.LastModified,
    };
}
