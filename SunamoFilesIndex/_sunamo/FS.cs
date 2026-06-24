namespace SunamoFilesIndex._sunamo;

internal class FS
{
    internal static string WithoutEndSlash(string path) => WithoutEndSlash(ref path);

    internal static string WithoutEndSlash(ref string path)
    {
        path = path.TrimEnd('\\');
        return path;
    }

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
