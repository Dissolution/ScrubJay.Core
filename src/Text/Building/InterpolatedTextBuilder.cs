namespace ScrubJay.Text;

/// <summary>
/// Provides a handler used to append interpolated strings into <see cref="TextBuilder"/> instances.
/// </summary>
/// <remarks>
/// Heavily inspired by <see cref="DefaultInterpolatedStringHandler"/> and <see cref="System.Text.TextBuilder.AppendInterpolatedStringHandler"/>
/// </remarks>
[PublicAPI]
[InterpolatedStringHandler]
public struct InterpolatedTextBuilder
{
    internal TextBuilder _builder;


    public InterpolatedTextBuilder(int literalLength, int formattedCount, TextBuilder builder)
    {
        Throw.IfNull(builder);
        _builder = builder;
    }

    public void AppendLiteral(string str) => _builder.Write(str);

    public void AppendFormatted(char ch) => _builder.Write(ch);

    public void AppendFormatted(char ch, int alignment) => _builder.Align(ch, alignment);

    public void AppendFormatted(string? str) => _builder.Write(str);

    public void AppendFormatted(string? str, int alignment) => _builder.Align(str, alignment);

    public void AppendFormatted(scoped text text) => _builder.Write(text);

    public void AppendFormatted(scoped text text, int alignment) => _builder.Align(text, alignment);

    public void AppendFormatted<T>(T value)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        _builder.Append<T>(value);
    }

    public void AppendFormatted<T>(T value, scoped text format)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        if (format.Equate('@'))
        {
            // render this value
            _builder.Render<T>(value);
        }
        else
        {
            // no other valid formats?
            throw Ex.NotImplemented();
        }
    }

    public void AppendFormatted<T>(T value, string? format)
    {
        if (format.Equate('@'))
        {
            // render this value
            _builder.Render<T>(value);
        }
        else
        {
            // no other valid formats?
            throw Ex.NotImplemented();
        }
    }

    /*
    public void AppendFormatted<T>(T value, string? format)
    {
        _builder.Format<T>(value, format);
    }

    public void AppendFormatted<T>(T value, int alignment) =>
        AppendFormatted(value, alignment, format: null);

    public void AppendFormatted<T>(T value, int alignment, string? format)
    {
        if (alignment == 0)
        {
            _builder.Format<T>(value, format);
        }
        else
        {
            _builder.AlignFormat<T>(value, alignment, format);
        }
    }
    */
}