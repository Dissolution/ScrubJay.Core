using ScrubJay.Pooling;
#pragma warning disable CA1045

// ReSharper disable MergeCastWithTypeCheck

namespace ScrubJay.Text;

/// <summary>
/// Extensions on <see cref="Buffer{T}">Buffer&lt;char&gt;</see>
/// </summary>
public static class CharBufferExtensions
{
    public static void Write(this ref Buffer<char> buffer, char ch) => buffer.Add(ch);
    
    public static void Write(this ref Buffer<char> buffer, scoped text text) => buffer.AddMany(text);

    public static void Write(this ref Buffer<char> buffer, string? str)
    {
        if (str is not null)
        {
            buffer.AddMany(str.AsSpan());
        }
    }

    public static void Write<T>(this ref Buffer<char> buffer, T? value)
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
                while (!((ISpanFormattable)value).TryFormat(buffer.Available, out charsWritten, default, default))
                {
                    buffer.Grow();
                }
                buffer.Count += charsWritten;
                return;
            }

            // constrained call avoiding boxing for value types
            str = ((IFormattable)value).ToString(null, null);
        }
        else
        {
            str = value?.ToString();
        }

        buffer.AddMany(str.AsSpan());
    }

    public static void Write<T>(this ref Buffer<char> buffer, T? value, text format, IFormatProvider? provider = null)
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
                while (!((ISpanFormattable)value).TryFormat(buffer.Available, out charsWritten, format, provider))
                {
                    buffer.Grow();
                }
                buffer.Count += charsWritten;
                return;
            }

            // constrained call avoiding boxing for value types
            str = ((IFormattable)value).ToString(format.AsString(), provider);
        }
        else
        {
            str = value?.ToString();
        }

        buffer.AddMany(str.AsSpan());
    }

    public static void Write<T>(this ref Buffer<char> buffer, T? value, string? format, IFormatProvider? provider = null)
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
                while (!((ISpanFormattable)value).TryFormat(buffer.Available, out charsWritten, format.AsSpan(), provider))
                {
                    buffer.Grow();
                }
                buffer.Count += charsWritten;
                return;
            }

            // constrained call avoiding boxing for value types
            str = ((IFormattable)value).ToString(format, provider);
        }
        else
        {
            str = value?.ToString();
        }

        buffer.AddMany(str.AsSpan());
    }

    public static string ToStringAndDispose(
        [HandlesResourceDisposal]
        this ref Buffer<char> buffer)
    {
        string str = buffer.Written.ToString();
        buffer.Dispose();
        return str;
    }
}
