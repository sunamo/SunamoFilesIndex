namespace SunamoFilesIndex.Data;

public class CheckBoxDataShared<T>
{
    public bool? Tick { get; set; } = false;

    public T? Value { get; set; } = default;
}
