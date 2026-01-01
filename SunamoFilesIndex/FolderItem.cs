namespace SunamoFilesIndex;

/// <summary>
/// Represents a folder in the file index
/// </summary>
public class FolderItem : IFSItem
{
    /// <summary>
    /// Gets or sets the folder name
    /// </summary>
    public string? Name { get; set; } = null;

    /// <summary>
    /// Gets or sets the full folder path
    /// </summary>
    public string? Path { get; set; }

    /// <summary>
    /// Gets or sets the parent folder ID
    /// </summary>
    public int IDParent { get; set; } = -1;

    /// <summary>
    /// Gets or sets the total size of the folder in bytes
    /// </summary>
    public long Length { get; set; } = -1;

    /// <summary>
    /// Gets or sets whether this folder has subfolders
    /// </summary>
    public bool HasFolderSubfolder { get; set; } = false;
}
