﻿
using System.Text;

namespace ScrubJay.Text;

public static class StringBuilderExtensions
{
#if NET48 || NETSTANDARD2_0
    public static StringBuilder Append(this StringBuilder builder, ReadOnlySpan<char> text)
    {
        unsafe
        {
            fixed (char* ptr = &text.GetPinnableReference())
            {
                builder.Append(ptr, text.Length);
            }
        }
        return builder;
    }
#endif

    public static StringBuilder Append<T>(
        this StringBuilder builder,
        T? value,
        string? format = null,
        IFormatProvider? provider = null)
    {
        string? str;
        if (value is null)
        {
            return builder;
        }
        // No boxing for value types
        // ReSharper disable once MergeCastWithTypeCheck
        if (value is IFormattable)
        {
            str = ((IFormattable)value).ToString(format, provider);
        }
        else
        {
            str = value.ToString();
        }
        return builder.Append(str);
    }

    
    public static StringBuilder AppendDelimit<T>(
        this StringBuilder builder,
        string delimiter,
        ReadOnlySpan<T> values)
    {
        int count = values.Length;
        if (count > 0)
        {
            builder.Append<T>(values[0]);
            for (var i = 1; i < count; i++)
            {
                builder.Append(delimiter)
                    .Append<T>(values[i]);
            }   
        }
        return builder;
    }
    
    public static StringBuilder AppendDelimit<T>(
        this StringBuilder builder,
        string delimiter,
        IEnumerable<T> values)
    {
        using var e = values.GetEnumerator();
        if (!e.MoveNext())
            return builder;

        builder.Append<T>(e.Current);
        while (e.MoveNext())
        {
            builder.Append(delimiter)
                .Append<T>(e.Current);
        }
        return builder;
    }
}
