// constrained call avoiding boxing for value types
// ReSharper disable MergeCastWithTypeCheck
namespace ScrubJay.Text;

public static class TextBufferExtensions
{
    public static void Append(this ref TextBuffer buffer, char ch) => buffer.Add(ch);
    public static void Append(this ref TextBuffer buffer, string? str) => buffer.AddMany(str.AsSpan());
    public static void Append(this ref TextBuffer buffer, scoped ReadOnlySpan<char> text) => buffer.AddMany(text);
    public static void Append(this ref TextBuffer buffer, params char[]? characters) => buffer.AddMany(characters);

    public static void AppendFormatted<T>(this ref TextBuffer buffer, T value, string? format = null, IFormatProvider? provider = null)
    {
        string? str;
        if (value is IFormattable)
        {
#if NET6_0_OR_GREATER
            if (value is ISpanFormattable)
            {
                int charsWritten;
                while (!((ISpanFormattable)value).TryFormat(buffer.Available, out charsWritten, format, provider))
                {
                    buffer.Grow();
                }

                buffer.Count += charsWritten;
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
            buffer.AddMany(str.AsSpan());
        }
    }

    [HandlesResourceDisposal]
    public static string ToStringAndDispose(this ref TextBuffer buffer)
    {
        string result = buffer.Written.ToString();
        buffer.Dispose();
        return result;
    }
}