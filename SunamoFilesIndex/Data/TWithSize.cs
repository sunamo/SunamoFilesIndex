namespace SunamoFilesIndex.Data;

public class TWithSize<T>
{
    public T? Value { get; set; } = default;
    public long Size { get; set; } = 0;
}
