using ScrubJay.Memory;

#pragma warning disable CA1045

// ReSharper disable MergeCastWithTypeCheck

namespace ScrubJay.Text;

/// <summary>
/// Extensions on <see cref="SpanWriter{T}">FormatWriter</see>
/// </summary>
public static class FormatWriterExtensions
{
    public static bool TryWrite(this ref FormatWriter writer, char ch) 
        => writer.TryWrite(ch);

    public static bool TryWrite(this ref FormatWriter writer, scoped ReadOnlySpan<char> text) 
        => writer.TryWriteMany(text);

    public static bool TryWrite(this ref FormatWriter writer, string? str)
        => writer.TryWriteMany(str.AsSpan());

    public static bool TryWrite<T>(this ref FormatWriter writer, T? value)
    {
        string? str;
        // ReSharper disable once MergeCastWithTypeCheck
        if (value is IFormattable)
        {
            // If the value can format itself directly into our buffer, do so
            // ReSharper disable once MergeCastWithTypeCheck
            if (value is ISpanFormattable)
            {
                int charsWritten;
                // constrained call avoiding boxing for value types
                if (!((ISpanFormattable)value).TryFormat(writer.Available, out charsWritten, default, default))
                {
                    return false;
                }

                writer.Count += charsWritten;
                return true;
            }

            // constrained call avoiding boxing for value types
            str = ((IFormattable)value).ToString(null, null);
        }
        else
        {
            str = value?.ToString();
        }

        return writer.TryWriteMany(str.AsSpan());
    }

    public static bool TryWrite<T>(this ref FormatWriter writer, T? value, 
        scoped ReadOnlySpan<char> format,
        IFormatProvider? provider = null)
    {
        string? str;
        // ReSharper disable once MergeCastWithTypeCheck
        if (value is IFormattable)
        {
            // If the value can format itself directly into our buffer, do so
            // ReSharper disable once MergeCastWithTypeCheck
            if (value is ISpanFormattable)
            {
                int charsWritten;
                // constrained call avoiding boxing for value types
                if (!((ISpanFormattable)value).TryFormat(writer.Available, out charsWritten, format, provider))
                {
                    return false;
                }

                writer.Count += charsWritten;
                return true;
            }

            // constrained call avoiding boxing for value types
            str = ((IFormattable)value).ToString(format.AsString(), provider);
        }
        else
        {
            str = value?.ToString();
        }

        return writer.TryWriteMany(str.AsSpan());
    }
    
    public static bool TryWrite<T>(this ref FormatWriter writer, T? value, 
        string? format,
        IFormatProvider? provider = null)
    {
        string? str;
        // ReSharper disable once MergeCastWithTypeCheck
        if (value is IFormattable)
        {
            // If the value can format itself directly into our buffer, do so
            // ReSharper disable once MergeCastWithTypeCheck
            if (value is ISpanFormattable)
            {
                int charsWritten;
                // constrained call avoiding boxing for value types
                if (!((ISpanFormattable)value).TryFormat(writer.Available, out charsWritten, format.AsSpan(), provider))
                {
                    return false;
                }

                writer.Count += charsWritten;
                return true;
            }

            // constrained call avoiding boxing for value types
            str = ((IFormattable)value).ToString(format, provider);
        }
        else
        {
            str = value?.ToString();
        }

        return writer.TryWriteMany(str.AsSpan());
    }
}