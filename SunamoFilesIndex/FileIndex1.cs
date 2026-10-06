namespace SunamoFilesIndex;

public partial class FileIndex
{
    // Check (or uncheck) all in columns by filesize
    public static CheckBoxDataShared<TWithSize<string>>[,] CheckVertically(CheckBoxDataShared<TWithSize<string>>[,] allRows)
    {
        int columns = allRows.GetLength(1);
        int rows = allRows.GetLength(0);
        // List all files
        for (int columnIndex = 0; columnIndex < columns; columnIndex++)
        {
            // Create collections for all rows
            // key - row, value - size
            Dictionary<int, long> fileSize = [];
            // For easy compare of size and find out any difference
            List<long> fileSize2 = [];
            for (int rowIndex = 0; rowIndex < rows; rowIndex++)
            {
                CheckBoxDataShared<TWithSize<string>> checkBoxData = allRows[rowIndex, columnIndex];
                if (checkBoxData != null)
                {
                    if (checkBoxData.Value == null)
                    {
                        throw new Exception($"{checkBoxData.Value} is null");
                    }

                    fileSize.Add(rowIndex, checkBoxData.Value.Size);
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
                    TickIfItIsForDelete(allRows, 0, columnIndex, fileSize, min, max, false);
                    for (int rowIndex = 1; rowIndex < rows; rowIndex++)
                    {
                        TickIfItIsForDelete(allRows, rowIndex, columnIndex, fileSize, min, max, true);
                    }
                }
                else
                {
                    for (int rowIndex = 0; rowIndex < rows; rowIndex++)
                    {
                        TickIfItIsForDelete(allRows, rowIndex, columnIndex, fileSize, min, max, null);
                    }
                }
            }
            else
            {
                // Maybe leave file with zero size?
                TickIfItIsForDelete(allRows, 0, columnIndex, fileSize, min, max, false);
            }
            #endregion
        }

        return allRows;
    }

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
                checkBoxData.Tick = forceToAll.HasValue ? forceToAll.Value : false;
            }
            else if (currentFileSize == min)
            {
                checkBoxData.Tick = forceToAll.HasValue ? forceToAll.Value : true;
            }
            else
            {
                checkBoxData.Tick = forceToAll.HasValue ? forceToAll.Value : (bool?)null;
            }
        }
    }
}
