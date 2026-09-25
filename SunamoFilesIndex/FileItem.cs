namespace SunamoFilesIndex;

/// <summary>
/// Represents a file in the file index
/// </summary>
public class FileItem
{
    /// <summary>
    /// Gets or sets the file name
    /// </summary>
    public string? Name { get; set; } = null;

    /// <summary>
    /// Gets or sets the relative directory ID (starts counting from 1)
    /// Use with FileIndex.relativeDirectories to get the relative path
    /// </summary>
    public int IDRelativeDirectory { get; set; }
}