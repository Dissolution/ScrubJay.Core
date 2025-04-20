// Prefix generic type parameter with T

#pragma warning disable CA1715

namespace ScrubJay.Text;




[PublicAPI]
public static class TextBuilderExtensions
{
#region AppendIf

    public static B AppendIf<B, T>(this B builder, bool condition, T trueValue)
        where B : TextBuilderBase<B>
    {
        if (condition)
        {
            builder.Append<T>(trueValue);
        }
        return builder;
    }

    public static B AppendIf<B, T>(
        this B builder,
        bool condition,
        T trueValue,
        T falseValue)
        where B : TextBuilderBase<B>
    {
        if (condition)
        {
            builder.Append<T>(trueValue);
        }
        else
        {
            builder.Append<T>(falseValue);
        }
        return builder;
    }

    public static B AppendIf<B, T>(this B builder, Nullable<T> nullable, string? format = null, IFormatProvider? provider = null)
        where B : TextBuilderBase<B>
        where T : struct
    {
        if (nullable.TryGetValue(out var value))
        {
            builder.Append<T>(value, format, provider);
        }
        return builder;
    }

    public static B AppendIf<B, T>(this B builder, Option<T> option, string? format = null, IFormatProvider? provider = null)
        where B : TextBuilderBase<B>
    {
        if (option.IsSome(out var some))
        {
            builder.Append<T>(some, format, provider);
        }
        return builder;
    }

    public static B AppendIf<B, T>(this B builder, Result<T> result, string? format = null, IFormatProvider? provider = null)
        where B : TextBuilderBase<B>
    {
        if (result.IsOk(out var ok))
        {
            builder.Append<T>(ok, format, provider);
        }
        return builder;
    }

#endregion

#region AppendOk + AppendError

    public static B AppendOk<B, T>(this B builder, Result<T> result)
        where B : TextBuilderBase<B>
    {
        if (result.IsOk(out var ok))
        {
            builder.Append<T>(ok);
        }
        return builder;
    }

    public static B AppendError<B, T>(this B builder, Result<T> result)
        where B : TextBuilderBase<B>
    {
        if (result.IsError(out var error))
        {
            builder.Append(error);
        }
        return builder;
    }


    public static B AppendOk<B, T, E>(this B builder, Result<T, E> result)
        where B : TextBuilderBase<B>
    {
        if (result.IsOk(out var ok))
        {
            builder.Append<T>(ok);
        }
        return builder;
    }

    public static B AppendError<B, T, E>(this B builder, Result<T, E> result)
        where B : TextBuilderBase<B>
    {
        if (result.IsError(out var error))
        {
            builder.Append<E>(error);
        }
        return builder;
    }

#endregion

#region With SpanSplitter

    public delegate void BuildEnumeratedSplitValue<in B, T>(B builder, ReadOnlySpan<T> segment)
        where B : TextBuilderBase<B>
        where T : IEquatable<T>;

    public delegate void BuildEnumeratedSplitValueIndex<in B, T>(B builder, ReadOnlySpan<T> segment, int index)
        where B : TextBuilderBase<B>
        where T : IEquatable<T>;


    public static B Enumerate<B, T>(this B builder, SpanSplitter<T> splitSpan, BuildEnumeratedSplitValue<B, T> buildValue)
        where B : TextBuilderBase<B>
        where T : IEquatable<T>
    {
        while (splitSpan.MoveNext())
        {
            buildValue(builder, splitSpan.Current);
        }
        return builder;
    }

    public static B Iterate<B, T>(this B builder, SpanSplitter<T> splitSpan, BuildEnumeratedSplitValueIndex<B, T> buildValueIndex)
        where B : TextBuilderBase<B>
        where T : IEquatable<T>
    {
        int i = 0;
        while (splitSpan.MoveNext())
        {
            buildValueIndex(builder, splitSpan.Current, i);
            i++;
        }
        return builder;
    }

    public static B Delimit<B, T>(this B builder, char delimiter, SpanSplitter<T> splitSpan, BuildEnumeratedSplitValue<B, T> buildValue)
        where B : TextBuilderBase<B>
        where T : IEquatable<T>
    {
        if (!splitSpan.MoveNext())
            return builder;
        buildValue(builder, splitSpan.Current);
        while (splitSpan.MoveNext())
        {
            builder.Append(delimiter);
            buildValue(builder, splitSpan.Current);
        }

        return builder;
    }

    public static B Delimit<B, T>(this B builder, scoped text delimiter, SpanSplitter<T> splitSpan, BuildEnumeratedSplitValue<B, T> buildValue)
        where B : TextBuilderBase<B>
        where T : IEquatable<T>
    {
        if (!splitSpan.MoveNext())
            return builder;
        buildValue(builder, splitSpan.Current);
        while (splitSpan.MoveNext())
        {
            builder.Append(delimiter);
            buildValue(builder, splitSpan.Current);
        }

        return builder;
    }

    public static B Delimit<B, T>(this B builder, Action<B> delimit, SpanSplitter<T> splitSpan, BuildEnumeratedSplitValue<B, T> buildValue)
        where B : TextBuilderBase<B>
        where T : IEquatable<T>
    {
        if (!splitSpan.MoveNext())
            return builder;
        buildValue(builder, splitSpan.Current);
        while (splitSpan.MoveNext())
        {
            delimit(builder);
            buildValue(builder, splitSpan.Current);
        }

        return builder;
    }

#endregion
}
