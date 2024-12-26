#pragma warning disable CA1045

// ReSharper disable MergeCastWithTypeCheck

namespace ScrubJay.Memory;

/// <summary>
/// Extensions on <see cref="SpanWriter{T}">FormatWriter</see>
/// </summary>
public static class FormatWriterExtensions
{
    public static bool TryWrite(this ref FormatWriter spanWriter, string? str) => spanWriter.TryWriteMany(str.AsSpan());

#pragma warning disable S3247
    public static bool TryWrite<T>(this ref FormatWriter spanWriter, T? value)
    {
        var avail = spanWriter.Available;
#if NET6_0_OR_GREATER
        if (value is ISpanFormattable)
        {
            if (!((ISpanFormattable)value).TryFormat(avail, out int written, default, default))
                return false;
            spanWriter.Count += written;
            return true;
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

        if (str is null)
            return true;

        if (!str.TryCopyTo(avail))
            return false;

        spanWriter.Count += str.Length;
        return true;
    }

    public static bool TryWriteFormatted<T>(this ref FormatWriter spanWriter, T? value, ReadOnlySpan<char> format, IFormatProvider? provider = null)
    {
        var avail = spanWriter.Available;
#if NET6_0_OR_GREATER
        if (value is ISpanFormattable)
        {
            if (!((ISpanFormattable)value).TryFormat(avail, out int written, format, provider))
                return false;
            spanWriter.Count += written;
            return true;
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

        if (str is null)
            return true;

        if (!str.TryCopyTo(avail))
            return false;

        spanWriter.Count += str.Length;
        return true;
    }

    public static bool TryWriteFormatted<T>(this ref FormatWriter spanWriter, T? value, string? format, IFormatProvider? provider = null)
    {
        var avail = spanWriter.Available;
#if NET6_0_OR_GREATER
        if (value is ISpanFormattable)
        {
            if (!((ISpanFormattable)value).TryFormat(avail, out int written, format.AsSpan(), provider))
                return false;
            spanWriter.Count += written;
            return true;
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

        if (str is null)
            return true;

        if (!str.TryCopyTo(avail))
            return false;

        spanWriter.Count += str.Length;
        return true;
    }
#pragma warning restore S3247
}
