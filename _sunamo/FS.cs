namespace SunamoFilesIndex._sunamo;

internal class FS
{
    internal static string WithoutEndSlash(string v)
    {
        return WithoutEndSlash(ref v);
    }
    internal static string WithoutEndSlash(ref string v)
    {
        v = v.TrimEnd('\\');
        return v;
    }
    internal static string WithEndSlash(string v)
    {
        return WithEndSlash(ref v);
    }
    internal static string WithEndSlash(ref string v)
    {
        if (v != string.Empty)
        {
            v = v.TrimEnd('\\') + '\\';
        }
        SH.FirstCharUpper(ref v);
        return v;
    }
}