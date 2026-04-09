using Domain;
using Application.Common.Cqrs;

namespace Application.Features.FileSystem.Queries.SearchFileSystem;

public sealed record SearchFileSystemQuery(string Query, string RelativePath, bool Recursive) : IRequest<SearchResult>;
