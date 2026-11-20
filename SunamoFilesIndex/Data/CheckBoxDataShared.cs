namespace SunamoFilesIndex.Data;

/// <summary>
/// Represents the check box data shared t type.
/// </summary>
public class CheckBoxDataShared<T>
{
    /// <summary>
    /// Gets or sets the tick.
    /// </summary>
    public bool? Tick { get; set; } = false;

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    public T? Value { get; set; } = default;
}
