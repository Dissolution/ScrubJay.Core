using System.Text;

namespace ScrubJay.Extensions;

/// <summary>
/// Extensions on <see cref="StringBuilder"/>
/// </summary>
public static class StringBuilderExtensions
{
    public static StringBuilder AppendJoin<T>(this StringBuilder builder, char delimiter, ReadOnlySpan<T> values)
    {
        if (values.Length == 0)
            return builder;
        builder.Append(values[0]);
        for (int i = 1; i < values.Length; i++)
        {
            builder.Append(delimiter);
            builder.Append(values[i]);
        }
        return builder;
    }
    
    public static StringBuilder AppendJoin<T>(this StringBuilder builder, char delimiter, params T[]? values)
    {
        if (values is null || values.Length == 0)
            return builder;
        builder.Append(values[0]);
        for (int i = 1; i < values.Length; i++)
        {
            builder.Append(delimiter).Append(values[i]);
        }
        return builder;
    }
    
    public static StringBuilder AppendJoin<T>(this StringBuilder builder, char delimiter, IEnumerable<T>? values)
    {
        if (values is null)
            return builder;
        using var e = values.GetEnumerator();
        if (!e.MoveNext())
            return builder;
        builder.Append(e.Current);
        while (e.MoveNext())
        {
            builder.Append(delimiter).Append(e.Current);
        }
        return builder;
    }
    
    
    public static StringBuilder AppendJoin<T>(this StringBuilder builder, string delimiter, ReadOnlySpan<T> values)
    {
        if (values.Length == 0)
            return builder;
        builder.Append(values[0]);
        for (int i = 1; i < values.Length; i++)
        {
            builder.Append(delimiter);
            builder.Append(values[i]);
        }
        return builder;
    }
    
    public static StringBuilder AppendJoin<T>(this StringBuilder builder, string delimiter, params T[]? values)
    {
        if (values is null || values.Length == 0)
            return builder;
        builder.Append(values[0]);
        for (int i = 1; i < values.Length; i++)
        {
            builder.Append(delimiter).Append(values[i]);
        }
        return builder;
    }
    
    public static StringBuilder AppendJoin<T>(this StringBuilder builder, string delimiter, IEnumerable<T>? values)
    {
        if (values is null)
            return builder;
        using var e = values.GetEnumerator();
        if (!e.MoveNext())
            return builder;
        builder.Append(e.Current);
        while (e.MoveNext())
        {
            builder.Append(delimiter).Append(e.Current);
        }
        return builder;
    }
}