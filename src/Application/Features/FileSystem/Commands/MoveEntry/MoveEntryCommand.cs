using Application.Common.Cqrs;

namespace Application.Features.FileSystem.Commands.MoveEntry;

public sealed record MoveEntryCommand(string SourceRelativePath, string DestinationRelativePath) : IRequest;
