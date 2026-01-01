namespace SunamoFilesIndex._sunamo;

/// <summary>
/// String helper methods
/// </summary>
internal class SH
{
    #region SH.FirstCharUpper
    /// <summary>
    /// Capitalizes the first character of the input string (ref version)
    /// </summary>
    /// <param name="text">The text to capitalize</param>
    internal static void FirstCharUpper(ref string text)
    {
        text = FirstCharUpper(text);
    }

    /// <summary>
    /// Capitalizes the first character of the input string
    /// </summary>
    /// <param name="text">The text to capitalize</param>
    /// <returns>String with capitalized first character</returns>
    internal static string FirstCharUpper(string text)
    {
        if (text.Length == 1)
        {
            return text.ToUpper();
        }
        string remainder = text.Substring(1);
        return text[0].ToString().ToUpper() + remainder;
    }
    #endregion
}
