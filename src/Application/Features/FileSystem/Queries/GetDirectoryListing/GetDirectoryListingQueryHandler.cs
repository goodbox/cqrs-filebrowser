using Application.Common;
using Domain;
using Application.Common.Cqrs;

namespace Application.Features.FileSystem.Queries.GetDirectoryListing;

public sealed class GetDirectoryListingQueryHandler : IRequestHandler<GetDirectoryListingQuery, DirectoryListing>
{
    private readonly IFileSystemRepository _repository;

    public GetDirectoryListingQueryHandler(IFileSystemRepository repository)
    {
        _repository = repository;
    }

    public Task<DirectoryListing> Handle(GetDirectoryListingQuery request, CancellationToken cancellationToken)
        => _repository.GetDirectoryListingAsync(request.RelativePath, cancellationToken);
}
