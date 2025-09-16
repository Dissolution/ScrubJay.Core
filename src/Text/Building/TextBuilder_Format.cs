// ReSharper disable MergeCastWithTypeCheck

#if NET9_0_OR_GREATER
using ScrubJay.Core.Utilities;
#endif
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
    }


#region Format

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TextBuilder Format<T>(T? value)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
#if NET9_0_OR_GREATER
        string str = Any<T>.ToString(value);
        Write(str);
#else
        WriteValue<T>(value);
#endif
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

    public TextBuilder FormatMany<T>(params ReadOnlySpan<T> values)
    {
        if (!values.IsEmpty)
        {
            foreach (var value in values)
            {
                WriteValue<T>(value);
            }
        }

        return this;
    }

    public TextBuilder FormatMany<T>(scoped ReadOnlySpan<T> values, string? format, IFormatProvider? provider = null)
    {
        if (!values.IsEmpty)
        {
            foreach (var value in values)
            {
                WriteValue<T>(value, format, provider);
            }
        }

        return this;
    }

    public TextBuilder FormatMany<T>(scoped ReadOnlySpan<T> values, scoped text format, IFormatProvider? provider = null)
    {
        if (!values.IsEmpty)
        {
            foreach (var value in values)
            {
                WriteValue<T>(value, format, provider);
            }
        }

        return this;
    }

    public TextBuilder FormatMany<T>(IEnumerable<T>? values)
    {
        if (values is not null)
        {
            foreach (var value in values)
            {
                WriteValue<T>(value);
            }
        }

        return this;
    }

    public TextBuilder FormatMany<T>(IEnumerable<T>? values, string? format, IFormatProvider? provider = null)
    {
        if (values is not null)
        {
            foreach (var value in values)
            {
                WriteValue<T>(value, format, provider);
            }
        }

        return this;
    }

    public TextBuilder FormatMany<T>(IEnumerable<T>? values, scoped text format, IFormatProvider? provider = null)
    {
        if (values is not null)
        {
            foreach (var value in values)
            {
                WriteValue<T>(value, format, provider);
            }
        }

        return this;
    }

    public TextBuilder FormatMany<T>(Func<Option<T>>? iterator, string? format, IFormatProvider? provider = null)
    {
        if (iterator is not null)
        {
            while (iterator().IsSome(out var value))
            {
                WriteValue<T>(value, format, provider);
            }
        }

        return this;
    }

    public TextBuilder FormatMany<T>(Func<Option<T>>? iterator, scoped text format, IFormatProvider? provider = null)
    {
        if (iterator is not null)
        {
            while (iterator().IsSome(out var value))
            {
                WriteValue<T>(value, format, provider);
            }
        }

        return this;
    }

#endregion

#region FormatLine

    public TextBuilder FormatLine<T>(T? value) => Format<T>(value).NewLine();

    public TextBuilder FormatLine<T>(T? value, string? format, IFormatProvider? provider = null)
        => Format<T>(value, format, provider).NewLine();

    public TextBuilder FormatLine<T>(T? value, scoped text format, IFormatProvider? provider = null)
        => Format<T>(value, format, provider).NewLine();

#endregion
}