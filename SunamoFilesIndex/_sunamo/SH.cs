namespace SunamoFilesIndex._sunamo;

internal class SH
{
    #region SH.FirstCharUpper
    internal static void FirstCharUpper(ref string text)
    {
        text = FirstCharUpper(text);
    }

    internal static string FirstCharUpper(string text)
    {
        if (text.Length == 1)
        {
            return text.ToUpper();
        }
        var remainder = text.Substring(1);
        return text[0].ToString().ToUpper() + remainder;
    }
    #endregion
}
