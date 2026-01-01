namespace SunamoFilesIndex.Data;

/// <summary>
/// Represents a value with an associated size
/// </summary>
/// <typeparam name="T">Type of the value</typeparam>
public class TWithSize<T>
{
    /// <summary>
    /// Gets or sets the value
    /// </summary>
    public T? Value { get; set; } = default;

    /// <summary>
    /// Gets or sets the size in bytes
    /// </summary>
    public long Size { get; set; } = 0;
}
