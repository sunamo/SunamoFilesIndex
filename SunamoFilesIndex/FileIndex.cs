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
    /// Without base paths
    /// </summary>
    static readonly List<string> relativeDirectories = [];

    /// <summary>
    /// All files in the index
    /// </summary>
    public List<FileItem> files = [];

    /// <summary>
    /// All folders which were processed except root
    /// </summary>
    private readonly List<FolderItem> _folders = [];

    private int _actualFolderID = -1;


    /// <summary>
    /// Gets the base path of the indexed folder structure
    /// </summary>
    public string? BasePath { get; private set; }

    /// <summary>
    /// Get folders with specified name from given parent IDs
    /// </summary>
    /// <param name="parentIds">Array of parent folder IDs to search in (null = search all)</param>
    /// <param name="name">Name of the folder to find</param>
    /// <returns>List of folders matching the criteria</returns>
    public IList<FolderItem> GetFoldersWithName(int[] parentIds, string name)
    {
        if (parentIds == null)
        {
            return _folders.Where(c => c.Name == name).ToList();
        }

        return _folders.Where(c => c.Name == name).Where(d => parentIds.Contains(d.IDParent)).ToList();
    }

    /// <summary>
    /// Find all files with the specified name
    /// </summary>
    /// <param name="name">Name of the file to find</param>
    /// <returns>List of files with matching name</returns>
    public List<FileItem> FindAllFilesWithName(string name)
    {
        return files.FindAll(d => d.Name == name);
    }

    /// <summary>
    /// Process all files including subfolders
    /// Folder path must end with backslash
    /// </summary>
    /// <param name="folder">Folder path to index (must end with backslash)</param>
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

    /// <summary>
    /// Index all files from specified folder
    /// Add files with relative file path
    /// </summary>
    /// <param name="basePath">Full path to base folder</param>
    /// <param name="folder">Full path to actual folder to process</param>
    private void AddFilesFromFolder(string basePath, string folder)
    {
        var filesInFolder = Directory.GetFiles(folder, "*.*", SearchOption.TopDirectoryOnly);
        filesInFolder.ToList().ForEach(filePath => files.Add(GetFileItem(filePath, basePath)));
    }

    /// <summary>
    /// Create FolderItem from path
    /// </summary>
    /// <param name="path">Full path to the folder</param>
    /// <returns>FolderItem with populated properties</returns>
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
    /// Return index of folder or -1 if not found
    /// </summary>
    /// <param name="folder">Folder path to search for</param>
    /// <returns>Index of the folder or -1 if not found</returns>
    public int GetRelativeFolder(string folder)
    {
        folder = FS.WithEndSlash(folder);
        return relativeDirectories.IndexOf(folder);
    }

    /// <summary>
    /// Get relative folder path by index
    /// </summary>
    /// <param name="folder">Index of the folder</param>
    /// <returns>Relative folder path</returns>
    public string GetRelativeFolder(int folder)
    {
        return relativeDirectories[folder];
    }

    /// <summary>
    /// Return FileItem object
    /// Add to relativeDirectories if needed and use indexes for directory
    /// </summary>
    /// <param name="path">Full path to the file</param>
    /// <param name="basePath">Base path to create relative paths</param>
    /// <returns>FileItem with populated properties</returns>
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
    /// Clear folders and files collection
    /// </summary>
    public void Nuke()
    {
        _folders.Clear();
        files.Clear();
    }

    /// <summary>
    /// Get index of folder in internal collection
    /// </summary>
    /// <param name="item">Folder item to find</param>
    /// <returns>Index of the folder</returns>
    public int GetIndexOfFolder(FolderItem item)
    {
        return _folders.IndexOf(item);
    }

    /// <summary>
    /// Get all files in relative folder by index
    /// </summary>
    /// <param name="relativeDirectoryIndex">Index of the relative directory</param>
    /// <returns>List of files in that directory</returns>
    public IList<FileItem> GetFilesInRelativeFolder(int relativeDirectoryIndex)
    {
        return files.Where(c => c.IDRelativeDirectory == relativeDirectoryIndex).ToList();
    }

    /// <summary>
    /// Process recursively all folders - for every folder one FileIndex object in output
    /// </summary>
    /// <param name="folders">List of folder paths to index</param>
    /// <returns>Dictionary with folder paths as keys and FileIndex objects as values</returns>
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
    /// Add file to dictionary with relative path if it doesn't exist
    /// Use relative path to file to find relative directory ID and insert with file path ID to dictionary
    /// </summary>
    /// <param name="folderOfSolution">Base path that will be discarded, used to make relative file paths</param>
    /// <param name="fileIndex">FileIndex object containing files to process</param>
    /// <param name="relativeFilePathForEveryColumn">Dictionary where key is relative file path, value is column index</param>
    /// <param name="filesFromAllFoldersUniqueRelative">List of relative file paths, used to fill dictionary, not modified</param>
    public static void AggregateFilesFromAllFolders(string folderOfSolution, FileIndex fileIndex, Dictionary<string, int> relativeFilePathForEveryColumn, List<string> filesFromAllFoldersUniqueRelative)
    {
        foreach (var file in fileIndex.files)
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
    /// Create matrix from files where each file will be in specific column
    /// When file doesn't exist, it will be null
    /// Load size of files from disc
    /// </summary>
    /// <param name="files">Dictionary with folder paths as keys and FileIndex objects as values</param>
    /// <param name="relativeFilePathForEveryColumn">Dictionary where key is relative file path, value is column index</param>
    /// <returns>Matrix of CheckBoxData with file information</returns>
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
            for (int columnIndex = 0; columnIndex < fileIndex.files.Count; columnIndex++)
            {
                // get files in column
                var file = fileIndex.files[columnIndex];
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
