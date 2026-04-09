using Application.Common.Cqrs;

namespace Application.Features.FileSystem.Commands.DeleteEntry;

public sealed record DeleteEntryCommand(string RelativePath) : IRequest;
