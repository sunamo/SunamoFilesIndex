namespace SunamoFilesIndex._sunamo;

internal class FS
{
    /// <summary>
    /// Removes trailing backslash from path
    /// </summary>
    /// <param name="path">The path to process</param>
    /// <returns>Path without ending slash</returns>
    internal static string WithoutEndSlash(string path) => WithoutEndSlash(ref path);

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
    internal static string WithEndSlash(string path) => WithEndSlash(ref path);

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
