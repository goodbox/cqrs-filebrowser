using Domain;
using Application.Common.Cqrs;

namespace Application.Features.FileSystem.Queries.GetDirectoryListing;

public sealed record GetDirectoryListingQuery(string RelativePath) : IRequest<DirectoryListing>;
