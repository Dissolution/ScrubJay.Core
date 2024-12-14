namespace ScrubJay.Text.Tests;

public class InterpolatedTextTests
{
    public static TheoryData<char> Characters { get; } = new TheoryData<char>
    {
        '\0',
        'J',
        '\n',
        '‼',
        '❤',
    };
    
    public static TheoryData<string?> Strings { get; } = new TheoryData<string?>
    {
        null,
        string.Empty,
        "\0",
        Guid.NewGuid().ToString(),
        "❤️",
        "𬑨𛀫",
    };
    
    
    [Fact]
    public void CanInstantiateNew()
    {
        InterpolatedText text = new();
        
    }

    [Fact]
    public void CanDispose()
    {
        InterpolatedText text_a = new();
        ; // nop
        text_a.Dispose();

        using (InterpolatedText text_b = new())
        {
            ; // nop
        }
    }
    
    [Fact]
    public void CanDisposeMultipleTimes()
    {
        InterpolatedText text_a = new();
        ; // nop
        text_a.Dispose();
        ; // nop
        text_a.Dispose();
        ; // nop
        text_a.Dispose();
    }

    [Theory]
    [MemberData(nameof(Characters))]
    public void CanAppendChar(char ch)
    {
        using InterpolatedText text = new();
        text.AppendFormatted(ch);
        Assert.Equal(1, text.Length);
        Assert.Equal(ch, text[0]);
    }
    
    [Theory]
    [MemberData(nameof(Strings))]
    public void CanAppendString(string? str)
    {
        using InterpolatedText text = new();
        text.AppendFormatted(str);
        Assert.Equal(str?.Length ?? 0, text.Length);
        var textStr = text.ToString();
        if (str is null)
        {
            Assert.Empty(textStr);
        }
        else
        {
            Assert.Equal(str, textStr);
        }
    }
    
}