using ScrubJay.Text;

namespace ScrubJay.Tests.TextTests;

public class ExtensionTests
{
    public static TheoryData<char> Characters { get; } = new TheoryData<char>
    {
        '\0',
        'J',
        '\n',
        '‼',
        '❤',
        char.MaxValue,
    };


    [Theory]
    [MemberData(nameof(Characters))]
    public void CharToSpanWorks(char ch)
    {
        var span = ch.AsSpan();
        Assert.Equal(1, span.Length);
        Assert.Equal(ch, span[0]);
    }
}