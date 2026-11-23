namespace SunamoFilesIndex.Data;

/// <summary>
/// Represents the t with size t type.
/// </summary>
public class TWithSize<T>
{
    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    public T? Value { get; set; } = default;
    /// <summary>
    /// Gets or sets the size.
    /// </summary>
    public long Size { get; set; } = 0;
}
