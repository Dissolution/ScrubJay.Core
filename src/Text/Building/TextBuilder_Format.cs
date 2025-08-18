// ReSharper disable MergeCastWithTypeCheck


using ScrubJay.Text.Rendering;

namespace ScrubJay.Text;

public partial class TextBuilder
{
    internal void WriteValue<T>(T? value)
    {
        if (value is IFormattable)
        {
#if NET6_0_OR_GREATER
            if (value is ISpanFormattable)
            {
                int charsWritten;
                while (!((ISpanFormattable)value)
                       .TryFormat(Available, out charsWritten,
                           default, default))
                {
                    GrowBy(16);
                }

                _position += charsWritten;
                return;
            }
#endif

            Write(((IFormattable)value).ToString(default, default));
            return;
        }

        if (value is not null)
        {
            Write(value.ToString());
        }
    }

    internal void WriteValue<T>(T? value, string? format, IFormatProvider? provider)
    {
        // special format codes for Rendering
        // this is to support interpolated text
        if (format.Equate('@'))
        {
            RendererCache.RenderTo(this, value);
            return;
        }

        if (format.Equate("@T", StringComparison.OrdinalIgnoreCase))
        {
            RendererCache.RenderTo(this, value?.GetType() ?? typeof(T));
            return;
        }

        if (value is IFormattable)
        {
#if NET6_0_OR_GREATER
            if (value is ISpanFormattable)
            {
                int charsWritten;
                while (!((ISpanFormattable)value).TryFormat(Available, out charsWritten, format, provider))
                {
                    GrowBy(16);
                }

                _position += charsWritten;
                return;
            }
#endif

            Write(((IFormattable)value).ToString(format, provider));
            return;
        }

        if (value is not null)
        {
            Write(value.ToString());
        }

        return;
    }

    internal void WriteValue<T>(T? value, scoped text format, IFormatProvider? provider)
    {
        // special format codes for Rendering
        // this is to support interpolated text
        if (format.Equate('@'))
        {
            RendererCache.RenderTo(this, value);
            return;
        }

        if (format.Equate("@T", StringComparison.OrdinalIgnoreCase))
        {
            RendererCache.RenderTo(this, value?.GetType() ?? typeof(T));
            return;
        }

        if (value is IFormattable)
        {
#if NET6_0_OR_GREATER
            if (value is ISpanFormattable)
            {
                int charsWritten;
                while (!((ISpanFormattable)value).TryFormat(Available, out charsWritten, format, provider))
                {
                    GrowBy(16);
                }

                _position += charsWritten;
                return;
            }
#endif

            Write(((IFormattable)value).ToString(format.AsString(), provider));
            return;
        }

        if (value is not null)
        {
            Write(value.ToString());
        }

        return;
    }


#region Format

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TextBuilder Format<T>(T? value)
    {
        WriteValue<T>(value);
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TextBuilder Format<T>(T? value, string? format, IFormatProvider? provider = null)
    {
        WriteValue<T>(value, format, provider);
        return this;
    }

    public TextBuilder Format<T>(T? value, scoped text format, IFormatProvider? provider = null)
    {
        WriteValue<T>(value, format, provider);
        return this;
    }

#endregion

#region FormatMany

    public TextBuilder FormatMany<T>(params ReadOnlySpan<T?> values)
    {
        if (!values.IsEmpty)
        {
            foreach (T? value in values)
            {
                WriteValue<T>(value);
            }
        }

        return this;
    }

    public TextBuilder FormatMany<T>(scoped ReadOnlySpan<T?> values, string? format, IFormatProvider? provider = null)
    {
        if (!values.IsEmpty)
        {
            foreach (T? value in values)
            {
                WriteValue<T>(value, format, provider);
            }
        }

        return this;
    }

    public TextBuilder FormatMany<T>(scoped ReadOnlySpan<T?> values, scoped text format, IFormatProvider? provider = null)
    {
        if (!values.IsEmpty)
        {
            foreach (T? value in values)
            {
                WriteValue<T>(value, format, provider);
            }
        }

        return this;
    }

    public TextBuilder FormatMany<T>(IEnumerable<T?>? values)
    {
        if (values is not null)
        {
            foreach (T? value in values)
            {
                WriteValue<T>(value);
            }
        }

        return this;
    }

    public TextBuilder FormatMany<T>(IEnumerable<T?>? values, string? format, IFormatProvider? provider = null)
    {
        if (values is not null)
        {
            foreach (T? value in values)
            {
                WriteValue<T>(value, format, provider);
            }
        }

        return this;
    }

    public TextBuilder FormatMany<T>(IEnumerable<T?>? values, scoped text format, IFormatProvider? provider = null)
    {
        if (values is not null)
        {
            foreach (T? value in values)
            {
                WriteValue<T>(value, format, provider);
            }
        }

        return this;
    }

#endregion

#region FormatMany with Delimiter
    //
    // public TextBuilder AppendMany(scoped text text, Delimiter delimiter)
    // {
    //     if (!text.IsEmpty)
    //     {
    //         Write(text[0]);
    //         for (var i = 1; i < text.Length; i++)
    //         {
    //             delimiter.Invoke(this);
    //             Write(text[i]);
    //         }
    //     }
    //
    //     return this;
    // }
    //
    // public TextBuilder AppendMany(IEnumerable<char>? characters, Delimiter delimiter)
    // {
    //     if (characters is not null)
    //     {
    //         using var e = characters.GetEnumerator();
    //         if (!e.MoveNext())
    //             return this;
    //
    //         Write(e.Current);
    //         while (e.MoveNext())
    //         {
    //             delimiter.Invoke(this);
    //             Write(e.Current);
    //         }
    //     }
    //
    //     return this;
    // }
    //
    // public TextBuilder AppendMany(IEnumerable<string?>? strings, Delimiter delimiter)
    // {
    //     if (strings is not null)
    //     {
    //         using var e = strings.GetEnumerator();
    //         if (!e.MoveNext())
    //             return this;
    //
    //         Write(e.Current);
    //         while (e.MoveNext())
    //         {
    //             delimiter.Invoke(this);
    //             Write(e.Current);
    //         }
    //     }
    //
    //     return this;
    // }

#endregion

#region FormatLine

    public TextBuilder FormatLine<T>(T? value) => Format<T>(value).NewLine();

    public TextBuilder FormatLine<T>(T? value, string? format, IFormatProvider? provider = null)
        => Format<T>(value, format, provider).NewLine();

    public TextBuilder FormatLine<T>(T? value, scoped text format, IFormatProvider? provider = null)
        => Format<T>(value, format, provider).NewLine();

#endregion
}