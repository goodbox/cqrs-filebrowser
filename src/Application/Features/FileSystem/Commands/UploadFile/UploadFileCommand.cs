using Application.Common.Cqrs;

namespace Application.Features.FileSystem.Commands.UploadFile;

public sealed record UploadFileCommand(
    string TargetDirectory,
    string FileName,
    Stream Content,
    long SizeBytes) : IRequest<UploadFileResult>;
