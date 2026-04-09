namespace Application.Features.FileSystem.Queries.GetFileDownload;

public sealed record FileDownloadResult(Stream Content, string FileName, string ContentType, long SizeBytes);
