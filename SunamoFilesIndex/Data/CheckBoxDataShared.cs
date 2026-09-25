namespace SunamoFilesIndex.Data;

public class CheckBoxDataShared<T>
{
    // true = checked, false = unchecked, null = indeterminate
    public bool? Tick { get; set; } = false;

    public T? Value { get; set; } = default;
}
