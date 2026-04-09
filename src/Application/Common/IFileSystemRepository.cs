using Domain;

namespace Application.Common;

public interface IFileSystemRepository
{
    Task<DirectoryListing> GetDirectoryListingAsync(string relativePath, CancellationToken ct);
    Task<SearchResult> SearchAsync(string query, string relativePath, bool recursive, CancellationToken ct);
    Task<Stream> GetFileStreamAsync(string relativePath, CancellationToken ct);
    Task<string> SaveFileAsync(string relativePath, string fileName, Stream content, CancellationToken ct);
    Task DeleteAsync(string relativePath, CancellationToken ct);
    Task MoveAsync(string sourceRelativePath, string destinationRelativePath, CancellationToken ct);
    bool IsFile(string relativePath);
    bool IsDirectory(string relativePath);
}
