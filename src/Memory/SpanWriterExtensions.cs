#pragma warning disable S3247

namespace ScrubJay.Memory;

/// <summary>
/// Extensions on <see cref="SpanWriter{T}"/>
/// </summary>
/// <remarks>
/// These methods are extensions because they work on a constrained <see cref="SpanWriter{T}"/> 
/// </remarks>
[PublicAPI]
public static class SpanWriterExtensions
{
    public static bool TryWriteFormatted<T>(this SpanWriter<char> spanWriter, T? value, string? format = null, IFormatProvider? provider = null)
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

    public static bool TryWrite(this SpanWriter<char> spanWriter, string? str)
    {
        return spanWriter.TryWriteMany(str.AsSpan());
    }
}