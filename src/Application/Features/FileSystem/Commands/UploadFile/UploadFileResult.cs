namespace Application.Features.FileSystem.Commands.UploadFile;

public sealed record UploadFileResult(string RelativePath, string FileName, long SizeBytes);
