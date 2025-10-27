// ReSharper disable MergeCastWithTypeCheck

#if NET9_0_OR_GREATER
#endif
namespace ScrubJay.Text;

/* Format operations use IFormattable and ISpanFormattable functionality
 * There are some special format codes:
 * @    - Renders this value (rather than Append)
 * @T   - Renders this value's Type
 */

public partial class TextBuilder
{
#region Format T

    public TextBuilder Format<T>(T? value)
    {
        if (value is IFormattable)
        {
#if NET6_0_OR_GREATER
            if (value is ISpanFormattable)
            {
                int charsWritten;
                while (!((ISpanFormattable)value).TryFormat(Available, out charsWritten, default, null))
                {
                    GrowBy(16);
                }

                _position += charsWritten;
                return this;
            }
#endif

            return Append(((IFormattable)value).ToString(null, null));
        }

        if (value is not null)
        {
            return Append(value.ToString());
        }

        return this;
    }

    public TextBuilder Format<T>(T? value,
        string? format,
        IFormatProvider? provider = null)
    {
        if (format.Equate('@'))
            return Render<T>(value);
        if (format.Equate("@T"))
            return Render<Type>(Type.GetType<T>(value));

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
        if (format.Equate('@'))
            return Render<T>(value);
        if (format.Equate("@T"))
            return Render<Type>(Type.GetType<T>(value));

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

#region Format Object

    public TextBuilder Format(object? obj)
    {
        if (obj is IFormattable formattable)
        {
#if NET6_0_OR_GREATER
            if (obj is ISpanFormattable spanFormattable)
            {
                int charsWritten;
                while (!spanFormattable.TryFormat(Available, out charsWritten, default, null))
                {
                    GrowBy(16);
                }

                _position += charsWritten;
                return this;
            }
#endif

            return Append(formattable.ToString(null, null));
        }

        if (obj is not null)
        {
            return Append(obj.ToString());
        }

        return this;
    }

    public TextBuilder Format(object? obj, string? format, IFormatProvider? provider = null)
    {
        if (format.Equate('@'))
            return Render<object>(obj);
        if (format.Equate("@T"))
            return Render<Type>(obj?.GetType() ?? typeof(object));

        if (obj is IFormattable formattable)
        {
#if NET6_0_OR_GREATER
            if (obj is ISpanFormattable spanFormattable)
            {
                int charsWritten;
                while (!spanFormattable.TryFormat(Available, out charsWritten, format, provider))
                {
                    GrowBy(16);
                }

                _position += charsWritten;
                return this;
            }
#endif

            return Append(formattable.ToString(format, provider));
        }

        if (obj is not null)
        {
            return Append(obj.ToString());
        }

        return this;
    }

    public TextBuilder Format(object? obj, scoped text format, IFormatProvider? provider = null)
    {
        if (format.Equate('@'))
            return Render<object>(obj);
        if (format.Equate("@T"))
            return Render<Type>(obj?.GetType() ?? typeof(object));

        if (obj is IFormattable formattable)
        {
#if NET6_0_OR_GREATER
            if (obj is ISpanFormattable spanFormattable)
            {
                int charsWritten;
                while (!spanFormattable.TryFormat(Available, out charsWritten, format, provider))
                {
                    GrowBy(16);
                }

                _position += charsWritten;
                return this;
            }
#endif

            return Append(formattable.ToString(format.AsString(), provider));
        }

        if (obj is not null)
        {
            return Append(obj.ToString());
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