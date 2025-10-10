namespace ScrubJay.Extensions;

/// <summary>
/// Extensions on <see cref="IEnumerable{T}"/>, <see cref="IEnumerable"/>, <see cref="IEnumerator{T}"/>, and <see cref="IEnumerator"/>
/// </summary>
[PublicAPI]
public static class EnumerableExtensions
{
#region IEnumerable

    public static void Dispose(
        [HandlesResourceDisposal] this IEnumerator enumerator)
    {
        if (enumerator is IDisposable)
        {
            ((IDisposable)enumerator).Dispose();
        }
    }

#endregion


    /// <summary>
    /// A Predicate against <paramref name="input"/> that also produces <paramref name="output"/>
    /// </summary>
    /// <typeparam name="I"></typeparam>
    /// <typeparam name="O"></typeparam>
    public delegate bool SelectWherePredicate<in I, O>(I input, out O output);

    public static IEnumerable<O> SelectWhere<I, O>(this IEnumerable<I> enumerable,
        SelectWherePredicate<I, O> selectWherePredicate)
    {
        foreach (I input in enumerable)
        {
            if (selectWherePredicate(input, out O output))
            {
                yield return output;
            }
        }
    }


    /// <summary>
    /// Enumerates over the non-<c>null</c> items in <paramref name="source"/>
    /// </summary>
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?>? source)
    {
        if (source is null)
            yield break;
        foreach (T? value in source)
        {
            if (value is not null)
                yield return value;
        }
    }


    public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> enumerable, T[] itemOrder)
        where T : IEquatable<T>
        => enumerable.OrderBy(e => Array.IndexOf(itemOrder, e));

    /// <summary>
    /// Consume this <see cref="IEnumerable{T}"/> by performing an <see cref="Action{T}"/> on each of its values
    /// </summary>
    /// <param name="enumerable">
    /// The <see cref="IEnumerable{T}"/> to consume (after which further enumeration will fail)
    /// </param>
    /// <param name="perItem">
    /// An <see cref="Action{T}"/> to invoke upon each value in the <see cref="IEnumerable{T}"/>
    /// </param>
    public static void Consume<T>(this IEnumerable<T>? enumerable, Action<T> perItem)
    {
        switch (enumerable)
        {
            case null:
                return;
            case IList<T> list:
            {
                for (int i = 0; i < list.Count; i++)
                {
                    perItem(list[i]);
                }

                break;
            }
            default:
            {
                foreach (T item in enumerable)
                {
                    perItem(item);
                }

                break;
            }
        }
    }

#region One()

    /* The behavior of Enumerable.SingleOrDefault() is counter-intuitive:
     * The name suggests that if there is one item in an enumerable, that item will be returned,
     * and if there are zero or more than one items, default(T) will be returned.
     * That is not the case -- default(T) is returned if there are zero items and an Exception is thrown if there is more than one.
     *
     * This is an implementation of expected behavior.
     */


    public static Result<T> TryGetOne<T>(this IEnumerable<T>? enumerable)
    {
        if (enumerable is null)
        {
            return new ArgumentNullException(nameof(enumerable));
        }
        else if (enumerable is IList<T> list)
        {
            if (list.Count == 1)
            {
                return Ok(list[0]);
            }

            return new ArgumentException($"List [{list.Count}] does not have one item", nameof(enumerable));
        }
        else if (enumerable is ICollection<T> collection)
        {
            if (collection.Count == 1)
            {
                using var e = collection.GetEnumerator();
                e.MoveNext();
                return Ok(e.Current);
            }

            return new ArgumentException($"Collection ({collection.Count}) does not have one item", nameof(enumerable));
        }
        else
        {
            using var e = enumerable.GetEnumerator();

            if (!e.MoveNext())
                return new ArgumentException("Enumerable has zero items");

            T value = e.Current;

            if (e.MoveNext())
                return new ArgumentException("Enumerable has more than one item");

            return Ok(value);
        }
    }

    public static Result<T> TryGetOne<T>(this IEnumerable<T>? enumerable, Func<T, bool>? predicate)
    {
        if (enumerable is null)
            return new ArgumentNullException(nameof(enumerable));

        if (predicate is null)
            return new ArgumentNullException(nameof(predicate));

        if (enumerable is IList<T> list)
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
                            return new ArgumentException("List has more than one matching item", nameof(enumerable));
                        }
                    }

                    // exactly one matching value
                    return Ok(value);
                }
            }

            return new ArgumentException("List has no matching items", nameof(enumerable));
        }

        using IEnumerator<T> e = enumerable.GetEnumerator();

        while (e.MoveNext())
        {
            T value = e.Current;

            if (predicate(value))
            {
                while (e.MoveNext())
                {
                    if (predicate(e.Current))
                    {
                        return new ArgumentException("Enumerable has more than one matching item");
                    }
                }

                // exactly one matching value
                return Ok(value);
            }
        }

        return new ArgumentException("Enumerable has no matching items");
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


#pragma warning restore S3776

#if NETSTANDARD2_0
    public static HashSet<T> ToHashSet<T>(this IEnumerable<T> enumerable)
        => new(enumerable);
#endif
}