using ScrubJay.Memory;

namespace ScrubJay.Tests.Memory;

public class SpanEnumeratorTests
{
    [Fact]
    public void TestRefDiscard()
    {
        Span<char> text = new Span<char>("ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray());
        SpanEnumerator<char> textIterator = new(text);
        Assert.Equal(0, textIterator.EnumeratedCount);
        Assert.Equal(26, textIterator.UnenumeratedCount);

        var aTaken = textIterator.TakeWhile(ch => ch == '.');
        Assert.Equal(0, aTaken.Length);

        var bTaken = textIterator.TakeWhile(ch => ch == 'A');
        Assert.Equal(1, bTaken.Length);
        Assert.Equal('A', bTaken[0]);
        Assert.Equal(1,textIterator.EnumeratedCount);
        Assert.Equal(25, textIterator.UnenumeratedCount);

        var cTaken = textIterator.TakeWhile(ch => ch != 'F');
        Assert.Equal(4, cTaken.Length);
        Assert.Equal("BCDE", cTaken.ToString());
        Assert.Equal(5, textIterator.EnumeratedCount);
    }
}