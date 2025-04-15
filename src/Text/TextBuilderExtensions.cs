// Prefix generic type parameter with T

#pragma warning disable CA1715

namespace ScrubJay.Text;

public delegate void BuildWithSpan<in B, T>(B builder, Span<T> span)
    where B : TextBuilderBase<B>;

public delegate void BuildWithReadOnlySpan<in B, T>(B builder, ReadOnlySpan<T> span)
    where B : TextBuilderBase<B>;


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

#region If

    public static B If<B>(
        this B builder,
        bool condition,
        Action<B>? onTrue = null,
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

    public static B If<B, S>(
        this B builder,
        S state,
        Func<S, bool> condition,
        Action<B, S>? onTrue = null,
        Action<B, S>? onFalse = null)
        where B : TextBuilderBase<B>
    {
        if (condition(state))
        {
            onTrue?.Invoke(builder, state);
        }
        else
        {
            onFalse?.Invoke(builder, state);
        }
        return builder;
    }

    public static B If<B, T>(
        this B builder,
        Nullable<T> nullable,
        Action<B, T>? onNotNull = null,
        Action<B>? onNull = null)
        where B : TextBuilderBase<B>
        where T : struct
    {
        if (nullable.TryGetValue(out var value))
        {
            onNotNull?.Invoke(builder, value);
        }
        else
        {
            onNull?.Invoke(builder);
        }
        return builder;
    }

    public static B If<B, T>(
        this B builder,
        Option<T> option,
        Action<B, T>? onSome = null,
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

    public static B If<B, T>(
        this B builder,
        Result<T> result,
        Action<B, T>? onOk = null,
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

    public static B If<B, T, E>(
        this B builder,
        Result<T, E> result,
        Action<B, T>? onOk = null,
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

#endregion

#region Validation

    public static B IfNotNull<B, T>(
        this B builder,
        T? value,
        Action<B, T>? onNotNull = null,
        Action<B>? onNull = null)
        where B : TextBuilderBase<B>
    {
        if (value is not null)
        {
            onNotNull?.Invoke(builder, value!);
        }
        else
        {
            onNull?.Invoke(builder);
        }
        return builder;
    }

    public static B IfNotNull<B, T, S>(
        this B builder,
        T? value,
        S state,
        Action<B, T>? onNotNull = null,
        Action<B, S>? onNull = null)
        where B : TextBuilderBase<B>
    {
        if (value is not null)
        {
            onNotNull?.Invoke(builder, value!);
        }
        else
        {
            onNull?.Invoke(builder, state);
        }
        return builder;
    }

    public static B IfNotEmpty<B, T>(
        this B builder,
        T[]? array,
        Action<B, T[]>? onNotEmpty = null,
        Action<B>? onEmpty = null)
        where B : TextBuilderBase<B>
    {
        if (array is null || (array.Length == 0))
        {
            onEmpty?.Invoke(builder);
        }
        else
        {
            onNotEmpty?.Invoke(builder, array);
        }
        return builder;
    }

    public static B IfNotEmpty<B, T>(
        this B builder,
        Span<T> span,
        BuildWithSpan<B,T>? onNotEmpty = null,
        Action<B>? onEmpty = null)
        where B : TextBuilderBase<B>
    {
        if (span.Length == 0)
        {
            onEmpty?.Invoke(builder);
        }
        else
        {
            onNotEmpty?.Invoke(builder, span);
        }
        return builder;
    }

    public static B IfNotEmpty<B, T>(
        this B builder,
        ReadOnlySpan<T> span,
        BuildWithReadOnlySpan<B,T>? onNotEmpty = null,
        Action<B>? onEmpty = null)
        where B : TextBuilderBase<B>
    {
        if (span.Length == 0)
        {
            onEmpty?.Invoke(builder);
        }
        else
        {
            onNotEmpty?.Invoke(builder, span);
        }
        return builder;
    }

    public static B IfNotEmpty<B, T>(
        this B builder,
        ICollection<T>? collection,
        Action<B, ICollection<T>>? onNotEmpty = null,
        Action<B>? onEmpty = null)
        where B : TextBuilderBase<B>
    {
        if (collection is null || (collection.Count == 0))
        {
            onEmpty?.Invoke(builder);
        }
        else
        {
            onNotEmpty?.Invoke(builder, collection);
        }
        return builder;
    }

    public static B IfNotEmpty<B>(
        this B builder,
        string? str,
        Action<B, string>? onNotEmpty = null,
        Action<B>? onEmpty = null)
        where B : TextBuilderBase<B>
    {
        if (string.IsNullOrEmpty(str))
        {
            onEmpty?.Invoke(builder);
        }
        else
        {
            onNotEmpty?.Invoke(builder, str!);
        }
        return builder;
    }


    public static B IfNotEmpty<B>(
        this B builder,
        ICollection? collection,
        Action<B, ICollection>? onNotEmpty = null,
        Action<B>? onEmpty = null)
        where B : TextBuilderBase<B>
    {
        if (collection is null || (collection.Count == 0))
        {
            onEmpty?.Invoke(builder);
        }
        else
        {
            onNotEmpty?.Invoke(builder, collection);
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
