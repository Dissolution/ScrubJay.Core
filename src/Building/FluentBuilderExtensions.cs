namespace ScrubJay.Building;

public delegate void BuildWithSpan<in B, T>(B builder, Span<T> span)
    where B : IFluentBuilder<B>;

public delegate void BuildWithReadOnlySpan<in B, T>(B builder, ReadOnlySpan<T> span)
    where B : IFluentBuilder<B>;

/// <summary>
/// Extensions on <see cref="IFluentBuilder{B}"/>
/// </summary>
[PublicAPI]
public static class FluentBuilderExtensions
{
#region Invoke

    public static B Invoke<B>(this B builder, Action<B>? instanceAction)
        where B : IFluentBuilder<B>
    {
        instanceAction?.Invoke(builder);
        return builder;
    }

    public static B Invoke<B, S>(this B builder, S state, Action<B, S>? instanceStateAction)
        where B : IFluentBuilder<B>
    {
        instanceStateAction?.Invoke(builder, state);
        return builder;
    }


    public static B Invoke<B, R>(this B builder, Func<B, R>? instanceFunc)
        where B : IFluentBuilder<B>
    {
        if (instanceFunc is not null)
        {
            // we never return the instance from the func, only ever ours
            _ = instanceFunc(builder);
        }

        return builder;
    }

    public static B Invoke<B, S, R>(this B builder, S state, Func<B, S, R>? instanceStateFunc)
        where B : IFluentBuilder<B>
    {
        if (instanceStateFunc is not null)
        {
            // we never return the instance from the func, only ever ours
            _ = instanceStateFunc(builder, state);
        }

        return builder;
    }

#endregion

#region If

    public static B If<B>(this B builder,
        bool condition,
        Action<B>? onTrue = null,
        Action<B>? onFalse = null)
        where B : IFluentBuilder<B>
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

    public static B If<B, S>(this B builder,
        S state,
        bool condition,
        Action<B, S>? onTrue = null,
        Action<B, S>? onFalse = null)
        where B : IFluentBuilder<B>
    {
        if (condition)
        {
            onTrue?.Invoke(builder, state);
        }
        else
        {
            onFalse?.Invoke(builder, state);
        }

        return builder;
    }

    public static B If<B, S>(this B builder,
        S state,
        Func<S, bool> statePredicate,
        Action<B, S>? onTrue = null,
        Action<B, S>? onFalse = null)
        where B : IFluentBuilder<B>
    {
        if (statePredicate(state))
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
        Option<T> option,
        Action<B, T>? onSome = null,
        Action<B>? onNone = null)
        where B : IFluentBuilder<B>
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
        where B : IFluentBuilder<B>
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
        where B : IFluentBuilder<B>
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

#endregion

#region IfNotNull

    public static B IfNotNull<B, T>(
        this B builder,
        T? value,
        Action<B, T>? onNotNull = null,
        Action<B>? onNull = null)
        where B : IFluentBuilder<B>
    {
        if (value is not null)
        {
            onNotNull?.Invoke(builder, value);
        }
        else
        {
            onNull?.Invoke(builder);
        }

        return builder;
    }

    public static B IfNotNull<B, T>(
        this B builder,
        // ReSharper disable once ConvertNullableToShortForm
        Nullable<T> nullable,
        Action<B, T>? onNotNull = null,
        Action<B>? onNull = null)
        where B : IFluentBuilder<B>
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

    public static B IfNotNull<B, T, S>(
        this B builder,
        T? value,
        S state,
        Action<B, S, T>? onNotNull = null,
        Action<B, S>? onNull = null)
        where B : IFluentBuilder<B>
    {
        if (value is not null)
        {
            onNotNull?.Invoke(builder, state, value);
        }
        else
        {
            onNull?.Invoke(builder, state);
        }

        return builder;
    }

    public static B IfNotNull<B, T, S>(
        this B builder,
        // ReSharper disable once ConvertNullableToShortForm
        Nullable<T> nullable,
        S state,
        Action<B, S, T>? onNotNull = null,
        Action<B, S>? onNull = null)
        where B : IFluentBuilder<B>
        where T : struct
    {
        if (nullable.TryGetValue(out var value))
        {
            onNotNull?.Invoke(builder, state, value);
        }
        else
        {
            onNull?.Invoke(builder, state);
        }

        return builder;
    }

#endregion

#region IfNotEmpty

    public static B IfNotEmpty<B, T>(
        this B builder,
        ReadOnlySpan<T> span,
        BuildWithReadOnlySpan<B, T>? onNotEmpty = null,
        Action<B>? onEmpty = null)
        where B : IFluentBuilder<B>
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
        Span<T> span,
        BuildWithSpan<B, T>? onNotEmpty = null,
        Action<B>? onEmpty = null)
        where B : IFluentBuilder<B>
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
        T[]? array,
        Action<B, T[]>? onNotEmpty = null,
        Action<B>? onEmpty = null)
        where B : IFluentBuilder<B>
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
        ICollection<T>? collection,
        Action<B, ICollection<T>>? onNotEmpty = null,
        Action<B>? onEmpty = null)
        where B : IFluentBuilder<B>
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

    public static B IfNotEmpty<B, T>(
        this B builder,
        IReadOnlyCollection<T>? collection,
        Action<B, IReadOnlyCollection<T>>? onNotEmpty = null,
        Action<B>? onEmpty = null)
        where B : IFluentBuilder<B>
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
        ICollection? collection,
        Action<B, ICollection>? onNotEmpty = null,
        Action<B>? onEmpty = null)
        where B : IFluentBuilder<B>
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
        where B : IFluentBuilder<B>
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

#endregion


#region Enumerate

    public static B Enumerate<B, T>(this B builder, scoped ReadOnlySpan<T> values, Action<B, T> onBuilderValue)
        where B : IFluentBuilder<B>
    {
        foreach (var t in values)
        {
            onBuilderValue(builder, t);
        }

        return builder;
    }

    public static B Enumerate<B, T>(this B builder, scoped Span<T> values, Action<B, T> onBuilderValue)
        where B : IFluentBuilder<B>
    {
        foreach (var t in values)
        {
            onBuilderValue(builder, t);
        }

        return builder;
    }

    public static B Enumerate<B, T>(this B builder, T[]? values, Action<B, T> onBuilderValue)
        where B : IFluentBuilder<B>
    {
        if (values is not null)
        {
            foreach (var t in values)
            {
                onBuilderValue(builder, t);
            }
        }

        return builder;
    }

    public static B Enumerate<B, T>(this B builder, IEnumerable<T>? values, Action<B, T> onBuilderValue)
        where B : IFluentBuilder<B>
    {
        if (values is not null)
        {
            foreach (var value in values)
            {
                onBuilderValue(builder, value);
            }
        }

        return builder;
    }

#endregion

#region Enumerate + Delimit

    public static B EnumerateAndDelimit<B, T>(this B builder,
        scoped ReadOnlySpan<T> values,
        Action<B, T> buildValue,
        Action<B> buildDelimiter)
        where B : IFluentBuilder<B>
    {
        int len = values.Length;
        if (len == 0)
            return builder;
        buildValue(builder, values[0]);
        for (int i = 1; i < len; i++)
        {
            buildDelimiter(builder);
            buildValue(builder, values[i]);
        }

        return builder;
    }

    public static B EnumerateAndDelimit<B, T>(this B builder,
        scoped Span<T> values,
        Action<B, T> buildValue,
        Action<B> buildDelimiter)
        where B : IFluentBuilder<B>
    {
        int len = values.Length;
        if (len == 0)
            return builder;
        buildValue(builder, values[0]);
        for (int i = 1; i < len; i++)
        {
            buildDelimiter(builder);
            buildValue(builder, values[i]);
        }

        return builder;
    }

    public static B EnumerateAndDelimit<B, T>(this B builder,
        T[]? values,
        Action<B, T> buildValue,
        Action<B> buildDelimiter)
        where B : IFluentBuilder<B>
    {
        if (values is not null)
        {
            int len = values.Length;
            if (len == 0)
                return builder;
            buildValue(builder, values[0]);
            for (int i = 1; i < len; i++)
            {
                buildDelimiter(builder);
                buildValue(builder, values[i]);
            }
        }

        return builder;
    }

    public static B EnumerateAndDelimit<B, T>(this B builder,
        IList<T>? values,
        Action<B, T> buildValue,
        Action<B> buildDelimiter)
        where B : IFluentBuilder<B>
    {
        if (values is not null)
        {
            int len = values.Count;
            if (len == 0)
                return builder;
            buildValue(builder, values[0]);
            for (int i = 1; i < len; i++)
            {
                buildDelimiter(builder);
                buildValue(builder, values[i]);
            }
        }

        return builder;
    }

    public static B EnumerateAndDelimit<B, T>(this B builder,
        IEnumerable<T>? values,
        Action<B, T> buildValue,
        Action<B> buildDelimiter)
        where B : IFluentBuilder<B>
    {
        if (values is not null)
        {
            using var e = values.GetEnumerator();
            if (!e.MoveNext())
                return builder;
            buildValue(builder, e.Current);
            while (e.MoveNext())
            {
                buildDelimiter(builder);
                buildValue(builder, e.Current);
            }
        }

        return builder;
    }

#endregion

#region Iterate

    public static B Iterate<B, T>(this B builder,
        scoped ReadOnlySpan<T> values,
        Action<B, T, int> onBuilderValueIndex)
        where B : IFluentBuilder<B>
    {
        for (int i = 0; i < values.Length; i++)
        {
            onBuilderValueIndex(builder, values[i], i);
        }

        return builder;
    }

    public static B Iterate<B, T>(this B builder,
        scoped Span<T> values,
        Action<B, T, int> onBuilderValueIndex)
        where B : IFluentBuilder<B>
    {
        for (int i = 0; i < values.Length; i++)
        {
            onBuilderValueIndex(builder, values[i], i);
        }

        return builder;
    }

    public static B Iterate<B, T>(this B builder,
        T[]? values,
        Action<B, T, int> onBuilderValueIndex)
        where B : IFluentBuilder<B>
    {
        if (values is not null)
        {
            for (int i = 0; i < values.Length; i++)
            {
                onBuilderValueIndex(builder, values[i], i);
            }
        }

        return builder;
    }

    public static B Iterate<B, T>(this B builder,
        IList<T>? values,
        Action<B, T, int> onBuilderValueIndex)
        where B : IFluentBuilder<B>
    {
        if (values is not null)
        {
            for (int i = 0; i < values.Count; i++)
            {
                onBuilderValueIndex(builder, values[i], i);
            }
        }

        return builder;
    }

    public static B Iterate<B, T>(this B builder,
        IEnumerable<T> values,
        Action<B, T, int> onBuilderValueIndex)
        where B : IFluentBuilder<B>
    {
        int index = 0;
        foreach (var value in values)
        {
            onBuilderValueIndex(builder, value, index);
            index++;
        }

        return builder;
    }

#endregion

    public static B Repeat<B>(this B builder, int count, Action<B>? build)
        where B : IFluentBuilder<B>
    {
        if (count > 0 && build is not null)
        {
            for (int i = 0; i < count; i++)
            {
                build(builder);
            }
        }

        return builder;
    }
}