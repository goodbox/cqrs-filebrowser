using Application.Common;
using Application.Common.Cqrs;

namespace Application.Features.FileSystem.Commands.UploadFile;

public sealed class UploadFileCommandHandler : IRequestHandler<UploadFileCommand, UploadFileResult>
{
    private readonly IFileSystemRepository _repository;
    private readonly IFileSystemSettings _settings;

    public UploadFileCommandHandler(IFileSystemRepository repository, IFileSystemSettings settings)
    {
        _repository = repository;
        _settings = settings;
    }

    public async Task<UploadFileResult> Handle(UploadFileCommand request, CancellationToken cancellationToken)
    {
        if (request.SizeBytes > _settings.MaxUploadSizeBytes)
            throw new InvalidOperationException(
                $"File size {request.SizeBytes} exceeds maximum allowed size of {_settings.MaxUploadSizeBytes} bytes.");

        if (_settings.AllowedExtensions.Count > 0)
        {
            var ext = Path.GetExtension(request.FileName).ToLowerInvariant();
            if (!_settings.AllowedExtensions.Contains(ext))
                throw new InvalidOperationException($"File extension '{ext}' is not allowed.");
        }

        var relativePath = await _repository.SaveFileAsync(
            request.TargetDirectory,
            request.FileName,
            request.Content,
            cancellationToken);

        return new UploadFileResult(relativePath, request.FileName, request.SizeBytes);
    }
}
