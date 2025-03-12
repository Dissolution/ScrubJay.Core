#pragma warning disable CA1715

namespace ScrubJay.Text;

[PublicAPI]
public static class TextBuilderExtensions
{
    public static B AppendType<B>(this B builder, Type? type)
        where B : TextBuilderBase<B>
        => builder.Append(type.NameOf());

#region AppendIf
    public static B AppendIf<B, T>(this B builder, bool condition, T trueValue)
        where B : TextBuilderBase<B>
    {
        if (condition)
        {
            return builder.Append<T>(trueValue);
        }
        else
        {
            return builder;
        }
    }

    public static B AppendIf<B, T>(this B builder, bool condition,
        T trueValue,
        T falseValue)
        where B : TextBuilderBase<B>
    {
        if (condition)
        {
            return builder.Append<T>(trueValue);
        }
        else
        {
            return builder.Append<T>(falseValue);
        }
    }

    public static B AppendIf<B>(this B builder, bool condition,
        Action<B>? trueBuild,
        Action<B>? falseBuild = null)
        where B : TextBuilderBase<B>
    {
        if (condition)
        {
            return builder.Invoke(trueBuild);
        }
        else
        {
            return builder.Invoke(falseBuild);
        }
    }

    public static B AppendOk<B, O, E>(this B builder, Result<O, E> result)
        where B : TextBuilderBase<B>
    {
        if (result.HasOk(out var ok))
        {
            return builder.Append<O>(ok);
        }
        return builder;
    }

    public static B AppendError<B, O, E>(this B builder, Result<O, E> result)
        where B : TextBuilderBase<B>
    {
        if (result.HasError(out var error))
        {
            return builder.Append<E>(error);
        }
        return builder;
    }

    public static B AppendIf<B, O, E>(this B builder, Result<O, E> result,
        Action<B, O>? okBuild,
        Action<B, E>? errorBuild = null)
        where B : TextBuilderBase<B>
    {
        result.Match(
            ok => okBuild?.Invoke(builder, ok),
            error => errorBuild?.Invoke(builder, error));
        return builder;
    }

    public static B AppendSome<B, T>(this B builder, Option<T> option)
        where B : TextBuilderBase<B>
    {
        if (option.HasSome(out var some))
        {
            return builder.Append<T>(some);
        }
        return builder;
    }

    public static B AppendIf<B, T>(this B builder, Option<T> option,
        Action<B, T>? someBuild,
        Action<B, None>? noneBuild = null)
        where B : TextBuilderBase<B>
    {
        if (option.HasSome(out var some))
        {
            someBuild?.Invoke(builder, some);
        }
        else
        {
            noneBuild?.Invoke(builder, default);
        }
        return builder;
    }
#endregion

    public static B InvokeIf<B>(this B builder, bool condition,
        Action<B>? trueBuild,
        Action<B>? falseBuild = null)
        where B : TextBuilderBase<B>
    {
        if (condition)
        {
            trueBuild?.Invoke(builder);
        }
        else
        {
            falseBuild?.Invoke(builder);
        }
        return builder;
    }

    public static B InvokeIf<B, T>(this B builder, Option<T> option,
        Action<B, T>? someBuild,
        Action<B>? noneBuild = null)
        where B : TextBuilderBase<B>
    {
        if (option.HasSome(out var some))
        {
            someBuild?.Invoke(builder, some);
        }
        else
        {
            noneBuild?.Invoke(builder);
        }
        return builder;
    }

    public static B InvokeIf<B, O, E>(this B builder, Result<O,E> result,
        Action<B, O>? okBuild,
        Action<B, E>? errorBuild = null)
        where B : TextBuilderBase<B>
    {
        if (result.HasOkOrError(out var ok, out var error))
        {
            okBuild?.Invoke(builder, ok);
        }
        else
        {
            errorBuild?.Invoke(builder, error);
        }
        return builder;
    }

    public delegate void BuildEnumeratedSplitValue<B, T>(B builder, ReadOnlySpan<T> segment)
        where B : TextBuilderBase<B>
        where T : IEquatable<T>;

    public delegate void BuildEnumeratedSplitValueIndex<B, T>(B builder, ReadOnlySpan<T> segment, int index)
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
