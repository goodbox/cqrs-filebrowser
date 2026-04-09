namespace Domain.Exceptions;

public sealed class FileNotFoundException : Exception
{
    public FileNotFoundException(string path)
        : base($"File '{path}' was not found.") { }
}
