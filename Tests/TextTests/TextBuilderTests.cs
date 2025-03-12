using ScrubJay.Text;
using ScrubJay.Utilities;
// ReSharper disable CollectionNeverUpdated.Local

namespace ScrubJay.Tests.TextTests;

public class TextBuilderTests
{
    public const string SHORT_STRING = "TRJ";
    public const string LONG_STRING = "Sphinx of Black Quartz, Judge My Vow.";

    [Fact]
    public void CanRefIndex()
    {
        using var text = TextBuilder.New;
        text.Append(LONG_STRING);

        ref char ch = ref text[5];
        Assert.Equal('x', ch);
        ch = '#';
        Assert.Equal('#', ch);
        Assert.Equal('#', text[5]);
        Assert.Equal("Sphin# of Black Quartz, Judge My Vow.", text.ToString());

        ref char ch2 = ref text[^13];
        Assert.Equal('J', ch2);
        ch2 = 'j';
        Assert.Equal('j', ch2);
        Assert.Equal('j', text[^13]);
        Assert.Equal("Sphin# of Black Quartz, judge My Vow.", text.ToString());
    }

    [Fact]
    public void CanSpanRange()
    {
        using var text = TextBuilder.New;
        text.Append(LONG_STRING);

        Span<char> slice = text[24..29];
        Assert.Equal("Judge", slice.AsString());
        slice[0] = 'w';
        slice[1] = 'a';
        slice[2] = 't';
        slice[3] = 'c';
        slice[4] = 'h';
        Assert.Equal("watch", slice.AsString());
        Assert.Equal("Sphinx of Black Quartz, watch My Vow.", text.ToString());
    }

    [Fact]
    public void CanReadLength()
    {
        using var text = new TextBuilder();

        Assert.Equal(0, text.Length);
        text.Append(SHORT_STRING);
        Assert.Equal(3, text.Length);
        text.Append(SHORT_STRING);
        Assert.Equal(6, text.Length);
        text.RemoveLast(5);
        Assert.Equal(1, text.Length);
        text.Clear();
        Assert.Equal(0, text.Length);
    }

    [Fact]
    public void CanCreate()
    {
        {
            var text = new TextBuilder();
            text.Dispose();
        }
        {
            var text = TextBuilder.New;
            text.Dispose();
        }
        {
            var text = new TextBuilder(ushort.MaxValue);
            text.Dispose();
        }
    }

    [Fact]
    public void CanAppendChar()
    {
        using var text = new TextBuilder();
        text.Append('j');
        Assert.Equal("j", text.ToString());
        text.Append('x').Append('y').Append('z');
        Assert.Equal("jxyz", text.ToString());
        text.Clear();

        for (int i = char.MinValue; i <= char.MaxValue; i++)
        {
            text.Append((char)i);
        }

        Assert.Equal(char.MaxValue + 1, text.Length);
    }

    [Fact]
    public void CanAppendText()
    {
        using var text = new TextBuilder();

        ReadOnlySpan<char> first = "abc".AsSpan();
        ReadOnlySpan<char> second = "123".AsSpan();
        text.Append(first).Append(second);
        Assert.Equal("abc123", text.ToString());
    }

    [Fact]
    public void CanAppendString()
    {
        using var text = new TextBuilder();

        text.Append("eat");
        Assert.Equal("eat", text.ToString());

        text.Append(" ").Append("at").Append(" joes");
        Assert.Equal("eat at joes", text.ToString());
    }


    [Fact]
    public void CanAppendInterpolatedString()
    {
        // additional tests in InterpolatedTextBuilderTests.cs

        using var text = new TextBuilder();

        text.Append($"{'a'}_{"bc"}_{"def".AsSpan()}_{147.1311m:F2}");

        Assert.Equal("a_bc_def_147.13", text.ToString());
    }

    [Fact]
    public void CanAppendObject()
    {
        using var text = new TextBuilder();

        object? objNull = null;
        object? objChar = 'j';
        object? objDouble = 147.13d;
        object? objPair = Pair.New(SHORT_STRING, StringComparison.Ordinal);

        Assert.Equal(0, text.Length);
        text.Append<object>(objNull);
        Assert.Equal(0, text.Length);
        text.Append<object>(objChar);
        Assert.Equal(1, text.Length);
        Assert.Equal('j', text[0]);
        text.Append<object>(objDouble);
        Assert.Equal(7, text.Length);
        Assert.Equal("j147.13", text.ToString());
        text.Clear();

        text.Append<object>(objPair);
        Assert.Equal($"({SHORT_STRING}, Ordinal)", text.ToString());
    }

    [Fact]
    public void CanAppendValueFormat()
    {
        using var text = new TextBuilder();

        text.Append(147.13, "F1");
        Assert.Equal(5, text.Length);
        Assert.Equal("147.1", text.ToString());
        text.Clear();

        DateTime xmas = new DateTime(2000, 12, 25, 8,0,0,0, DateTimeKind.Local);
        text.Append(xmas, "yyyy-MM-dd HH:mm:ss");
        Assert.Equal("2000-12-25 08:00:00", text.ToString());
    }

    [Fact]
    public void CanNewLine()
    {
        using var text = new TextBuilder();

        text.Append("abc").NewLine();
        text.Append("123").NewLine();
        text.Append('x');

        Assert.Equal("""
            abc
            123
            x
            """, text.ToString());
    }

    [Fact]
    public void CanEnumerate()
    {
        using var text = new TextBuilder();

        IEnumerable<char> values = SHORT_STRING.ToCharArray();
        text.Enumerate<char>(values, static (tb, ch) => tb.Append(ch).Append('_'));
        Assert.Equal("T_R_J_", text.ToString());
        text.Clear();

        ReadOnlySpan<char> span = SHORT_STRING.AsSpan();
        text.Enumerate<char>(span, static (tb,ch) => tb.Append(ch).Append('_'));
        Assert.Equal("T_R_J_", text.ToString());
    }

    [Fact]
    public void CanIterate()
    {
        using var text = new TextBuilder();

        IEnumerable<char> values = SHORT_STRING.ToCharArray();
        text.Iterate<char>(values, static (tb, ch, i) => tb.Append(i).Append(ch));
        Assert.Equal("0T1R2J", text.ToString());
        text.Clear();

        ReadOnlySpan<char> span = SHORT_STRING.AsSpan();
        text.Iterate<char>(span, static (tb, ch, i) => tb.Append(i).Append(ch));
        Assert.Equal("0T1R2J", text.ToString());
    }

    [Fact]
    public void CanDelimit()
    {
        using var text = new TextBuilder();

        IEnumerable<char> values = SHORT_STRING.ToCharArray();
        text.Delimit<char>(',', values, static (tb, ch) => tb.Append(ch));
        Assert.Equal("T,R,J", text.ToString());
        text.Clear();
        text.Delimit<char>(", ", values, static (tb, ch) => tb.Append(ch));
        Assert.Equal("T, R, J", text.ToString());
        text.Clear();
        text.Delimit<char>(static tb => tb.Append('-'), values, static (tb, ch) => tb.Append(ch));
        Assert.Equal("T-R-J", text.ToString());
        text.Clear();

        ReadOnlySpan<char> span = SHORT_STRING.AsSpan();
        text.Delimit<char>(',', span, static (tb, ch) => tb.Append(ch));
        Assert.Equal("T,R,J", text.ToString());
        text.Clear();
        text.Delimit<char>(", ", span, static (tb, ch) => tb.Append(ch));
        Assert.Equal("T, R, J", text.ToString());
        text.Clear();
        text.Delimit<char>(static tb => tb.Append('-'), span, static (tb, ch) => tb.Append(ch));
        Assert.Equal("T-R-J", text.ToString());
        text.Clear();


    }
}
