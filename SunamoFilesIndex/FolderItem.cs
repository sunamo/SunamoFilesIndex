namespace SunamoFilesIndex;

/// <summary>
/// Represents the folder item type.
/// </summary>
public class FolderItem : IFSItem
{
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string? Name { get; set; } = null;

    /// <summary>
    /// Gets or sets the path.
    /// </summary>
    public string? Path { get; set; }

    /// <summary>
    /// Gets or sets the ID parent.
    /// </summary>
    public int IDParent { get; set; } = -1;

    /// <summary>
    /// Gets or sets the length.
    /// </summary>
    public long Length { get; set; } = -1;

    /// <summary>
    /// Gets or sets the has folder subfolder.
    /// </summary>
    public bool HasFolderSubfolder { get; set; } = false;
}
