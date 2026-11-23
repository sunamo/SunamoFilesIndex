namespace SunamoFilesIndex.Tests;

/// <summary>
/// Tests for the lookups of FileIndex.
/// </summary>
public class FileIndexLookupTests
{
    /// <summary>
    /// Tests that the count, the folder identifier and the full path match the indexed files and that a missing folder is skipped.
    /// </summary>
    [Fact]
    public void CountFolderIdAndFullPath()
    {
        var root = Path.Combine(Path.GetTempPath(), "sunamofilesindex-" + Guid.NewGuid().ToString("N"));
        var sub = Path.Combine(root, "a", "b");
        Directory.CreateDirectory(sub);
        File.WriteAllText(Path.Combine(sub, "inner.txt"), "x");
        File.WriteAllText(Path.Combine(root, "top.txt"), "x");
        try
        {
            var index = new FileIndex();
            index.AddFolderRecursively(Path.Combine(root, "missing"));
            Assert.Equal(0, index.Count);

            index.AddFolderRecursively(root);
            Assert.Equal(2, index.Count);

            var inner = index.FindAllFilesWithName("inner.txt").Single();
            Assert.Equal(inner.IDRelativeDirectory, index.FolderId("a/b"));
            Assert.Equal(inner.IDRelativeDirectory, index.FolderId(@"a\b\"));
            Assert.Equal(Path.Combine(sub, "inner.txt"), index.GetFullPath(inner));

            var top = index.FindAllFilesWithName("top.txt").Single();
            Assert.Equal(top.IDRelativeDirectory, index.FolderId(string.Empty));
            Assert.Equal(Path.Combine(root, "top.txt"), index.GetFullPath(top));
        }
        finally
        {
            Directory.Delete(root, true);
        }
    }
}