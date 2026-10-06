namespace SunamoFilesIndex;

public partial class FileIndex
{
    // Without base paths
    static readonly List<string> relativeDirectories = [];

    // All files in the index
    public List<FileItem> Files = [];

    // All folders which were processed except root
    private readonly List<FolderItem> _folders = [];

    private int _actualFolderID = -1;

    public string? BasePath { get; private set; }

    public IList<FolderItem> GetFoldersWithName(int[] parentIds, string name)
    {
        if (parentIds == null)
        {
            return _folders.Where(c => c.Name == name).ToList();
        }

        return _folders.Where(c => c.Name == name).Where(d => parentIds.Contains(d.IDParent)).ToList();
    }

    public List<FileItem> FindAllFilesWithName(string name)
    {
        return Files.FindAll(d => d.Name == name);
    }

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

    public int GetRelativeFolder(string folder)
    {
        folder = FS.WithEndSlash(folder);
        return relativeDirectories.IndexOf(folder);
    }

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

    public void Nuke()
    {
        _folders.Clear();
        Files.Clear();
    }

    public int GetIndexOfFolder(FolderItem item)
    {
        return _folders.IndexOf(item);
    }

    public IList<FileItem> GetFilesInRelativeFolder(int relativeDirectoryIndex)
    {
        return Files.Where(c => c.IDRelativeDirectory == relativeDirectoryIndex).ToList();
    }

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
