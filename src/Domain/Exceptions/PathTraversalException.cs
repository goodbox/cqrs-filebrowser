namespace Domain.Exceptions;

public sealed class PathTraversalException : Exception
{
    public PathTraversalException(string path)
        : base($"Path '{path}' attempts to traverse outside the allowed root.") { }
}
