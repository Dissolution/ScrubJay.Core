﻿
// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

using ScrubJay.Comparison;

namespace ScrubJay.Extensions;

public static class EnumerableExtensions
{
#if NETSTANDARD2_0
    public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
    {
        return new HashSet<T>(source);
    }
    public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source, IEqualityComparer<T>? itemComparer)
    {
        return new HashSet<T>(source, itemComparer);
    }
#endif

    public delegate bool SelectWherePredicate<in TIn, TOut>(TIn input, out TOut output);

    public static IEnumerable<TOut> SelectWhere<TIn, TOut>(this IEnumerable<TIn> source, SelectWherePredicate<TIn, TOut> selectWherePredicate)
    {
        foreach (TIn input in source)
        {
            if (selectWherePredicate(input, out TOut? output))
            {
                yield return output;
            }
        }
    }

    public static IEnumerable<TOut> SelectWhere<TIn, TOut>(
        this IEnumerable<TIn> enumerable,
        Func<TIn, Option<TOut>> selectWhere)
    {
        return enumerable.SelectMany(i => selectWhere(i));
    }

    public static IEnumerable<TOut> SelectWhere<TIn, TOut>(
        this IEnumerable<TIn> enumerable,
        Func<TIn, Result<TOut>> selectWhere)
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
        if (source is null) return defaultValue;
        using var e = source.GetEnumerator();
        if (!e.MoveNext()) return defaultValue;
        T value = e.Current;
        if (e.MoveNext()) return defaultValue;
        return value;
    }

    [return: NotNullIfNotNull(nameof(defaultValue))]
    public static T? OneOrDefault<T>(this IEnumerable<T>? source,
        Func<T, bool> predicate, T? defaultValue = default)
    {
        if (source is null) return defaultValue;
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
    /// Enumerates exactly two elements from <paramref name="source"/>
    /// </summary>
    /// <param name="source"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static IEnumerable<T> Two<T>(this IEnumerable<T> source)
    {
        using var e = source.GetEnumerator();
        if (!e.MoveNext())
            throw new ArgumentException("There are no elements", nameof(source));
        T firstValue = e.Current;
        if (!e.MoveNext())
            throw new ArgumentException("There is only one element", nameof(source));
        T secondValue = e.Current;
        if (e.MoveNext())
            throw new ArgumentException("There are more than two elements", nameof(source));
        yield return firstValue;
        yield return secondValue;
    }
    
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source)
    {
        return source.Where(value => value is not null)!;
    }


    public static IEnumerable<T> OrderBy<T>(
        this IEnumerable<T> enumerable,
        T[] itemOrder)
        where T : IEquatable<T>
    {
        return enumerable.OrderBy(itemOrder.FirstIndexOf);
    }

    public static IEnumerable<T> OrderBy<T, TSub>(this IEnumerable<T> enumerable,
        Func<T, TSub> selectSub,
        TSub[] subItemOrder)
        where TSub : IEquatable<TSub>
    {
        return enumerable
            .OrderBy(selectSub, Relate.Compare.CreateComparer<TSub>((x, y) =>
            {
                var xIndex = subItemOrder.FirstIndexOf(x);
                var yIndex = subItemOrder.FirstIndexOf(y);
                return xIndex.CompareTo(yIndex);
            }));
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

#if !NET6_0_OR_GREATER
    public static bool TryGetNonEnumeratedCount<T>(this IEnumerable<T> enumerable, out int count)
    {
        if (enumerable is ICollection<T> collection)
        {
            count = collection.Count;
            return true;
        }
        if (enumerable is IReadOnlyCollection<T> roCollection)
        {
            count = roCollection.Count;
            return true;
        }
        count = 0;
        return false;
    }
#endif
    
    /// <summary>
    /// A deep wrapper for <see cref="IEnumerable{T}"/> that ignores all thrown exceptions
    /// at every level of enumeration, only returning values that could be acquired without error
    /// </summary>
    public static IEnumerable<T> Swallowed<T>(this IEnumerable<T>? enumerable)
    {
        if (enumerable is null) yield break;
        IEnumerator<T> enumerator;
        try
        {
            enumerator = enumerable.GetEnumerator();
        }
        catch (Exception)
        {
            yield break;
        }
       
        while (true)
        {
            // try to move next
            try
            {
                if (!enumerator.MoveNext())
                {
                    // stop enumerating
                    enumerator.Dispose();
                    yield break;
                }
            }
            catch (Exception)
            {
                // ignore this, stop enumerating
                enumerator.Dispose();
                yield break;
            }

            // try to get current value
            T current;
            try
            {
                current = enumerator.Current;
            }
            catch (Exception)
            {
                // ignore this, continue enumerating
                continue;
            }

            // finally, yield the next value
            yield return current;
        }
    }
}