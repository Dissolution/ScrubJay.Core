using ScrubJay.Memory;

namespace ScrubJay.Tests.MemoryTests;

public class FormatWriterExtensionsTests
{
    [Fact]
    public void TryWriteFormattedWorks()
    {
        SpanWriter<char> writer = new(stackalloc char[64]);

        bool wrote = writer.TryWriteFormatted<long>(147L, "N0");
        Assert.True(wrote);
        Assert.Equal(3, writer.Count);
        Assert.Equal("147", writer.Written.ToString());

        DateTime dt = DateTime.Parse("12/16/2024 09:41:33");
        wrote = writer.TryWriteFormatted<DateTime>(dt, "yyyy-MM-dd HH:mm:ss");
        Assert.True(wrote);
        Assert.Equal(22, writer.Count);
        Assert.Equal("2024-12-16 09:41:33", writer.Written[3..].ToString());
    }
}
