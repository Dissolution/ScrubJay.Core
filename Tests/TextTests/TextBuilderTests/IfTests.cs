using ScrubJay.Text;

namespace ScrubJay.Tests.TextTests.TextBuilderTests;

public class IfTests
{
    [Fact]
    public void IfWorks()
    {
        using var builder = new TextBuilder();

        Action<TextBuilder> onTrue = tb => tb.Append("true");

        builder.If(true, onTrue, TBA.NewLine);

        Assert.Equal(1, builder.Length);
        Assert.Equal('c', builder[0]);
    }
}