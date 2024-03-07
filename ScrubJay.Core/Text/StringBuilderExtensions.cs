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

#region AppendJoin

    private static StringBuilder AppendJoinCore<T>(StringBuilder builder, ReadOnlySpan<char> separator, T[]? values)
    {
        if (values is null || values.Length == 0) return builder;

        builder.Append<T>(values[0]);

        for (int i = 1; i < values.Length; i++)
        {
            builder.Append(separator).Append<T>(values[i]);
        }

        return builder;
    }

    private static StringBuilder AppendJoinCore<T>(StringBuilder builder, ReadOnlySpan<char> separator, IEnumerable<T> values)
    {
        using var e = values.GetEnumerator();

        if (!e.MoveNext()) return builder;

        builder.Append<T>(e.Current);
        while (e.MoveNext())
        {
            builder.Append(separator).Append(e.Current);
        }

        return builder;
    }


    public static StringBuilder AppendJoin(this StringBuilder builder, char separator, params object?[]? values)
    {
        return AppendJoinCore<object?>(builder, separator.AsSpan(), values);
    }

    public static StringBuilder AppendJoin<T>(this StringBuilder builder, char separator, IEnumerable<T> values)
    {
        return AppendJoinCore<T>(builder, separator.AsSpan(), values);
    }

    public static StringBuilder AppendJoin(this StringBuilder builder, char separator, params string?[]? values)
    {
        return AppendJoinCore<string?>(builder, separator.AsSpan(), values);
    }
    
    public static StringBuilder AppendJoin<T>(this StringBuilder builder, char separator, params T[]? values)
    {
        return AppendJoinCore<T>(builder, separator.AsSpan(), values);
    }

    public static StringBuilder AppendJoin(this StringBuilder builder, string? separator, params object?[]? values)
    {
        return AppendJoinCore<object?>(builder, separator.AsSpan(), values);
    }

    public static StringBuilder AppendJoin<T>(this StringBuilder builder, string? separator, IEnumerable<T> values)
    {
        return AppendJoinCore<T>(builder, separator.AsSpan(), values);
    }

    public static StringBuilder AppendJoin(this StringBuilder builder, string? separator, params string?[]? values)
    {
        return AppendJoinCore<string?>(builder, separator.AsSpan(), values);
    }
    
    public static StringBuilder AppendJoin<T>(this StringBuilder builder, string? separator, params T[]? values)
    {
        return AppendJoinCore<T>(builder, separator.AsSpan(), values);
    }

#endregion

#endif

    public static StringBuilder Append<T>(
        this StringBuilder builder,
        T? value)
    {
        return value is null ? builder : builder.Append(value.ToString());
    }

    public static StringBuilder Append<T>(
        this StringBuilder builder,
        T? value,
        string? format,
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