#pragma warning disable CA1815, IDE0250

using System.Text;
using ScrubJay.Text.Rendering;
using ScrubJay.Text.Scratch;


namespace ScrubJay.Text;

/// <summary>
/// A custom <c>InterpolatedStringHandler</c> that supports everything in <see cref="DefaultInterpolatedStringHandler"/> as well as:<br/>
/// - Custom format codes
/// - Ability to append `ref struct` values
/// - Implicit conversion from <see cref="string"/> and <see cref="ReadOnlySpan{T}">ReadOnlySpan&lt;char&gt;</see>
/// </summary>
[PublicAPI]
[InterpolatedStringHandler]
public ref struct InterpolatedText
{
    /* The below implicit conversions from strings and ReadOnlySpan<char>s
     * use dangerous code to convert those initial values into a writable Span<char>
     * that can belong to InterpolatedText.
     * This is fine because InterpolatedStringHandlers are write-only:
     * - Text will be used exactly as-is
     * or
     * - As capacity is filled, further appends will grow by renting a new array and discarding this span
     */

    public static implicit operator InterpolatedText(text text)
    {
        if (text.Length == 0)
            return default;

        var span = Notsafe.Text.AsWritableSpan(text);
        return new InterpolatedText(span);
    }

    public static implicit operator InterpolatedText(string? str)
    {
        if (string.IsNullOrEmpty(str))
            return default;

        var span = Notsafe.Text.AsWritableSpan(str);
        return new InterpolatedText(span);
    }


    private Buffer<char> _buffer;

    public Span<char> Written => _buffer.Written;

    public int Length => _buffer.Count;

    private InterpolatedText(Span<char> span)
    {
        _buffer = new Buffer<char>(span, span.Length);
    }

    public InterpolatedText()
    {
        _buffer = default;
    }

    public InterpolatedText(int literalLength, int formattedCount)
    {
        _buffer = new Buffer<char>(literalLength + (formattedCount * 16));
    }

    public void AppendLiteral(string str) => _buffer.WriteMany(str.AsSpan());

    public void AppendFormatted(char ch) => _buffer.Write(ch);

    public void AppendFormatted(char ch, int alignment)
    {
        if (alignment == 0)
            return;

        if (alignment < 0)
        {
            // left align
            var span = _buffer.Allocate(-alignment);
            span[0] = ch;
            span[1..].Fill(' ');
        }
        else
        {
            // right align
            var span = _buffer.Allocate(alignment);
            span[..^1].Fill(' ');
            span[^1] = ch;
        }
    }

    public void AppendFormatted(string? str)
    {
        if (str is not null)
        {
            _buffer.WriteMany(str.AsSpan());
        }
    }

    public void AppendFormatted(string? str, int alignment)
    => AppendFormatted(str.AsSpan(), alignment);

    public void AppendFormatted(scoped text text)
    {
        _buffer.WriteMany(text);
    }

    public void AppendFormatted(scoped text text, int alignment)
    {
        if (alignment == 0)
            return;


        if (alignment < 0)
        {
            // left align
            var span = _buffer.Allocate(-alignment);
            if (text.Length > span.Length)
            {
                text[..span.Length].CopyTo(span);
                return;
            }

            span[..text.Length].CopyFrom(text);
            span[text.Length..].Fill(' ');
        }
        else
        {
            // right align
            var span = _buffer.Allocate(alignment);
            if (text.Length > span.Length)
            {
                text[^span.Length..].CopyTo(span);
                return;
            }

            span[..text.Length].Fill(' ');
            span[text.Length..].CopyFrom(text);
        }
    }



    public void AppendFormatted<T>(T? value)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        _buffer.WriteMany(value.Stringify().AsSpan());
    }

    public void AppendFormatted<T>(T value, scoped text format)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        if (format.Length == 0)
        {
            AppendFormatted<T>(value);
        }
        else if (format.Equate('@'))
        {
            // render
            _buffer.WriteMany(value.Render());
        }
        else
        {
            // no other valid formats?
            Debugger.Break();
            throw Ex.NotImplemented();
        }
    }

    public void AppendFormatted<T>(T value, string? format)
    {
        if (format.Equate('@'))
        {
            // render this value
            AppendLiteral(value.Render());
        }
        else if (value is IFormattable)
        {
            if (value is ISpanFormattable)
            {
                int charsWritten;
                while (!((ISpanFormattable)value).TryFormat(_buffer.Available, out charsWritten, format, null))
                {
                    _buffer.Grow();
                }

                _buffer.Count += charsWritten;
                return;
            }

            AppendLiteral(((IFormattable)value).ToString(format, null));
        }
        else if (value is not null)
        {
            AppendFormatted(value.ToString());
        }
    }

    public void Clear()
    {
        _buffer.Dispose();
    }

    public string ToStringAndClear()
    {
        string str = ToString();
        Clear();
        return str;
    }

    public override string ToString()
    {
        return _buffer.ToString();
    }
}