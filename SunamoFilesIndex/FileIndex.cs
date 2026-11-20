namespace SunamoFilesIndex;

/// <summary>
/// Represents the file index type.
/// </summary>
public partial class FileIndex
{
    // Without base paths
    static readonly List<string> relativeDirectories = [];

    // All files in the index
    /// <summary>
    /// The files.
    /// </summary>
    public List<FileItem> Files = [];

    // All folders which were processed except root
    private readonly List<FolderItem> _folders = [];

    private int _actualFolderID = -1;

    /// <summary>
    /// Gets or sets the base path.
    /// </summary>
    public string? BasePath { get; private set; }

    /// <summary>
    /// Get folders with name.
    /// </summary>
    /// <param name="parentIds">The parent ids.</param>
    /// <param name="name">The name.</param>
    /// <returns>The get folders with name result.</returns>
    public IList<FolderItem> GetFoldersWithName(int[] parentIds, string name)
    {
        if (parentIds == null)
        {
            return _folders.Where(c => c.Name == name).ToList();
        }

        return _folders.Where(c => c.Name == name).Where(d => parentIds.Contains(d.IDParent)).ToList();
    }

    /// <summary>
    /// Find all files with name.
    /// </summary>
    /// <param name="c">The c.</param>
    /// <returns>The find all files with name result.</returns>
    public List<FileItem> FindAllFilesWithName(string name)
    {
        return Files.FindAll(d => d.Name == name);
    }

    /// <summary>
    /// Add folder recursively.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>The add folder recursively result.</returns>
    public void AddFolderRecursively(string folder)
    {
        folder = FS.WithEndSlash(folder);
        BasePath = folder;
        _actualFolderID++;
        var dirs = Directory.GetDirectories(folder, "*", SearchOption.AllDirectories);
        foreach (var directory in dirs)
        {
            _folders.Add(GetFolderItem(directory));
            AddFilesFromFolder(folder, directory);
        }

        AddFilesFromFolder(folder, FS.WithoutEndSlash(folder));
    }

    private void AddFilesFromFolder(string basePath, string folder)
    {
        var filesInFolder = Directory.GetFiles(folder, "*.*", SearchOption.TopDirectoryOnly);
        filesInFolder.ToList().ForEach(filePath => Files.Add(GetFileItem(filePath, basePath)));
    }

    private FolderItem GetFolderItem(string path)
    {
        FolderItem folderItem = new()
        {
            IDParent = _actualFolderID,
            Name = Path.GetFileName(path),
            Path = Path.GetDirectoryName(path)
        };
        return folderItem;
    }

    /// <summary>
    /// Get relative folder.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>The get relative folder result.</returns>
    public int GetRelativeFolder(string folder)
    {
        folder = FS.WithEndSlash(folder);
        return relativeDirectories.IndexOf(folder);
    }

    /// <summary>
    /// Get relative folder.
    /// </summary>
    /// <param name="folder">The folder.</param>
    /// <returns>The get relative folder result.</returns>
    public string GetRelativeFolder(int folder)
    {
        return relativeDirectories[folder];
    }

    private FileItem GetFileItem(string path, string basePath)
    {
        FileItem fileItem = new()
        {
            Name = Path.GetFileName(path)
        };

        var basePathName = Path.GetDirectoryName(path);
        if (basePathName == null)
        {
            throw new Exception($"{basePathName} is null");
        }

        string relDirName = basePathName.Replace(basePath, "");
        if (!relativeDirectories.Contains(relDirName))
        {
            relativeDirectories.Add(relDirName);
            // Starts counting from 1
            fileItem.IDRelativeDirectory = relativeDirectories.Count;
        }
        else
        {
            fileItem.IDRelativeDirectory = relativeDirectories.IndexOf(relDirName) + 1;
        }

        return fileItem;
    }

    /// <summary>
    /// Nuke.
    /// </summary>
    /// <param name="relDirName">The rel dir name.</param>
    /// <returns>The nuke result.</returns>
    public void Nuke()
    {
        _folders.Clear();
        Files.Clear();
    }

    /// <summary>
    /// Get index of folder.
    /// </summary>
    public int GetIndexOfFolder(FolderItem item)
    {
        return _folders.IndexOf(item);
    }

    /// <summary>
    /// Get files in relative folder.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>The get files in relative folder result.</returns>
    public IList<FileItem> GetFilesInRelativeFolder(int relativeDirectoryIndex)
    {
        return Files.Where(c => c.IDRelativeDirectory == relativeDirectoryIndex).ToList();
    }

    /// <summary>
    /// Index folders.
    /// </summary>
    /// <param name="relativeDirectoryIndex">The relative directory index.</param>
    /// <returns>The index folders result.</returns>
    public static Dictionary<string, FileIndex> IndexFolders(IList<string> folders)
    {
        Dictionary<string, FileIndex> result = [];
        foreach (var folder in folders)
        {
            FileIndex fileIndex = new();
            fileIndex.AddFolderRecursively(folder);
            result.Add(folder, fileIndex);
        }

        return result;
    }

    /// <summary>
    /// Initializes a new instance of the aggregate files from all folders class.
    /// </summary>
    /// <param name="folder">The folder.</param>
    /// <param name="fileIndex">The file index.</param>
    public static void AggregateFilesFromAllFolders(string folderOfSolution, FileIndex fileIndex, Dictionary<string, int> relativeFilePathForEveryColumn, List<string> filesFromAllFoldersUniqueRelative)
    {
        foreach (var file in fileIndex.Files)
        {
            string relativeFilePath = (relativeDirectories[file.IDRelativeDirectory] + file.Name).Replace(folderOfSolution, "");
            if (!relativeFilePathForEveryColumn.ContainsKey(relativeFilePath))
            {
                int relativeDirectoryId = filesFromAllFoldersUniqueRelative.IndexOf(relativeFilePath);
                relativeFilePathForEveryColumn.Add(relativeFilePath, relativeDirectoryId);
            }
        }
    }

    /// <summary>
    /// Exists files on drive.
    /// </summary>
    /// <param name="relativeFilePath">The relative file path.</param>
    /// <returns>The exists files on drive result.</returns>
    public static CheckBoxDataShared<TWithSize<string>?>?[,] ExistsFilesOnDrive(Dictionary<string, FileIndex> files, Dictionary<string, int> relativeFilePathForEveryColumn)
    {
        int columns = relativeFilePathForEveryColumn.Count;
        CheckBoxDataShared<TWithSize<string>?>?[,] result = new CheckBoxDataShared<TWithSize<string>?>?[files.Count, columns];
        int rowIndex = -1;
        // Process all rows
        foreach (var fileIndexEntry in files)
        {
            rowIndex++;
            var fileIndex = fileIndexEntry.Value;
            for (int columnIndex = 0; columnIndex < fileIndex.Files.Count; columnIndex++)
            {
                // get files in column
                var file = fileIndex.Files[columnIndex];
                string relativeFilePath = (relativeDirectories[file.IDRelativeDirectory] + file.Name).Replace(fileIndexEntry.Key, "");
                int columnToInsert = relativeFilePathForEveryColumn[relativeFilePath];
                string fullFilePath = relativeDirectories[file.IDRelativeDirectory] + file.Name;
                if (File.Exists(fullFilePath))
                {
                    long fileSize = new FileInfo(fullFilePath).Length;
                    // To result set CheckBoxData - full path and size
                    result[rowIndex, columnToInsert] = new CheckBoxDataShared<TWithSize<string>?>
                    {
                        Value = new TWithSize<string>
                        {
                            Value = fullFilePath,
                            Size = fileSize
                        }
                    };
                }
                else
                {
                    result[rowIndex, columnToInsert] = null;
                }
            }
        }

        return result;
    }
}
