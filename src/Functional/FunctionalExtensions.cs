namespace ScrubJay.Functional;

[PublicAPI]
public static class FunctionalExtensions
{
    public static IEnumerable<TOut> SelectWhere<TIn, TOut>(
        this IEnumerable<TIn> enumerable,
        Func<TIn, Option<TOut>> selectWhere)
    {
        foreach (TIn input in enumerable)
        {
            if (selectWhere(input).IsSome(out var output))
            {
                yield return output;
            }
        }
    }

    public static IEnumerable<TOut> SelectWhere<TIn, TOut>(
        this IEnumerable<TIn> enumerable,
        Func<TIn, Result<TOut>> selectWhere)
    {
        foreach (TIn input in enumerable)
        {
            if (selectWhere(input).IsOk(out var output))
            {
                yield return output;
            }
        }
    }

    public static IEnumerable<TOut> SelectWhere<TIn, TOut, TError>(
        this IEnumerable<TIn> enumerable,
        Func<TIn, Result<TOut, TError>> selectWhere)
    {
        foreach (TIn input in enumerable)
        {
            if (selectWhere(input).IsOk(out var output))
            {
                yield return output;
            }
        }
    }

#region One()
    /* The behavior of Enumerable.SingleOrDefault is counter-intuitive
     * One would assume that the default value would be returned if there isn't a single value,
     * but instead the default is only returned if there are no values, and an exception is thrown if there is more than one.
     * One corrects this by returning the fallback if there is zero or more than one value.
     * Technically only OneOrDefault is required, but One is included for completeness.
     */

    public static Result<T> TryGetOne<T>(
        [AllowNull, NotNullWhen(true)]
        this IEnumerable<T> source)
    {
        if (source is null)
            return new ArgumentNullException(nameof(source));

        if (source is IList<T> list)
        {
            if (list.Count == 1)
                return Ok(list[0]);

            return new ArgumentException($"Source list has {list.Count} value(s)", nameof(source));
        }
        else if (source is ICollection<T> collection)
        {
            if (collection.Count == 1)
            {
                using var e = collection.GetEnumerator();
                e.MoveNext();
                return Ok(e.Current);
            }

            return new ArgumentException($"Source collection has {collection.Count} value(s)", nameof(source));
        }
        else
        {
            using var e = source.GetEnumerator();

            if (!e.MoveNext())
                return new ArgumentException("Source has 0 values");

            T value = e.Current;

            if (e.MoveNext())
                return new ArgumentException("Source has more than one value");

            return Ok(value);
        }
    }

    public static Result<T> TryGetOne<T>(
        [AllowNull, NotNullWhen(true)] this IEnumerable<T> source,
        Func<T, bool> predicate)
    {
        if (source is null)
            return new ArgumentNullException(nameof(source));

        if (predicate is null)
            return new ArgumentNullException(nameof(predicate));

        if (source is IList<T> list)
        {
            int count = list.Count;

            for (int i = 0; i < count; i++)
            {
                T value = list[i];
                if (predicate(value))
                {
                    for (i++; i < count; i++)
                    {
                        if (predicate(list[i]))
                        {
                            return new ArgumentException("Source list has more than one matching value");
                        }
                    }

                    // exactly one matching value
                    return Ok(value);
                }
            }

            return new ArgumentException("Source list has no matching values");
        }

        using IEnumerator<T> e = source.GetEnumerator();

        while (e.MoveNext())
        {
            T value = e.Current;

            if (predicate(value))
            {
                while (e.MoveNext())
                {
                    if (predicate(e.Current))
                    {
                        return new ArgumentException("Source has more than one matching value");
                    }
                }

                // exactly one matching value
                return Ok(value);
            }
        }

        return new ArgumentException("Source has no matching values");
    }


    public static T One<T>(this IEnumerable<T> source)
        => TryGetOne<T>(source).OkOrThrow();

    public static T One<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        => TryGetOne<T>(source, predicate).OkOrThrow();

    public static T? OneOrDefault<T>(this IEnumerable<T> source)
        => TryGetOne<T>(source).OkOrDefault();

    public static T OneOr<T>(this IEnumerable<T> source, T fallback)
        => TryGetOne<T>(source).OkOr(fallback);

    public static T? OneOrDefault<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        => TryGetOne<T>(source, predicate).OkOrDefault();

    public static T OneOr<T>(this IEnumerable<T> source, Func<T, bool> predicate, T fallback)
        => TryGetOne<T>(source, predicate).OkOr(fallback);

#endregion

#region First()
    public static Result<T> TryGetFirst<T>(
        [AllowNull, NotNullWhen(true)]
        this IEnumerable<T> source)
    {
        if (source is null)
            return new ArgumentNullException(nameof(source));

        if (source is IList<T> list)
        {
            if (list.Count > 0)
                return Ok(list[0]);

            return new ArgumentException("Source list has 0 values", nameof(source));
        }
        else if (source is ICollection<T> collection)
        {
            if (collection.Count > 1)
            {
                using var e = collection.GetEnumerator();
                e.MoveNext();
                return Ok(e.Current);
            }

            return new ArgumentException("Source collection has 0 values", nameof(source));
        }
        else
        {
            using var e = source.GetEnumerator();

            if (!e.MoveNext())
                return new ArgumentException("Source has 0 values");

            T value = e.Current;
            return Ok(value);
        }
    }

    public static Result<T> TryGetFirst<T>(
        [AllowNull, NotNullWhen(true)] this IEnumerable<T> source,
        Func<T, bool> predicate)
    {
        if (source is null)
            return new ArgumentNullException(nameof(source));

        if (predicate is null)
            return new ArgumentNullException(nameof(predicate));

        T value;

        if (source is IList<T> list)
        {
            int count = list.Count;

            for (int i = 0; i < count; i++)
            {
                value = list[i];
                if (predicate(value))
                    return Ok(value);
            }

            return new ArgumentException("Source list has no matching values");
        }

        using IEnumerator<T> e = source.GetEnumerator();

        while (e.MoveNext())
        {
            value = e.Current;
            if (predicate(value))
                return Ok(value);
        }

        return new ArgumentException("Source has no matching values");
    }


    public static T First<T>(this IEnumerable<T> source)
        => TryGetFirst<T>(source).OkOrThrow();

    public static T First<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        => TryGetFirst<T>(source, predicate).OkOrThrow();

    public static T? FirstOrDefault<T>(this IEnumerable<T> source)
        => TryGetFirst<T>(source).OkOrDefault();

    public static T FirstOr<T>(this IEnumerable<T> source, T fallback)
        => TryGetFirst<T>(source).OkOr(fallback);

    public static T? FirstOrDefault<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        => TryGetFirst<T>(source, predicate).OkOrDefault();

    public static T FirstOr<T>(this IEnumerable<T> source, Func<T, bool> predicate, T fallback)
        => TryGetFirst<T>(source, predicate).OkOr(fallback);

#endregion

    #region Min

    public static Result<T> TryGetMinimum<T>(
        [AllowNull, NotNullWhen(true)]
        this IEnumerable<T> source,
        IComparer<T>? valueComparer = null,
        bool firstMin = true)
    {
        if (source is null)
            return new ArgumentNullException(nameof(source));

        valueComparer ??= Comparer<T>.Default;

        T minimum;
        T value;
        int cut = firstMin ? 0 : 1;

        if (source is IList<T> list)
        {
            int count = list.Count;
            if (count == 0)
                return new ArgumentException("Source list has 0 values", nameof(source));
            minimum = list[0];
            for (int i = 1; i < count; i++)
            {
                value = list[i];
                if (valueComparer.Compare(value, minimum) < cut)
                    minimum = value;
            }

            return Ok(minimum);
        }

        using IEnumerator<T> e = source.GetEnumerator();

        if (!e.MoveNext())
            return new ArgumentException("Source has 0 values", nameof(source));

        minimum = e.Current;
        while (e.MoveNext())
        {
            value = e.Current;
            if (valueComparer.Compare(value, minimum) < cut)
                minimum = value;
        }

        return Ok(minimum);
    }

#endregion
}
