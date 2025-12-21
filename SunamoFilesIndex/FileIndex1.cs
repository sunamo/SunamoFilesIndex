namespace SunamoFilesIndex;

// EN: Variable names have been checked and replaced with self-descriptive names
// CZ: Názvy proměnných byly zkontrolovány a nahrazeny samopopisnými názvy
/// <summary>
/// Připomíná práci s databází - k označení složek se používají čísla int
///
/// Working with CheckBoxData
/// Use FolderItem, FileItem,
/// </summary>
public partial class FileIndex
{
    // TODO: Not use FileIndex, move to other locations
    /// <summary>
    /// Check (or uncheck) all in columns by filesize
    /// </summary>
    /// <param name = "allRows"></param>
    public static CheckBoxDataShared<TWithSize<string>>[, ] CheckVertically(CheckBoxDataShared<TWithSize<string>>[, ] allRows)
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
                CheckBoxDataShared<TWithSize<string>> cbd = allRows[r, c];
                if (cbd != null)
                {
                    if (cbd.t == null)
                    {
                        throw new Exception($"{cbd.t} is null");
                    }

                    fileSize.Add(r, cbd.t.size);
                    fileSize2.Add(cbd.t.size);
                }
            }

#region Get min and max size
            fileSize2.Sort();
            long min = fileSize2[0];
            long max = fileSize2[^1];
#endregion
#region Tick potencially unecesary files
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
    /// Check CheckBox in condition of size and A7 in location specified parameter row A2 and column A3
    ///
    /// A4 to find size of file. In keys are indexes.
    /// If size is A5 min, check.
    /// If A6 max, uncheck.
    /// Or none of this, set null. This behaviour can be changed setted A7 forceToAll
    /// </summary>
    /// <param name = "allRows"></param>
    /// <param name = "row"></param>
    /// <param name = "column"></param>
    /// <param name = "fileSize"></param>
    /// <param name = "min"></param>
    /// <param name = "max"></param>
    /// <param name = "forceToAll"></param>
    private static void TickIfItIsForDelete(CheckBoxDataShared<TWithSize<string>>[, ] allRows, int row, int column, Dictionary<int, long> fileSize, long min, long max, bool? forceToAll)
    {
        CheckBoxDataShared<TWithSize<string>> cbd = allRows[row, column];
        if (cbd != null)
        {
            long filSiz = fileSize[row];
            if (filSiz == -1)
            {
            }
            else if (filSiz == max)
            {
                if (forceToAll.HasValue)
                {
                    cbd.tick = forceToAll.Value;
                }
                else
                {
                    cbd.tick = false;
                }
            }
            else if (filSiz == min)
            {
                if (forceToAll.HasValue)
                {
                    cbd.tick = forceToAll.Value;
                }
                else
                {
                    cbd.tick = true;
                }
            }
            else
            {
                if (forceToAll.HasValue)
                {
                    cbd.tick = forceToAll.Value;
                }
                else
                {
                    cbd.tick = null;
                }
            }
        }
    }
}