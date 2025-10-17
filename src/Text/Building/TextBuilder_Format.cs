// ReSharper disable MergeCastWithTypeCheck

#if NET9_0_OR_GREATER
#endif
namespace ScrubJay.Text;

public partial class TextBuilder
{
#region Format

    public TextBuilder Format<T>(T? value)
    {
        if (value is IFormattable)
        {
#if NET6_0_OR_GREATER
            if (value is ISpanFormattable)
            {
                int charsWritten;
                while (!((ISpanFormattable)value).TryFormat(Available, out charsWritten, default, default))
                {
                    GrowBy(16);
                }

                _position += charsWritten;
                return this;
            }
#endif

            return Append(((IFormattable)value).ToString(default, default));
        }

        if (value is not null)
        {
            return Append(value.ToString());
        }

        return this;
    }

    public TextBuilder Format<T>(T? value, string? format, IFormatProvider? provider = null)
    {
        // special format codes for Rendering to support InterpolatedTextHandler

        // render this value
        if (format.Equate('@'))
        {
            return Rendering.Renderer.RenderValue<T>(this, value);
        }

        // render this value's type
        if (format.Equate("@T", StringComparison.OrdinalIgnoreCase))
        {
            return Rendering.Renderer.RenderValue<Type>(this, value?.GetType() ?? typeof(T));
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
                return this;
            }
#endif

            return Append(((IFormattable)value).ToString(format, provider));
        }

        if (value is not null)
        {
            return Append(value.ToString());
        }

        return this;
    }

    public TextBuilder Format<T>(T? value, scoped text format, IFormatProvider? provider = null)
    {
        // special format codes for Rendering
        // this is to support interpolated text

        // render this value
        if (format.Equate('@'))
        {
            return Rendering.Renderer.RenderValue<T>(this, value);

        }

        // render this value's type
        if (format.Equate("@T", StringComparison.OrdinalIgnoreCase))
        {
            return  Rendering.Renderer.RenderValue<Type>(this, value?.GetType() ?? typeof(T));
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
                return this;
            }
#endif

            return Append(((IFormattable)value).ToString(format.AsString(), provider));
        }

        if (value is not null)
        {
            return Append(value.ToString());
        }

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
                Format<T>(value);
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
                Format<T>(value, format, provider);
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
                Format<T>(value, format, provider);
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
                Format<T>(value);
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
                Format<T>(value, format, provider);
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
                Format<T>(value, format, provider);
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
                Format<T>(value, format, provider);
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
                Format<T>(value, format, provider);
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