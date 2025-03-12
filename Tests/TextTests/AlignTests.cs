using ScrubJay.Text;

namespace ScrubJay.Tests.TextTests;

public class AlignTests
{
    [Fact]
    public void AlignNothingWorks()
    {
        using var text = new TextBuilder();
        text.Align([], 10);
        Assert.Equal(10, text.Length);

        text.Align([], -10);
        Assert.Equal(20, text.Length);
    }

    [Fact]
    public void AlignOneCharZeroWidthWorks()
    {
        using var text = new TextBuilder();
        text.Align('j', 0);
        Assert.Equal(1, text.Length);
    }

    [Fact]
    public void AlignOneCharLeftWorks()
    {
        using var text = new TextBuilder();
        text.Align('j', 4, alignment: Alignment.Left);
        Assert.Equal("j   ", text.ToString());

        text.Clear();

        text.Align('j', -4);
        Assert.Equal("j   ", text.ToString());
    }

    [Fact]
    public void AlignOneCharRightWorks()
    {
        using var text = new TextBuilder();
        text.Align('j', 4, alignment: Alignment.Right);
        Assert.Equal("   j", text.ToString());

        text.Clear();

        text.Align('j', 4);
        Assert.Equal("   j", text.ToString());
    }
}
