#pragma warning disable CA1045

// ReSharper disable MergeCastWithTypeCheck

namespace ScrubJay.Memory;

/// <summary>
/// Extensions on <see cref="Buffer{T}">Buffer&lt;char&gt;</see>
/// </summary>
public static class CharBufferExtensions
{
    public static void Write(this ref Buffer<char> buffer, char ch) => buffer.Add(ch);
    public static void Write(this ref Buffer<char> buffer, ReadOnlySpan<char> text) => buffer.AddMany(text);
    public static void Write(this ref Buffer<char> buffer, string? str) => buffer.AddMany(str.AsSpan());

    public static void Write<T>(this ref Buffer<char> buffer, T? value)
    {
#if NET6_0_OR_GREATER
        if (value is ISpanFormattable)
        {
            int written;
            while (!((ISpanFormattable)value).TryFormat(buffer.Available, out written, default, default))
            {
                buffer.Grow();
            }

            buffer.Count += written;
            return;
        }
#endif

        string? str;
        if (value is IFormattable)
        {
            str = ((IFormattable)value).ToString(default, default);
        }
        else
        {
            str = value?.ToString();
        }

        buffer.AddMany(str.AsSpan());
    }

    public static void Write<T>(this ref Buffer<char> buffer, T? value, ReadOnlySpan<char> format, IFormatProvider? provider = null)
    {
#if NET6_0_OR_GREATER
        if (value is ISpanFormattable)
        {
            int written;
            while (!((ISpanFormattable)value).TryFormat(buffer.Available, out written, format, provider))
            {
                buffer.Grow();
            }

            buffer.Count += written;
            return;
        }
#endif

        string? str;
        if (value is IFormattable)
        {
            str = ((IFormattable)value).ToString(format.ToString(), provider);
        }
        else
        {
            str = value?.ToString();
        }

        buffer.AddMany(str.AsSpan());
    }

    public static void Write<T>(this ref Buffer<char> buffer, T? value, string? format, IFormatProvider? provider = null)
    {
#if NET6_0_OR_GREATER
        if (value is ISpanFormattable)
        {
            int written;
            while (!((ISpanFormattable)value).TryFormat(buffer.Available, out written, format, provider))
            {
                buffer.Grow();
            }

            buffer.Count += written;
            return;
        }
#endif

        string? str;
        if (value is IFormattable)
        {
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
