using System.Linq.Expressions;

namespace ScrubJay.Text;

public readonly ref struct Delimiter
{
    public static implicit operator Delimiter(in char ch) => new Delimiter(in ch);
    public static implicit operator Delimiter(text text) => new Delimiter(text);
    public static implicit operator Delimiter(string? str) => new Delimiter(str);
    public static implicit operator Delimiter(Action<TextBuilder>? delimit) => new Delimiter(delimit);

    public static readonly Action<TextBuilder> NewLine = static tb => tb.NewLine();

    private readonly text _delimiter;
    private readonly Action<TextBuilder>? _delimit;

    public Delimiter(in char ch)
    {
        _delimiter = TextExtensions.AsSpan(in ch);
        _delimit = null;
    }

    public Delimiter(text text)
    {
        _delimiter = text;
        _delimit = null;
    }

#if NETFRAMEWORK || NETSTANDARD2_0
    public Delimiter(string? str)
    {
        _delimiter = str.AsSpan();
        _delimit = null;
    }
#endif

    public Delimiter(Action<TextBuilder>? delimit)
    {
        _delimiter = default;
        _delimit = delimit;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Invoke(TextBuilder builder)
    {
        if (_delimit is null)
        {
            builder.Write(_delimiter);
        }
        else
        {
            builder.Invoke(_delimit);
        }
    }
}