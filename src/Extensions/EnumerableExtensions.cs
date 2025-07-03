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
    /// <typeparam name="I"></typeparam>
    /// <typeparam name="O"></typeparam>
    public delegate bool SelectWherePredicate<in I, O>(I input, out O output);

    public static IEnumerable<O> SelectWhere<I, O>(this IEnumerable<I> enumerable, SelectWherePredicate<I, O> selectWherePredicate)
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

#pragma warning restore S3776

#if NETSTANDARD2_0
    public static HashSet<T> ToHashSet<T>(this IEnumerable<T> enumerable)
        => new(enumerable);
#endif
}
