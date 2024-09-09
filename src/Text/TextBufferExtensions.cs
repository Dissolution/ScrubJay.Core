﻿#pragma warning disable S3247


// constrained call avoiding boxing for value types
// ReSharper disable MergeCastWithTypeCheck
namespace ScrubJay.Text;

/// <summary>
/// Extensions on <see cref="Buffer{T}">Buffers</see> that contain <see cref="char">chars</see>
/// </summary>
[PublicAPI]
public static class TextBufferExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Append(this ref TextBuffer buffer, char ch) => buffer.Add(ch);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Append(this ref TextBuffer buffer, string? str) => buffer.AddMany(str.AsSpan());
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Append(this ref TextBuffer buffer, scoped ReadOnlySpan<char> text) => buffer.AddMany(text);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Append(this ref TextBuffer buffer, params char[]? characters) => buffer.AddMany(characters);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AppendFormatted<T>(this ref TextBuffer buffer, T? value, string? format = null, IFormatProvider? provider = null)
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [HandlesResourceDisposal]
    public static string ToStringAndDispose(this ref TextBuffer buffer)
    {
        string result = buffer.Written.ToString();
        buffer.Dispose();
        return result;
    }
}