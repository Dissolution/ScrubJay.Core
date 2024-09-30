using ScrubJay.Collections;

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
    /// Returns an <see cref="Option{T}"/> possibly containing the first value in this <see cref="IEnumerable{T}"/>
    /// </summary>
    /// <returns>
    /// A <see cref="Option{T}.Some"/> containing the first value or<br/>
    /// A <see cref="Option{T}.None"/> if <paramref name="enumerable"/> is <c>null</c> or has no values
    /// </returns>
    public static Option<T> FirstOption<T>(this IEnumerable<T>? enumerable)
    {
        if (enumerable is null) return None();
        using var e = enumerable.GetEnumerator();
        return !e.MoveNext() ? None() : Some(e.Current);
    }

    /// <summary>
    /// Returns an <see cref="Option{T}"/> possibly containing the minimum value in an <see cref="IEnumerable{T}"/>
    /// </summary>
    /// <param name="enumerable">
    /// The <see cref="IEnumerable{T}"/> to get the minimum value of<br/>
    /// If <c>null</c>, <see cref="None"/> will be returned
    /// </param>
    /// <param name="itemComparer">
    /// The optional <see cref="IComparer{T}"/> to use for item comparison
    /// </param>
    /// <returns>
    /// A <see cref="Option{T}.Some"/> containing the minimum value or<br/>
    /// A <see cref="Option{T}.None"/> if <paramref name="enumerable"/> is <c>null</c> or has no values
    /// </returns>
    public static Option<T> MinOption<T>(this IEnumerable<T>? enumerable, IComparer<T>? itemComparer = null)
    {
        if (enumerable is null) return None();
        using var e = enumerable.GetEnumerator();
        if (!e.MoveNext()) return None();
        itemComparer ??= Comparer<T>.Default;
        T min = e.Current;
        while (e.MoveNext())
        {
            if (itemComparer.Compare(e.Current, min) < 0)
            {
                min = e.Current;
            }
        }
        return Some(min);
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
                for (var i = 0; i < list.Count; i++)
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

#pragma warning restore S3776

    /// <summary>
    /// A deep wrapper for <see cref="IEnumerable{T}"/> that ignores all thrown exceptions
    /// at every level of enumeration, only returning values that could be acquired without error
    /// </summary>
    public static UnbreakableEnumerable<T> UnbreakableEnumerate<T>(this IEnumerable<T>? enumerable) => new UnbreakableEnumerable<T>(enumerable);
}