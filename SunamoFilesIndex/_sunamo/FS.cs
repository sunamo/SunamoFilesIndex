namespace SunamoFilesIndex._sunamo;

/// <summary>
/// File system utility methods
/// </summary>
internal class FS
{
    /// <summary>
    /// Removes trailing backslash from path
    /// </summary>
    /// <param name="path">The path to process</param>
    /// <returns>Path without ending slash</returns>
    internal static string WithoutEndSlash(string path)
    {
        return WithoutEndSlash(ref path);
    }

    /// <summary>
    /// Removes trailing backslash from path (ref version)
    /// </summary>
    /// <param name="path">The path to process</param>
    /// <returns>Path without ending slash</returns>
    internal static string WithoutEndSlash(ref string path)
    {
        path = path.TrimEnd('\\');
        return path;
    }

    /// <summary>
    /// Ensures path ends with backslash
    /// </summary>
    /// <param name="path">The path to process</param>
    /// <returns>Path with ending slash</returns>
    internal static string WithEndSlash(string path)
    {
        return WithEndSlash(ref path);
    }

    /// <summary>
    /// Ensures path ends with backslash and capitalizes first character (ref version)
    /// </summary>
    /// <param name="path">The path to process</param>
    /// <returns>Path with ending slash and capitalized first character</returns>
    internal static string WithEndSlash(ref string path)
    {
        if (path != string.Empty)
        {
            path = path.TrimEnd('\\') + '\\';
        }
        SH.FirstCharUpper(ref path);
        return path;
    }
}