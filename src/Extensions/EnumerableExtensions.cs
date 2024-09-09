namespace ScrubJay.Extensions;

/// <summary>
/// Extensions on <see cref="IEnumerable{T}"/> and <see cref="IEnumerable"/>
/// </summary>
[PublicAPI]
public static class EnumerableExtensions
{
    /// <summary>
    /// A Predicate against <paramref name="input"/> that also produces <paramref name="output"/>
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    public delegate bool SelectWherePredicate<in TIn, TOut>(TIn input, out TOut output);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="selectWherePredicate"></param>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    /// <returns></returns>
    public static IEnumerable<TOut> SelectWhere<TIn, TOut>(this IEnumerable<TIn> source, SelectWherePredicate<TIn, TOut> selectWherePredicate)
    {
        foreach (TIn input in source)
        {
            if (selectWherePredicate(input, out TOut output))
            {
                yield return output;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="enumerable"></param>
    /// <param name="selectWhere"></param>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    /// <returns></returns>
    public static IEnumerable<TOut> SelectWhere<TIn, TOut>(this IEnumerable<TIn> enumerable, Func<TIn, Option<TOut>> selectWhere)
    {
        return enumerable.SelectMany(i => selectWhere(i));
    }

    public static IEnumerable<TOut> SelectWhere<TIn, TOut, TError>(this IEnumerable<TIn> enumerable, Func<TIn, Result<TOut, TError>> selectWhere)
    {
        return enumerable.SelectMany(i => selectWhere(i));
    }

    public static T One<T>(this IEnumerable<T> enumerable)
    {
        using var e = enumerable.GetEnumerator();
        if (!e.MoveNext())
            throw new InvalidOperationException("There are no items");
        var one = e.Current;
        if (e.MoveNext())
            throw new InvalidOperationException("There are too many items");
        return one;
    }

    [return: NotNullIfNotNull(nameof(defaultValue))]
    public static T? OneOrDefault<T>(this IEnumerable<T>? source, T? defaultValue = default)
    {
        if (source is null)
            return defaultValue;
        using var e = source.GetEnumerator();
        if (!e.MoveNext())
            return defaultValue;
        T value = e.Current;
        if (e.MoveNext())
            return defaultValue;
        return value;
    }

    [return: NotNullIfNotNull(nameof(defaultValue))]
    public static T? OneOrDefault<T>(this IEnumerable<T>? source, Func<T, bool> predicate, T? defaultValue = default)
    {
        if (source is null)
            return defaultValue;
        using var e = source.GetEnumerator();
        while (e.MoveNext())
        {
            T result = e.Current;
            if (predicate(result))
            {
                while (e.MoveNext())
                {
                    // If there is more than one match, fail
                    if (predicate(e.Current))
                    {
                        return defaultValue;
                    }
                }

                // Only one match
                return result;
            }
        }

        // No matches
        return defaultValue;
    }

    /// <summary>
    /// Enumerates over the non-<c>null</c> items in <paramref name="source"/>
    /// </summary>
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?>? source)
        where T : notnull
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
    {
        return enumerable.OrderBy(e => Array.IndexOf(itemOrder, e));
    }

    public static void Consume<T>(this IEnumerable<T> enumerable, Action<T> perItem)
    {
        if (enumerable is IList<T> list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                perItem(list[i]);
            }
        }
        else
        {
            foreach (T item in enumerable)
            {
                perItem(item);
            }
        }
    }

    public static int SequenceCompareTo<TSource>(this IEnumerable<TSource>? first, IEnumerable<TSource>? second) => SequenceCompareTo(first, second, null);

#pragma warning disable S3776
    public static int SequenceCompareTo<TSource>(this IEnumerable<TSource>? first, IEnumerable<TSource>? second, IComparer<TSource>? comparer)
    {
        if (ReferenceEquals(first, second))
            return 0;
        if (first is null)
            return -1;
        if (second is null)
            return 1;

        int c;

        if (first is ICollection<TSource> firstCol && second is ICollection<TSource> secondCol)
        {
            c = firstCol.Count.CompareTo(secondCol.Count);
            if (c != 0)
                return c;

            if (firstCol is IList<TSource> firstList && secondCol is IList<TSource> secondList)
            {
                comparer ??= Comparer<TSource>.Default;

                int count = firstCol.Count;
                for (int i = 0; i < count; i++)
                {
                    c = comparer.Compare(firstList[i], secondList[i]);
                    if (c != 0)
                        return c;
                }

                return 0;
            }
        }

        using IEnumerator<TSource> e1 = first.GetEnumerator();
        using IEnumerator<TSource> e2 = second.GetEnumerator();

        comparer ??= Comparer<TSource>.Default;

        while (true)
        {
            bool e1Moved = e1.MoveNext();
            bool e2Moved = e2.MoveNext();
            if (e1Moved != e2Moved)
                return e1Moved ? 1 : -1; // different counts
            if (!e1Moved)
                return 0; // same items, same count
            c = comparer.Compare(e1.Current, e2.Current);
            if (c != 0)
                return c;
        }
    }
#pragma warning restore S3776

    /// <summary>
    /// A deep wrapper for <see cref="IEnumerable{T}"/> that ignores all thrown exceptions
    /// at every level of enumeration, only returning values that could be acquired without error
    /// </summary>
    public static UnbreakableEnumerable<T> UnbreakableEnumerate<T>(this IEnumerable<T>? enumerable) => new UnbreakableEnumerable<T>(enumerable);
}