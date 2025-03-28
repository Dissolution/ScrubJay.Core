// Prefix generic type parameter with T
#pragma warning disable CA1715

namespace ScrubJay.Text;

[PublicAPI]
public static class TextBuilderExtensions
{
    /// <summary>
    /// Appends the name of a <see cref="Type"/> to this <typeparamref name="B"/>
    /// </summary>
    public static B AppendType<B>(this B builder, Type? type)
        where B : TextBuilderBase<B>
        => builder.Append(type.NameOf());


    public static B AppendIf<B, T>(this B builder, bool condition, T trueValue)
        where B : TextBuilderBase<B>
    {
        if (condition)
        {
            builder.Append<T>(trueValue);
        }
        return builder;
    }

    public static B AppendIf<B, T>(this B builder, bool condition,
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

    public static B AppendIf<B, T, F>(this B builder, bool condition,
        T trueValue,
        F falseValue)
        where B : TextBuilderBase<B>
    {
        if (condition)
        {
            builder.Append<T>(trueValue);
        }
        else
        {
            builder.Append<F>(falseValue);
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


    public static B AppendIf<B, T, E>(this B builder, Result<T, E> result)
        where B : TextBuilderBase<B>
    {
        if (result.IsOk(out var ok))
        {
            builder.Append<T>(ok);
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


    public static B If<B>(this B builder, bool condition,
        Action<B>? onTrue,
        Action<B>? onFalse = null)
        where B : TextBuilderBase<B>
    {
        if (condition)
        {
            onTrue?.Invoke(builder);
        }
        else
        {
            onFalse?.Invoke(builder);
        }
        return builder;
    }

    public static B If<B, T>(this B builder, Option<T> option,
        Action<B, T>? onSome,
        Action<B>? onNone = null)
        where B : TextBuilderBase<B>
    {
        if (option.IsSome(out var some))
        {
            onSome?.Invoke(builder, some);
        }
        else
        {
            onNone?.Invoke(builder);
        }
        return builder;
    }
    
    public static B If<B, T>(this B builder, Result<T> result,
        Action<B, T>? onOk,
        Action<B, Exception>? onError = null)
        where B : TextBuilderBase<B>
    {
        if (result.IsOkWithError(out var ok, out var error))
        {
            onOk?.Invoke(builder, ok);
        }
        else
        {
            onError?.Invoke(builder, error);
        }
        return builder;
    }

    public static B If<B, T, E>(this B builder, Result<T,E> result,
        Action<B, T>? onOk,
        Action<B, E>? onError = null)
        where B : TextBuilderBase<B>
    {
        if (result.HasOkOrError(out var ok, out var error))
        {
            onOk?.Invoke(builder, ok);
        }
        else
        {
            onError?.Invoke(builder, error);
        }
        return builder;
    }



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
}
