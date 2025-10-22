// Members can be made readonly
// ReSharper disable NotDisposedResource

using ScrubJay.Text.Rendering;

#pragma warning disable IDE0250, IDE0251, CA1001

namespace ScrubJay.Text;

/// <summary>
/// An InterpolatedStringHandler that writes to a <see cref="TextBuilder"/>
/// </summary>
/// <remarks>
/// Inspired by
/// </remarks>
[PublicAPI]
[InterpolatedStringHandler]
public ref struct InterpolatedText
{
    /// <summary>
    /// Implicitly convert <see cref="text"/> to an <see cref="InterpolatedText"/> containing it
    /// </summary>
    public static implicit operator InterpolatedText(text text)
    {
        if (text.Length == 0)
            return default;

        // yes, this is dangerous
        // but InterpolatedStringHandlers are write-only
        // so either this value is used as-is
        // or a new array is acquired upon a new Append and this Span goes out of scope
        var buffer = Notsafe.Text.AsWritableSpan(text);
        return new InterpolatedText(null, new(buffer, buffer.Length), true);
    }

    public static implicit operator InterpolatedText(string? str)
    {
        if (string.IsNullOrEmpty(str))
            return default;

        var buffer = Notsafe.Text.AsWritableSpan(str);
        return new InterpolatedText(null, new(buffer, buffer.Length), true);
    }

    private readonly TextBuilder? _builder;
    private Buffer<char> _buffer;
    private bool _disposeBuffer;

    public int Length
    {
        get
        {
            if (_builder is not null)
                return _builder.Length;
            return _buffer.Count;
        }
    }

    private InterpolatedText(TextBuilder? builder, Buffer<char> buffer, bool disposeBuffer)
    {
        _builder = builder;
        _buffer = buffer;
        _disposeBuffer = disposeBuffer;
    }

    public InterpolatedText()
        : this(null, default, true)
    {
    }

    public InterpolatedText(TextBuilder builder)
        : this(builder, default, false)
    {
    }

    public InterpolatedText(Span<char> buffer)
        : this(null, buffer, true)
    {
    }

    public InterpolatedText(Buffer<char> buffer)
        : this(null, buffer, false)
    {
    }

    public InterpolatedText(int literalLength, int formattedCount)
        : this(null, new(literalLength + (formattedCount * 16)), true)
    {
    }

    public InterpolatedText(int literalLength, int formattedCount, TextBuilder builder)
        : this(builder, default, false)
    {
    }

    public InterpolatedText(int literalLength, int formattedCount, Span<char> buffer)
        : this(null, buffer, true)
    {
    }

    public InterpolatedText(int literalLength, int formattedCount, Buffer<char> buffer)
        : this(null, buffer, false)
    {
    }

    public void AppendLiteral(string str)
    {
        Debug.Assert(str is not null);
        if (_builder is not null)
        {
            _builder.Write(str);
        }
        else
        {
            _buffer.WriteMany(str.AsSpan());
        }
    }

    public void AppendFormatted(char ch)
    {
        if (_builder is not null)
        {
            _builder.Write(ch);
        }
        else
        {
            _buffer.Write(ch);
        }
    }

    public void AppendFormatted(char ch, int alignment)
    {
        if (alignment == 0) return;

        if (_builder is not null)
        {
            _builder.Align(ch, width: alignment);
        }
        else
        {
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
    }


    public void AppendFormatted(scoped text text)
    {
        if (_builder is not null)
        {
            _builder.Write(text);
        }
        else
        {
            _buffer.WriteMany(text);
        }
    }

    public void AppendFormatted(scoped text text, int alignment)
    {
        if (alignment == 0) return;

        if (_builder is not null)
        {
            _builder.Align(text, width: alignment);
        }
        else
        {
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
    }

    public void AppendFormatted(string? str)
    {
        if (_builder is not null)
        {
            _builder.Write(str);
        }
        else
        {
            _buffer.WriteMany(str.AsSpan());
        }
    }

    public void AppendFormatted(string? str, int alignment)
    {
        if (alignment == 0) return;

        if (_builder is not null)
        {
            _builder.Align(str, width: alignment);
            return;
        }

        if (string.IsNullOrEmpty(str))
        {
            _buffer.Allocate(Math.Abs(alignment)).Fill(' ');
            return;
        }

        if (alignment < 0)
        {
            // left align
            var span = _buffer.Allocate(-alignment);
            if (str!.Length > span.Length)
            {
                str[..span.Length].CopyTo(span);
                return;
            }

            span[..str.Length].CopyFrom(str);
            span[str.Length..].Fill(' ');
        }
        else
        {
            // right align
            var span = _buffer.Allocate(alignment);
            if (str!.Length > span.Length)
            {
                str[^span.Length..].CopyTo(span);
                return;
            }

            span[..str.Length].Fill(' ');
            span[str.Length..].CopyFrom(str);
        }
    }

    public void AppendFormatted<T>(T? value)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        if (_builder is not null)
        {
            _builder.Render<T>(value);
        }
        else
        {
            _buffer.WriteMany(TextHelper.ToString<T>(value));
        }
    }

    public void AppendFormatted<T>(T? value, string? format)
    {
        if (_builder is not null)
        {
            _builder.Format<T>(value, format);
        }
        // render this value
        else if (format.Equate('@'))
        {
            _buffer.WriteMany(value.Render());
        }
        // render this value's type
        else if (format.Equate("@T", StringComparison.OrdinalIgnoreCase))
        {
            _buffer.WriteMany((value?.GetType() ?? typeof(T)).Render());
        }
        else if (value is IFormattable)
        {
#if NET6_0_OR_GREATER
            if (value is ISpanFormattable)
            {
                int charsWritten;
                while (!((ISpanFormattable)value).TryFormat(_buffer.Available, out charsWritten, format, null))
                {
                    _buffer.GrowBy(16);
                }

                _buffer.Count += charsWritten;
                return;
            }
#endif

            _buffer.WriteMany(((IFormattable)value).ToString(format, null));
        }
        else if (value is not null)
        {
            _buffer.WriteMany(value.ToString());
        }
    }


    public void AppendFormatted<T>(T? value, scoped text format)
    {
        if (_builder is not null)
        {
            _builder.Format<T>(value, format);
        }
        // render this value
        else if (format.Equate('@'))
        {
            _buffer.WriteMany(value.Render());
        }
        // render this value's type
        else if (format.Equate("@T", StringComparison.OrdinalIgnoreCase))
        {
            _buffer.WriteMany((value?.GetType() ?? typeof(T)).Render());
        }
        else if (value is IFormattable)
        {
#if NET6_0_OR_GREATER
            if (value is ISpanFormattable)
            {
                int charsWritten;
                while (!((ISpanFormattable)value).TryFormat(_buffer.Available, out charsWritten, format, null))
                {
                    _buffer.GrowBy(16);
                }

                _buffer.Count += charsWritten;
                return;
            }
#endif

            _buffer.WriteMany(((IFormattable)value).ToString(format.AsString(), null));
        }
        else if (value is not null)
        {
            _buffer.WriteMany(value.ToString());
        }
    }
    //
    //
    // public void AppendFormatted<T>(T? value, int alignment)
    // {
    //     if (value is not null)
    //     {
    //         _builder ??= new();
    //         _builder.AlignFormat<T>(value, width: alignment);
    //     }
    // }
    //
    //
    // public void AppendFormatted<T>(T? value, int alignment, string? format)
    // {
    //     if (value is not null)
    //     {
    //         _builder ??= new();
    //         _builder.AlignFormat<T>(value, alignment, format);
    //     }
    // }

    //[HandlesResourceDisposal]
    public void Dispose()
    {
        // never dispose Builder, it was only ever passed in, never created
        if (_disposeBuffer)
        {
            _buffer.Dispose();
        }
    }

    //[HandlesResourceDisposal]
    public string ToStringAndDispose()
    {
        string str = ToString();
        Dispose();
        return str;
    }

    public override string ToString()
    {
        if (_builder is not null)
        {
            return _builder.ToString();
        }

        return _buffer.ToString();
    }
}