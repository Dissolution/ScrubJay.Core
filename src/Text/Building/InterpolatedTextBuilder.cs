// Members can be made readonly
// ReSharper disable NotDisposedResource

#pragma warning disable IDE0250, IDE0251, CA1001

namespace ScrubJay.Text;

/// <summary>
/// An InterpolatedStringHandler that writes to a <see cref="TextBuilder"/>
/// </summary>
[PublicAPI]
[InterpolatedStringHandler]
[MustDisposeResource(false)]
public ref struct InterpolatedTextBuilder
{
    public static implicit operator InterpolatedTextBuilder(string? str)
    {
        if (str is not null)
        {
            TextBuilder builder = new TextBuilder().Append(str);
            return new InterpolatedTextBuilder(builder, true);
        }
        else
        {
            return default;
        }
    }

    private TextBuilder? _builder;
    private bool _disposeBuilder;

    private InterpolatedTextBuilder(TextBuilder? builder, bool disposeBuilder)
    {
        _builder = builder;
        _disposeBuilder = disposeBuilder;
    }

    [MustDisposeResource(false)]
    public InterpolatedTextBuilder()
    {
        _builder = null;
        _disposeBuilder = false;
    }

    [MustDisposeResource(true)]
    public InterpolatedTextBuilder(int literalLength, int formattedCount)
    {
        _builder = new TextBuilder(literalLength + (formattedCount * 16));
        _disposeBuilder = true;
    }

    [MustDisposeResource(false)]
    public InterpolatedTextBuilder(TextBuilder builder)
    {
        _builder = builder;
        _disposeBuilder = false;
    }

    [MustDisposeResource(false)]
    public InterpolatedTextBuilder(int literalLength, int formattedCount, TextBuilder builder)
    {
        _builder = builder;
        _disposeBuilder = false;
    }

    public void AppendLiteral(string str)
    {
        Debug.Assert(str is not null);
        _builder ??= new(str!.Length);
        _builder.Write(str);
    }

    public void AppendFormatted(char ch)
    {
        _builder ??= new(1);
        _builder.Write(ch);
    }

    public void AppendFormatted(char ch, int alignment)
    {
        _builder ??= new(alignment);
        _builder.Align(ch, width: alignment);
    }

    public void AppendFormatted(scoped text text)
    {
        _builder ??= new(text.Length);
        _builder.Write(text);
    }

    public void AppendFormatted(scoped text text, int alignment)
    {
        _builder ??= new(alignment);
        _builder.Align(text, width: alignment);
    }

    public void AppendFormatted(string? str)
    {
        if (str is not null)
        {
            _builder ??= new(str.Length);
            _builder.Write(str);
        }
    }

    public void AppendFormatted(string? str, int alignment)
    {
        if (str is not null)
        {
            _builder ??= new(str.Length);
            _builder.Align(str, width: alignment);
        }
    }

    public void AppendFormatted<T>(T? value)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        if (value is not null)
        {
            _builder ??= new();
            string str = TextHelper.ToString(value);
            _builder.Write(str);
        }
    }

    public void AppendFormatted<T>(T? value, string? format)
    {
        if (value is not null)
        {
            _builder ??= new();
            _builder.Format<T>(value, format);
        }
    }


    public void AppendFormatted<T>(T? value, scoped text format)
    {
        if (value is not null)
        {
            _builder ??= new();
            _builder.Format<T>(value, format);
        }
    }


    public void AppendFormatted<T>(T? value, int alignment)
    {
        if (value is not null)
        {
            _builder ??= new();
            _builder.AlignFormat<T>(value, width: alignment);
        }
    }


    public void AppendFormatted<T>(T? value, int alignment, string? format)
    {
        if (value is not null)
        {
            _builder ??= new();
            _builder.AlignFormat<T>(value, alignment, format);
        }
    }

    public void AppendFormatted(Action<TextBuilder>? build)
    {
        if (build is not null)
        {
            _builder ??= new();
            _builder.Invoke(build);
        }
    }

    public void Dispose()
    {
        if (_disposeBuilder && _builder is not null)
        {
            _builder.Dispose();
        }
    }

    public string ToStringAndDispose()
    {
        string str = ToString();
        Dispose();
        return str;
    }

    public override string ToString()
    {
        if (_builder is not null)
            return _builder.ToString();
        return string.Empty;
    }
}