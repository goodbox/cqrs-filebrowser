using Application.Common;
using Application.Common.Cqrs;

namespace Application.Features.FileSystem.Commands.MoveEntry;

public sealed class MoveEntryCommandHandler : IRequestHandler<MoveEntryCommand>
{
    private readonly IFileSystemRepository _repository;

    public MoveEntryCommandHandler(IFileSystemRepository repository)
    {
        _repository = repository;
    }

    public Task Handle(MoveEntryCommand request, CancellationToken cancellationToken)
        => _repository.MoveAsync(request.SourceRelativePath, request.DestinationRelativePath, cancellationToken);
}
