using Application.Common;
using Domain;
using Domain.Exceptions;
using DomainDirectoryNotFoundException = Domain.Exceptions.DirectoryNotFoundException;
using DomainFileNotFoundException = Domain.Exceptions.FileNotFoundException;

namespace Infrastructure.FileSystem;

public sealed class PhysicalFileSystemRepository : IFileSystemRepository
{
    private readonly IFileSystemSettings _settings;

    public PhysicalFileSystemRepository(IFileSystemSettings settings)
    {
        _settings = settings;
    }

    public Task<DirectoryListing> GetDirectoryListingAsync(string relativePath, CancellationToken ct)
    {
        var absolutePath = PathValidator.Validate(_settings.RootPath, relativePath);

        if (!Directory.Exists(absolutePath))
            throw new DomainDirectoryNotFoundException(relativePath);

        var dirInfo = new DirectoryInfo(absolutePath);

        var subdirs = dirInfo.GetDirectories()
            .OrderBy(d => d.Name, StringComparer.OrdinalIgnoreCase)
            .Select(d => BuildDirectoryEntry(d, relativePath))
            .ToList();

        var files = dirInfo.GetFiles()
            .OrderBy(f => f.Name, StringComparer.OrdinalIgnoreCase)
            .Select(f => BuildFileEntry(f, relativePath))
            .ToList();

        var normalizedRoot = Path.GetFullPath(_settings.RootPath);
        var currentRelative = GetRelativePath(normalizedRoot, absolutePath);
        var parentRelative = absolutePath.Equals(normalizedRoot, StringComparison.OrdinalIgnoreCase)
            ? (string?)null
            : GetRelativePath(normalizedRoot, dirInfo.Parent!.FullName);

        var listing = new DirectoryListing
        {
            CurrentPath = currentRelative,
            ParentPath = parentRelative,
            Directories = subdirs,
            Files = files,
            TotalFileCount = files.Count,
            TotalDirectoryCount = subdirs.Count,
            TotalSizeBytes = files.Sum(f => f.SizeBytes),
        };

        return Task.FromResult(listing);
    }

    public Task<SearchResult> SearchAsync(string query, string relativePath, bool recursive, CancellationToken ct)
    {
        var absolutePath = PathValidator.Validate(_settings.RootPath, relativePath);

        if (!Directory.Exists(absolutePath))
            throw new DomainDirectoryNotFoundException(relativePath);

        var normalizedRoot = Path.GetFullPath(_settings.RootPath);
        var option = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

        var entries = new DirectoryInfo(absolutePath)
            .EnumerateFileSystemInfos("*", option)
            .Where(e => e.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
            .Select(e => e is FileInfo fi
                ? (FileSystemEntry)BuildFileEntry(fi, GetRelativePath(normalizedRoot, fi.Directory!.FullName))
                : BuildDirectoryEntry((DirectoryInfo)e, GetRelativePath(normalizedRoot, ((DirectoryInfo)e).Parent!.FullName)))
            .ToList();

        var result = new SearchResult
        {
            Query = query,
            SearchRoot = GetRelativePath(normalizedRoot, absolutePath),
            Matches = entries,
            TotalMatches = entries.Count,
        };

        return Task.FromResult(result);
    }

    public Task<Stream> GetFileStreamAsync(string relativePath, CancellationToken ct)
    {
        var absolutePath = PathValidator.Validate(_settings.RootPath, relativePath);

        if (!File.Exists(absolutePath))
            throw new DomainFileNotFoundException(relativePath);

        Stream stream = File.OpenRead(absolutePath);
        return Task.FromResult(stream);
    }

    public async Task<string> SaveFileAsync(string relativePath, string fileName, Stream content, CancellationToken ct)
    {
        var absoluteDir = PathValidator.Validate(_settings.RootPath, relativePath);

        if (!Directory.Exists(absoluteDir))
            Directory.CreateDirectory(absoluteDir);

        var safeFileName = Path.GetFileName(fileName);
        var absoluteFilePath = Path.Combine(absoluteDir, safeFileName);

        await using var fs = File.Create(absoluteFilePath);
        await content.CopyToAsync(fs, ct);

        var normalizedRoot = Path.GetFullPath(_settings.RootPath);
        return GetRelativePath(normalizedRoot, absoluteFilePath);
    }

    public Task DeleteAsync(string relativePath, CancellationToken ct)
    {
        var absolutePath = PathValidator.Validate(_settings.RootPath, relativePath);

        if (Directory.Exists(absolutePath))
            Directory.Delete(absolutePath, recursive: true);
        else if (File.Exists(absolutePath))
            File.Delete(absolutePath);
        else
            throw new DomainFileNotFoundException(relativePath);

        return Task.CompletedTask;
    }

    public Task MoveAsync(string sourceRelativePath, string destinationRelativePath, CancellationToken ct)
    {
        var absoluteSource = PathValidator.Validate(_settings.RootPath, sourceRelativePath);
        var absoluteDest = PathValidator.Validate(_settings.RootPath, destinationRelativePath);

        if (Directory.Exists(absoluteSource))
            Directory.Move(absoluteSource, absoluteDest);
        else if (File.Exists(absoluteSource))
            File.Move(absoluteSource, absoluteDest);
        else
            throw new DomainFileNotFoundException(sourceRelativePath);

        return Task.CompletedTask;
    }

    public bool IsFile(string relativePath)
    {
        var absolutePath = PathValidator.Validate(_settings.RootPath, relativePath);
        return File.Exists(absolutePath);
    }

    public bool IsDirectory(string relativePath)
    {
        var absolutePath = PathValidator.Validate(_settings.RootPath, relativePath);
        return Directory.Exists(absolutePath);
    }

    private FileEntry BuildFileEntry(FileInfo fi, string parentRelativePath)
    {
        var rel = string.IsNullOrEmpty(parentRelativePath)
            ? fi.Name
            : $"{parentRelativePath}/{fi.Name}";

        return new FileEntry
        {
            Name = fi.Name,
            FullPath = fi.FullName,
            RelativePath = rel,
            LastModified = fi.LastWriteTimeUtc,
            SizeBytes = fi.Length,
            Extension = fi.Extension.ToLowerInvariant(),
        };
    }

    private DirectoryEntry BuildDirectoryEntry(DirectoryInfo di, string parentRelativePath)
    {
        var rel = string.IsNullOrEmpty(parentRelativePath)
            ? di.Name
            : $"{parentRelativePath}/{di.Name}";

        int fileCount = 0;
        int subDirCount = 0;
        long totalSize = 0;

        try
        {
            fileCount = di.GetFiles().Length;
            subDirCount = di.GetDirectories().Length;
            totalSize = di.GetFiles().Sum(f => f.Length);
        }
        catch (UnauthorizedAccessException) { }

        return new DirectoryEntry
        {
            Name = di.Name,
            FullPath = di.FullName,
            RelativePath = rel,
            LastModified = di.LastWriteTimeUtc,
            FileCount = fileCount,
            SubDirectoryCount = subDirCount,
            TotalSizeBytes = totalSize,
        };
    }

    private static string GetRelativePath(string root, string absolute)
    {
        var normalized = Path.GetFullPath(absolute);
        var normalizedRoot = Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar);

        if (normalized.Equals(normalizedRoot, StringComparison.OrdinalIgnoreCase))
            return "";

        var rel = Path.GetRelativePath(normalizedRoot, normalized);
        return rel.Replace('\\', '/');
    }
}
