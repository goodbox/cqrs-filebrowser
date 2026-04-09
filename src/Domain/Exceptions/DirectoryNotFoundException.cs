namespace Domain.Exceptions;

public sealed class DirectoryNotFoundException : Exception
{
    public DirectoryNotFoundException(string path)
        : base($"Directory '{path}' was not found.") { }
}
