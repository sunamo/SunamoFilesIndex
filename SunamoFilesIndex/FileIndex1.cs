namespace SunamoFilesIndex;

/// <summary>
/// Resembles database work - uses int numbers to mark folders
///
/// Working with CheckBoxData
/// Use FolderItem, FileItem
/// </summary>
public partial class FileIndex
{
    /// <summary>
    /// Check (or uncheck) all in columns by filesize
    /// </summary>
    /// <param name="allRows">The matrix of checkbox data to process</param>
    /// <returns>Updated matrix with ticked/unticked checkboxes based on file sizes</returns>
    public static CheckBoxDataShared<TWithSize<string>>[,] CheckVertically(CheckBoxDataShared<TWithSize<string>>[,] allRows)
    {
        int columns = allRows.GetLength(1);
        int rows = allRows.GetLength(0);
        // List all files
        for (int c = 0; c < columns; c++)
        {
            // Create collections for all rows
            // key - row, value - size
            Dictionary<int, long> fileSize = [];
            // For easy compare of size and find out any difference
            List<long> fileSize2 = [];
            for (int r = 0; r < rows; r++)
            {
                CheckBoxDataShared<TWithSize<string>> checkBoxData = allRows[r, c];
                if (checkBoxData != null)
                {
                    if (checkBoxData.Value == null)
                    {
                        throw new Exception($"{checkBoxData.Value} is null");
                    }

                    fileSize.Add(r, checkBoxData.Value.Size);
                    fileSize2.Add(checkBoxData.Value.Size);
                }
            }

            #region Get min and max size
            fileSize2.Sort();
            long min = fileSize2[0];
            long max = fileSize2[^1];
            #endregion

            #region Tick potentially unnecessary files
            if (fileSize.Count > 1)
            {
                if (min == max)
                {
                    TickIfItIsForDelete(allRows, 0, c, fileSize, min, max, false);
                    for (int r = 1; r < rows; r++)
                    {
                        TickIfItIsForDelete(allRows, r, c, fileSize, min, max, true);
                    }
                }
                else
                {
                    for (int r = 0; r < rows; r++)
                    {
                        TickIfItIsForDelete(allRows, r, c, fileSize, min, max, null);
                    }
                }
            }
            else
            {
                // Maybe leave file with zero size?
                TickIfItIsForDelete(allRows, 0, c, fileSize, min, max, false);
            }
            #endregion
        }

        return allRows;
    }

    /// <summary>
    /// Check CheckBox based on file size conditions at specified row and column location
    ///
    /// If size equals min, check the checkbox.
    /// If size equals max, uncheck the checkbox.
    /// Otherwise set to null or use forceToAll value if specified.
    /// </summary>
    /// <param name="allRows">The matrix of checkbox data</param>
    /// <param name="row">Row index</param>
    /// <param name="column">Column index</param>
    /// <param name="fileSize">Dictionary with row indices as keys and file sizes as values</param>
    /// <param name="min">Minimum file size</param>
    /// <param name="max">Maximum file size</param>
    /// <param name="forceToAll">Optional forced value for all checkboxes</param>
    private static void TickIfItIsForDelete(CheckBoxDataShared<TWithSize<string>>[,] allRows, int row, int column, Dictionary<int, long> fileSize, long min, long max, bool? forceToAll)
    {
        CheckBoxDataShared<TWithSize<string>> checkBoxData = allRows[row, column];
        if (checkBoxData != null)
        {
            long currentFileSize = fileSize[row];
            if (currentFileSize == -1)
            {
                // File size not available - do nothing
            }
            else if (currentFileSize == max)
            {
                if (forceToAll.HasValue)
                {
                    checkBoxData.Tick = forceToAll.Value;
                }
                else
                {
                    checkBoxData.Tick = false;
                }
            }
            else if (currentFileSize == min)
            {
                if (forceToAll.HasValue)
                {
                    checkBoxData.Tick = forceToAll.Value;
                }
                else
                {
                    checkBoxData.Tick = true;
                }
            }
            else
            {
                if (forceToAll.HasValue)
                {
                    checkBoxData.Tick = forceToAll.Value;
                }
                else
                {
                    checkBoxData.Tick = null;
                }
            }
        }
    }
}
