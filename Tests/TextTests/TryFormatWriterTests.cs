
using ScrubJay.Text;

namespace ScrubJay.Tests.TextTests;

public class TryFormatWriterTests
{
    [Fact]
    public void TryWriteFormattedWorks()
    {
        TryFormatWriter writer = new(stackalloc char[64]);

        bool wrote = writer.Write<long>(147L, "N0");
        Assert.True(wrote);
        Assert.Equal(3, writer.Count);
        Assert.Equal("147", writer.ToString());

        DateTime dt = DateTime.Parse("12/16/2024 09:41:33");
        wrote = writer.Write<DateTime>(dt, "yyyy-MM-dd HH:mm:ss");
        Assert.True(wrote);
        Assert.Equal(22, writer.Count);
        Assert.Equal("2024-12-16 09:41:33", writer.Written[3..].ToString());
    }
}
