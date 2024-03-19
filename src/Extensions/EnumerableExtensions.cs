namespace ScrubJay.Extensions;

/// <summary>
/// Extensions on <see cref="IEnumerable{T}"/> and <see cref="IEnumerable"/>
/// </summary>
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
    public static IEnumerable<TOut> SelectWhere<TIn, TOut>(
        this IEnumerable<TIn> enumerable,
        Func<TIn, Option<TOut>> selectWhere)
    {
        return enumerable.SelectMany(i => selectWhere(i));
    }

//    public static IEnumerable<TOut> SelectWhere<TIn, TOut>(
//        this IEnumerable<TIn> enumerable,
//        Func<TIn, Result<TOut>> selectWhere)
//    {
//        return enumerable.SelectMany(i => selectWhere(i));
//    }

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


    public static IEnumerable<T> OrderBy<T>(
        this IEnumerable<T> enumerable,
        T[] itemOrder)
        where T : IEquatable<T>
    {
        return enumerable.OrderBy(e => Array.IndexOf(itemOrder, e));
    }

//    public static IEnumerable<T> OrderBy<T, TSub>(this IEnumerable<T> enumerable,
//        Func<T, TSub> selectSub,
//        TSub[] subItemOrder)
//        where TSub : IEquatable<TSub>
//    {
//        return enumerable
//            .OrderBy(selectSub, Relate.Compare.CreateComparer<TSub>((x, y) =>
//            {
//                var xIndex = subItemOrder.FirstIndexOf(x);
//                var yIndex = subItemOrder.FirstIndexOf(y);
//                return xIndex.CompareTo(yIndex);
//            }));
//    }

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

    /// <summary>
    /// Split the elements of a sequence into chunks of size at most <paramref name="size"/>.
    /// </summary>
    /// <remarks>
    /// Every chunk except the last will be of size <paramref name="size"/>.
    /// The last chunk will contain the remaining elements and may be of a smaller size.
    /// </remarks>
    /// <param name="source">
    /// An <see cref="IEnumerable{T}"/> whose elements to chunk.
    /// </param>
    /// <param name="size">
    /// Maximum size of each chunk.
    /// </param>
    /// <typeparam name="TSource">
    /// The type of the elements of source.
    /// </typeparam>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> that contains the elements the input sequence split into chunks of size <paramref name="size"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="source"/> is null.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="size"/> is below 1.
    /// </exception>
    public static IEnumerable<TSource[]> Chunk<TSource>(this IEnumerable<TSource> source, int size)
    {
        if (source is null)
            throw new ArgumentNullException(nameof(source));
        if (size < 1)
            throw new ArgumentOutOfRangeException(nameof(size));
        return ChunkIterator(source, size);
    }

    private static IEnumerable<TSource[]> ChunkIterator<TSource>(IEnumerable<TSource> source, int size)
    {
        using IEnumerator<TSource> e = source.GetEnumerator();

        // Before allocating anything, make sure there's at least one element.
        if (e.MoveNext())
        {
            // Now that we know we have at least one item, allocate an initial storage array. This is not
            // the array we'll yield.  It starts out small in order to avoid significantly overallocating
            // when the source has many fewer elements than the chunk size.
            int arraySize = Math.Min(size, 4);
            int i;
            do
            {
                var array = new TSource[arraySize];

                // Store the first item.
                array[0] = e.Current;
                i = 1;

                if (size != array.Length)
                {
                    // This is the first chunk. As we fill the array, grow it as needed.
                    for (; i < size && e.MoveNext(); i++)
                    {
                        if (i >= array.Length)
                        {
                            arraySize = (int)Math.Min((uint)size, 2 * (uint)array.Length);
                            Array.Resize(ref array, arraySize);
                        }

                        array[i] = e.Current;
                    }
                }
                else
                {
                    // For all but the first chunk, the array will already be correctly sized.
                    // We can just store into it until either it's full or MoveNext returns false.
                    TSource[] local = array; // avoid bounds checks by using cached local (`array` is lifted to iterator object as a field)
                    //Debug.Assert(local.Length == size);
                    for (; (uint)i < (uint)local.Length && e.MoveNext(); i++)
                    {
                        local[i] = e.Current;
                    }
                }

                if (i != array.Length)
                {
                    Array.Resize(ref array, i);
                }

                yield return array;
            } while (i >= size && e.MoveNext());
        }
    }

#endif

    /// <summary>
    /// A deep wrapper for <see cref="IEnumerable{T}"/> that ignores all thrown exceptions
    /// at every level of enumeration, only returning values that could be acquired without error
    /// </summary>
    public static IEnumerable<T> Swallowed<T>(this IEnumerable<T>? enumerable)
    {
        if (enumerable is null) yield break;
        IEnumerator<T>? enumerator;
        try
        {
            enumerator = enumerable.GetEnumerator();
        }
        catch (Exception)
        {
            yield break;
        }

        if (enumerator is null)
            yield break;

        while (true)
        {
            // try to move next
            try
            {
                if (!enumerator.MoveNext())
                {
                    // stop enumerating
                    enumerator.SafeDispose();
                    yield break;
                }
            }
            catch (Exception)
            {
                // ignore this, stop enumerating
                enumerator.SafeDispose();
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

            // finally, yield the value
            yield return current;
        }
    }
}