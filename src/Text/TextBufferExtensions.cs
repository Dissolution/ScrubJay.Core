#pragma warning disable S3247


// constrained call avoiding boxing for value types
// ReSharper disable MergeCastWithTypeCheck
namespace ScrubJay.Text;

/// <summary>
/// Extensions on <see cref="SpanBuffer{T}">Buffers</see> that contain <see cref="char">chars</see>
/// </summary>
[PublicAPI]
public static class TextSpanBufferExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Append(this ref TextSpanBuffer spanBuffer, char ch) => spanBuffer.Add(ch);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Append(this ref TextSpanBuffer spanBuffer, string? str) => spanBuffer.AddMany(str.AsSpan());
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Append(this ref TextSpanBuffer spanBuffer, scoped ReadOnlySpan<char> text) => spanBuffer.AddMany(text);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Append(this ref TextSpanBuffer spanBuffer, params char[]? characters) => spanBuffer.AddMany(characters);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AppendFormatted<T>(this ref TextSpanBuffer spanBuffer, T? value, string? format = null, IFormatProvider? provider = null)
    {
        string? str;
        if (value is IFormattable)
        {
#if NET6_0_OR_GREATER
            if (value is ISpanFormattable)
            {
                int charsWritten;
                while (!((ISpanFormattable)value).TryFormat(spanBuffer.Available, out charsWritten, format, provider))
                {
                    spanBuffer.Grow();
                }

                spanBuffer.Count += charsWritten;
                return;
            }
#endif
            str = ((IFormattable)value).ToString(format, provider);
        }
        else
        {
            str = value?.ToString();
        }

        if (str is not null)
        {
            spanBuffer.AddMany(str.AsSpan());
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [HandlesResourceDisposal]
    public static string ToStringAndDispose(this ref TextSpanBuffer spanBuffer)
    {
        string result = spanBuffer.Written.ToString();
        spanBuffer.Dispose();
        return result;
    }
}