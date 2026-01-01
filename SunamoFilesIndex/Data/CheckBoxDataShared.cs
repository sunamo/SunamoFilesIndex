namespace SunamoFilesIndex.Data;

/// <summary>
/// Represents checkbox data with a tick state and a value
/// </summary>
/// <typeparam name="T">Type of the value</typeparam>
public class CheckBoxDataShared<T>
{
    /// <summary>
    /// Gets or sets the tick state of the checkbox (true = checked, false = unchecked, null = indeterminate)
    /// </summary>
    public bool? Tick { get; set; } = false;

    /// <summary>
    /// Gets or sets the value associated with this checkbox
    /// </summary>
    public T? Value { get; set; } = default;
}
