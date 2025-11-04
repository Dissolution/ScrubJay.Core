using System.Text;
using ScrubJay.Text.Rendering;

#pragma warning disable CA1815, IDE0250

namespace ScrubJay.Text;

/// <summary>
/// Provides a handler used to append interpolated strings into <see cref="TextBuilder"/> instances.
/// </summary>
/// <remarks>
/// Heavily inspired by <see cref="DefaultInterpolatedStringHandler"/> and <see cref="StringBuilder.AppendInterpolatedStringHandler"/>
/// </remarks>
[PublicAPI]
[InterpolatedStringHandler]
public ref struct InterpolatedTextBuilder
{
    public static implicit operator InterpolatedTextBuilder(string? str)
    {
        if (str is null) return default;
        var builder = new InterpolatedTextBuilder(str.Length, 0);
        builder.AppendLiteral(str);
        return builder;
    }

    internal readonly TextBuilder? _builder;
    internal Buffer<char> _buffer;

    public int Length
    {
        get
        {
            if (_builder is null)
                return _buffer.Count;
            return _builder.Length;
        }
    }

    public InterpolatedTextBuilder()
    {
        _builder = null;
        _buffer = new Buffer<char>();
    }

    public InterpolatedTextBuilder(int literalLength, int formattedCount)
    {
        _builder = null;
        _buffer = new Buffer<char>(literalLength + (formattedCount * 16));
    }

    public InterpolatedTextBuilder(int literalLength, int formattedCount, TextBuilder builder)
    {
        Throw.IfNull(builder);
        _builder = builder;
        _buffer = default;
    }

    public void AppendLiteral(string str)
    {
        if (_builder is null)
        {
            _buffer.WriteMany(str.AsSpan());
        }
        else
        {
            _builder.Write(str);
        }
    }

    public void AppendFormatted(char ch)
    {
        if (_builder is null)
        {
            _buffer.Write(ch);
        }
        else
        {
            _builder.Write(ch);
        }
    }

    public void AppendFormatted(char ch, int alignment)
    {
        if (_builder is null)
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
        else
        {
            _builder.Align(ch, alignment);
        }
    }

    public void AppendFormatted(string? str)
    {
        if (_builder is null)
        {
            _buffer.WriteMany(str.AsSpan());
        }
        else
        {
            _builder.Write(str);
        }
    }

    public void AppendFormatted(string? str, int alignment)
        => AppendFormatted(str.AsSpan(), alignment);

    public void AppendFormatted(scoped text text)
    {
        if (_builder is null)
        {
            _buffer.WriteMany(text);
        }
        else
        {
            _builder.Write(text);
        }
    }


    public void AppendFormatted(scoped text text, int alignment)
    {
        if (_builder is null)
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
        else
        {
            _builder.Align(text, alignment);
        }
    }

    public void AppendFormatted<T>(T? value)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        if (_builder is null)
        {
            _buffer.WriteMany(value.Stringify().AsSpan());
        }
        else
        {
            _builder.Append<T>(value);
        }
    }

    public void AppendFormatted<T>(T? value, scoped text format)
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
            if (_builder is null)
            {
                _buffer.WriteMany(value.Render());
            }
            else
            {
                _builder.Render(value);
            }
        }
        else
        {
            // no other valid formats?
            Debugger.Break();
            throw Ex.NotImplemented();
        }
    }

    public void AppendFormatted<T>(T? value, string? format)
    {
        if (_builder is null)
        {
            if (format.Equate('@'))
            {
                // render this value
                _buffer.WriteMany(value.Render());
            }
            else if (value is IFormattable)
            {
#if NET6_0_OR_GREATER
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
#endif

                _buffer.WriteMany(((IFormattable)value).ToString(format, null));
            }
            else if (value is not null)
            {
                _buffer.WriteMany(value.ToString());
            }
        }
        else
        {
            _builder.Format<T>(value, format);
        }
    }

    [HandlesResourceDisposal]
    public void Clear()
    {
        if (_builder is null)
        {
            _buffer.Dispose();
        }
        else
        {
            Debug.Assert(_buffer.Count == 0);
        }
    }

    [HandlesResourceDisposal]
    public string ToStringAndClear()
    {
        string str = this.ToString();
        this.Clear();
        return str;
    }

    public Span<char> AsSpan()
    {
        if (_builder is null)
        {
            return _buffer.Written;
        }
        else
        {
            return _builder.Written;
        }
    }

    public override string ToString()
    {
        if (_builder is null)
        {
            return _buffer.Written.AsString();
        }
        else
        {
            return _builder.ToString();
        }
    }
}