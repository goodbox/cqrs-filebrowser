using Application.Common;
using Application.Common.Cqrs;

namespace Application.Features.FileSystem.Commands.DeleteEntry;

public sealed class DeleteEntryCommandHandler : IRequestHandler<DeleteEntryCommand>
{
    private readonly IFileSystemRepository _repository;

    public DeleteEntryCommandHandler(IFileSystemRepository repository)
    {
        _repository = repository;
    }

    public Task Handle(DeleteEntryCommand request, CancellationToken cancellationToken)
        => _repository.DeleteAsync(request.RelativePath, cancellationToken);
}
