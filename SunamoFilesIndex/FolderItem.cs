namespace SunamoFilesIndex;

public class FolderItem : IFSItem
{
    public string? Name { get; set; } = null;
    public string? Path { get; set; }
    public int IDParent { get; set; } = -1;
    public long Length { get; set; } = -1;
    public bool HasFolderSubfolder { get; set; } = false;
}