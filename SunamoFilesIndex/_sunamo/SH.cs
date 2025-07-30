namespace SunamoFilesIndex._sunamo;

internal class SH
{
    #region SH.FirstCharUpper
    internal static void FirstCharUpper(ref string nazevPP)
    {
        nazevPP = FirstCharUpper(nazevPP);
    }
    internal static string FirstCharUpper(string nazevPP)
    {
        if (nazevPP.Length == 1)
        {
            return nazevPP.ToUpper();
        }
        string sb = nazevPP.Substring(1);
        return nazevPP[0].ToString().ToUpper() + sb;
    }
    #endregion
}