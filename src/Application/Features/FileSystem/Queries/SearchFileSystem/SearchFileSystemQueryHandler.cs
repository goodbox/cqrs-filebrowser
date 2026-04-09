using Application.Common;
using Domain;
using Application.Common.Cqrs;

namespace Application.Features.FileSystem.Queries.SearchFileSystem;

public sealed class SearchFileSystemQueryHandler : IRequestHandler<SearchFileSystemQuery, SearchResult>
{
    private readonly IFileSystemRepository _repository;

    public SearchFileSystemQueryHandler(IFileSystemRepository repository)
    {
        _repository = repository;
    }

    public Task<SearchResult> Handle(SearchFileSystemQuery request, CancellationToken cancellationToken)
        => _repository.SearchAsync(request.Query, request.RelativePath, request.Recursive, cancellationToken);
}
