// variables names: ok
namespace SunamoFilesIndex.Interfaces;

/// <summary>
/// Represents a file system item (file or folder)
/// </summary>
internal interface IFSItem : IName, IPath, IIDParent
{
    /// <summary>
    /// Gets or sets the length/size in bytes
    /// </summary>
    long Length { get; set; }
}
