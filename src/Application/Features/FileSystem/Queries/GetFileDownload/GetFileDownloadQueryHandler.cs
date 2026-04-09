using Application.Common;
using Application.Common.Cqrs;

namespace Application.Features.FileSystem.Queries.GetFileDownload;

public sealed class GetFileDownloadQueryHandler : IRequestHandler<GetFileDownloadQuery, FileDownloadResult>
{
    private readonly IFileSystemRepository _repository;

    public GetFileDownloadQueryHandler(IFileSystemRepository repository)
    {
        _repository = repository;
    }

    public async Task<FileDownloadResult> Handle(GetFileDownloadQuery request, CancellationToken cancellationToken)
    {
        var stream = await _repository.GetFileStreamAsync(request.RelativePath, cancellationToken);
        var fileName = Path.GetFileName(request.RelativePath);
        var contentType = GetContentType(fileName);
        var sizeBytes = stream.Length;

        return new FileDownloadResult(stream, fileName, contentType, sizeBytes);
    }

    private static string GetContentType(string fileName)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        return ext switch
        {
            ".pdf" => "application/pdf",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".txt" => "text/plain",
            ".html" or ".htm" => "text/html",
            ".json" => "application/json",
            ".xml" => "application/xml",
            ".zip" => "application/zip",
            _ => "application/octet-stream",
        };
    }
}
