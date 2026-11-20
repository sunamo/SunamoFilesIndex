namespace SunamoFilesIndex;

/// <summary>
/// Represents the file item type.
/// </summary>
public class FileItem
{
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string? Name { get; set; } = null;

    // Starts counting from 1
    // Use with FileIndex.relativeDirectories to get the relative path
    /// <summary>
    /// Gets or sets the ID relative directory.
    /// </summary>
    public int IDRelativeDirectory { get; set; }
}
