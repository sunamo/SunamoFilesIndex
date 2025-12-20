// EN: Variable names have been checked and replaced with self-descriptive names
// CZ: Názvy proměnných byly zkontrolovány a nahrazeny samopopisnými názvy
namespace SunamoFilesIndex;
/// <summary>
/// Připomíná práci s databází - k označení složek se používají čísla int
///
/// Working with CheckBoxData
/// Use FolderItem, FileItem,
/// </summary>
public partial class FileIndex
{
    /// <summary>
    /// Without base paths
    /// </summary>
    static readonly List<string> relativeDirectories = [];
    /// <summary>
    ///
    /// </summary>
    public List<FileItem> files = [];
    /// <summary>
    /// All folders which was processed expect root
    /// </summary>
    private readonly List<FolderItem> _folders = [];
    private int _actualFolderID = -1;
    // TODO: Is directories somewhere used?
    /// <summary>
    /// NEOBSAHUJE VSECHNY ZPRACOVANE SLOZKY
    /// Všechny složky tak jak byly postupně přidávany do metody AddFolderRecursively
    ///
    /// </summary>
    static readonly List<string> directories = [];
    public string? BasePath { get; private set; }

    /// <summary>
    /// Get folders with name A2. A1 is IDParent
    /// </summary>
    /// <param name = "prohledavatSlozky"></param>
    /// <param name = "name"></param>
    public IList<FolderItem> GetFoldersWithName(int[] prohledavatSlozky, string name)
    {
        if (prohledavatSlozky == null)
        {
            return _folders.Where(c => c.Name == name).ToList();
        }

        return _folders.Where(c => c.Name == name).Where(d => prohledavatSlozky.Contains(d.IDParent)).ToList();
    }

    public List<FileItem> FindAllFilesWithName(string name)
    {
        return files.FindAll(d => d.Name == name);
    }

    /// <summary>
    /// Process all files including subfolders
    ///
    /// A1 musí být cesta zakončená slashem
    /// </summary>
    /// <param name = "folder"></param>
    public void AddFolderRecursively(string folder)
    {
        folder = FS.WithEndSlash(folder);
        BasePath = folder;
        _actualFolderID++;
        directories.Add(folder);
        var dirs = Directory.GetDirectories(folder, "*", SearchOption.AllDirectories);
        foreach (var item in dirs)
        {
            _folders.Add(GetFolderItem(item));
            AddFilesFromFolder(folder, item);
        }

        AddFilesFromFolder(folder, FS.WithoutEndSlash(folder));
    }

    /// <summary>
    /// Index all files from A3.
    ///
    /// A1 - full path to base folder
    /// A2 - whether use relativeDirectories
    /// A3 - full path to actual folder
    /// Add with relative file path
    /// </summary>
    /// <param name = "basePath"></param>
    /// <param name = "folder"></param>
    private void AddFilesFromFolder(string basePath, string folder)
    {
        var files2 = Directory.GetFiles(folder, "*.*", SearchOption.TopDirectoryOnly);
        files2.ToList().ForEach(c => files.Add(GetFileItem(c, basePath)));
    }

    private FolderItem GetFolderItem(string p)
    {
        FolderItem fi = new()
        {
            IDParent = _actualFolderID,
            Name = Path.GetFileName(p),
            Path = Path.GetDirectoryName(p)
        };
        return fi;
    }

    /// <summary>
    /// Return index of folder or -1 if cannot found
    /// </summary>
    /// <param name = "folder"></param>
    public int GetRelativeFolder(string folder)
    {
        folder = FS.WithEndSlash(folder);
        return relativeDirectories.IndexOf(folder);
    }

    public string GetRelativeFolder(int folder)
    {
        return relativeDirectories[folder];
    }

    /// <summary>
    /// Return object FIleItem.
    /// Add to relativeDirectories, if A3.
    ///
    /// A3 - whether save to relativeDirectories and can use indexes for directory
    /// </summary>
    /// <param name = "p"></param>
    /// <param name = "basePath"></param>
    private FileItem GetFileItem(string p, string basePath)
    {
        FileItem fi = new()
        {
            //fi.IDDirectory = folders.Count;
            //fi.IDParent = actualFolderID;
            Name = Path.GetFileName(p)
        };
        //fi.Path = Path.GetDirectoryName(p);
        //if (relativeDirectoryName)
        //{
        var basePathName = Path.GetDirectoryName(p);
        if (basePathName == null)
        {
            throw new Exception($"{basePathName} is null");
        }

        string relDirName = basePathName.Replace(basePath, "");
        if (!relativeDirectories.Contains(relDirName))
        {
            relativeDirectories.Add(relDirName);
            // Počítá se od 1
            fi.IDRelativeDirectory = relativeDirectories.Count;
        }
        else
        {
            fi.IDRelativeDirectory = relativeDirectories.IndexOf(relDirName) + 1;
        }

        //}
        return fi;
    }

    /// <summary>
    /// Clear folders and files collection
    /// </summary>
    public void Nuke()
    {
        _folders.Clear();
        files.Clear();
    }

    public int GetIndexOfFolder(FolderItem item)
    {
        return _folders.IndexOf(item);
    }

    public IList<FileItem> GetFilesInRelativeFolder(int p)
    {
        return files.Where(c => c.IDRelativeDirectory == p).ToList();
    }

    /// <summary>
    /// Process recursively A1 - for every folder one object FileIndex in output
    ///
    /// </summary>
    /// <param name = "folders"></param>
    public static Dictionary<string, FileIndex> IndexFolders(IList<string> folders)
    {
        Dictionary<string, FileIndex> vr = [];
        foreach (var item in folders)
        {
            FileIndex fi = new();
            fi.AddFolderRecursively(item);
            vr.Add(item, fi);
        }

        return vr;
    }

    /// <summary>
    /// Prida do A3 soubor s relativni cestou pokud neexistuje
    /// Use relative path to file to find relative id directory and insert with file path to ID to A3
    ///
    /// A1 - base path, will be discard, used to make relative file paths from A2
    /// A2 -
    /// A3 - key is relative file path, value is index of relative directory
    /// A4 - relative paths to files which is used to fill A3. no change
    /// </summary>
    /// <param name = "folderOfSolution"></param>
    /// <param name = "fi"></param>
    /// <param name = "relativeFilePathForEveryColumn"></param>
    /// <param name = "filesFromAllFoldersUniqueRelative"></param>
    public static void AggregateFilesFromAllFolders(string folderOfSolution, FileIndex fi, Dictionary<string, int> relativeFilePathForEveryColumn, List<string> filesFromAllFoldersUniqueRelative)
    {
        foreach (var item2 in fi.files)
        {
            string relativeFilePath = (relativeDirectories[item2.IDRelativeDirectory] + item2.Name).Replace(folderOfSolution, "");
            if (!relativeFilePathForEveryColumn.ContainsKey(relativeFilePath))
            {
                int relativeDirectoryId = filesFromAllFoldersUniqueRelative.IndexOf(relativeFilePath);
                relativeFilePathForEveryColumn.Add(relativeFilePath, relativeDirectoryId);
            }
        }
    }

    /// <summary>
    /// Tato metoda má za úkol vytvořit matici ze souborů v A1, kde každý soubor bude v daném sloupci dle A2
    /// Kdyz soubor nebude existovat bude null
    ///
    /// Load size of files from disc
    /// In key of A2 - relativeFilePath, value - index of column.
    /// </summary>
    /// <param name = "files"></param>
    /// <param name = "relativeFilePathForEveryColumn"></param>
    public static CheckBoxDataShared<TWithSize<string>?>[, ] ExistsFilesOnDrive(Dictionary<string, FileIndex> files, Dictionary<string, int> relativeFilePathForEveryColumn)
    {
        int columns = relativeFilePathForEveryColumn.Count;
        CheckBoxDataShared<TWithSize<string>?>[, ] vr = new CheckBoxDataShared<TWithSize<string>?>[files.Count, columns];
        int r = -1;
        // Process all rows
        foreach (var item in files)
        {
            r++;
            var fi = item.Value;
            //List<long> fileSize = new List<long>(columns);
            //List<int> added = new List<int>();
            for (int c = 0; c < fi.files.Count; c++)
            {
                // get files in column c
                var file = fi.files[c];
                string relativeFilePath = (relativeDirectories[file.IDRelativeDirectory] + file.Name).Replace(item.Key, "");
                int columnToInsert = relativeFilePathForEveryColumn[relativeFilePath];
                string fullFilePath = relativeDirectories[file.IDRelativeDirectory] + file.Name;
                if (File.Exists(fullFilePath))
                {
                    long l2 = new FileInfo(fullFilePath).Length;
                    // To result set CheckBoxData - full path and size
                    vr[r, columnToInsert] = new CheckBoxDataShared<TWithSize<string>?>
                    {
                        t = new TWithSize<string>
                        {
                            t = fullFilePath,
                            size = l2
                        }
                    };
                }
                else
                {
#pragma warning disable CS8625
                    vr[r, columnToInsert] = null;
#pragma warning restore
                }
            }
        }

        return vr;
    }
}