using Domain.Exceptions;

namespace Application.Common;

public static class PathValidator
{
    public static string Validate(string rootPath, string relativePath)
    {
        // Normalize the root
        var normalizedRoot = Path.GetFullPath(rootPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

        // Combine and normalize — this resolves any .. segments
        var combined = string.IsNullOrEmpty(relativePath)
            ? normalizedRoot
            : Path.GetFullPath(Path.Combine(normalizedRoot, relativePath));

        if (!combined.StartsWith(normalizedRoot + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)
            && !combined.Equals(normalizedRoot, StringComparison.OrdinalIgnoreCase))
        {
            throw new PathTraversalException(relativePath);
        }

        return combined;
    }
}
