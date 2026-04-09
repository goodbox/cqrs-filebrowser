using Application.Common.Cqrs;

namespace Application.Features.FileSystem.Queries.GetFileDownload;

public sealed record GetFileDownloadQuery(string RelativePath) : IRequest<FileDownloadResult>;
