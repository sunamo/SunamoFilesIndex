namespace SunamoFilesIndex;

public class FileItem
{
    public string? Name { get; set; } = null;

    // Starts counting from 1
    // Use with FileIndex.relativeDirectories to get the relative path
    public int IDRelativeDirectory { get; set; }
}
