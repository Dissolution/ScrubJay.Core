using ScrubJay.Text;

namespace ScrubJay.Tests.TextTests.TextBuilderTests;

public class IfTests
{
    [Fact]
    public void IfWorks()
    {
        using var builder = new TextBuilder();

        builder.If(true, 'c', (147, "D2"));

        Assert.Equal(1, builder.Length);
        Assert.Equal('c', builder[0]);
    }
}