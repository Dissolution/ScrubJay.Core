#pragma warning disable S3776, MA0051

using ScrubJay.Comparison;

namespace ScrubJay.Extensions;

/// <summary>
/// Extensions on <see cref="Span{T}"/>
/// </summary>
[PublicAPI]
public static class SpanExtensions
{
    /// <summary>
    /// Performs the given <paramref name="perItem"/> action on each item in the <see cref="Span{T}"/>
    /// </summary>
    /// <param name="span">
    /// The <see cref="Span{T}"/> of items to perform the <see cref="RefItem{T}"/> delegate upon
    /// </param>
    /// <param name="perItem">
    /// The <see cref="RefItem{T}"/> delegate to perform on each item of the <see cref="Span{T}"/>
    /// </param>
    /// <typeparam name="T">
    /// The <see cref="Type"/> of items in the <see cref="Span{T}"/>
    /// </typeparam>
    public static void ForEach<T>(this Span<T> span, RefItem<T> perItem)
    {
        for (var i = 0; i < span.Length; i++)
        {
            perItem(ref span[i]);
        }
    }

#region Equal

#if !NET6_0_OR_GREATER
    public static bool SequenceEqual<T>(this Span<T> left, ReadOnlySpan<T> right, IEqualityComparer<T>? itemComparer = null)
        => SequenceEqual<T>((ReadOnlySpan<T>)left, right, itemComparer);
    
    public static bool SequenceEqual<T>(this ReadOnlySpan<T> left, ReadOnlySpan<T> right, IEqualityComparer<T>? itemComparer = null)
    {
        // If the spans differ in length, they're not equal.
        if (left.Length != right.Length)
        {
            return false;
        }

        // Use the comparer to compare each element.
        itemComparer ??= EqualityComparer<T>.Default;
        for (int i = 0; i < left.Length; i++)
        {
            if (!itemComparer.Equals(left[i], right[i]))
            {
                return false;
            }
        }
        return true;
    }
#endif

    public static bool SequenceEqual<T>(this Span<T> left, IEnumerable<T>? right, IEqualityComparer<T>? itemComparer = null) 
        => SequenceEqual<T>((ReadOnlySpan<T>)left, right, itemComparer);
    
    // ReSharper disable once CognitiveComplexity
    public static bool SequenceEqual<T>(this ReadOnlySpan<T> left, IEnumerable<T>? right, IEqualityComparer<T>? itemComparer = null)
    {
        if (right is null)
            return false;
        itemComparer ??= EqualityComparer<T>.Default;
        int count = left.Length;

        if (right is ICollection<T> collection)
        {
            if (right is IList<T> list)
            {
                if (list.Count != count)
                    return false;
                for (var i = 0; i < count; i++)
                {
                    if (!itemComparer.Equals(left[i], list[i]))
                        return false;
                }

                return true;
            }

            if (collection.Count != count)
                return false;
            using var e = collection.GetEnumerator();
            for (var i = 0; i < count; i++)
            {
                e.MoveNext();
                if (!itemComparer.Equals(left[i], e.Current))
                    return false;
            }

            return true;
        }
        else // have to enumerate
        {
            using var e = right.GetEnumerator();
            for (var i = 0; i < count; i++)
            {
                if (!e.MoveNext())
                    return false;
                if (!itemComparer.Equals(left[i], e.Current))
                    return false;
            }

            if (e.MoveNext())
                return false;
            return true;
        }
    }

#endregion

#region Compare

    public static int SequenceCompareTo<T>(this Span<T> left, ReadOnlySpan<T> right) => SequenceCompareTo<T>((ReadOnlySpan<T>)left, right);

    public static int SequenceCompareTo<T>(this ReadOnlySpan<T> left, ReadOnlySpan<T> right)
    {
        int minLength = Math.Min(left.Length, right.Length);
        int c;
        for (int i = 0; i < minLength; i++)
        {
            c = Comparer<T>.Default.Compare(left[i], right[i]);
            if (c != 0)
                return c;
        }

        return left.Length.CompareTo(right.Length);
    }

    public static int SequenceCompareTo<T>(this Span<T> left, ReadOnlySpan<T> right, IComparer<T>? comparer) => SequenceCompareTo<T>((ReadOnlySpan<T>)left, right, comparer);

    public static int SequenceCompareTo<T>(this ReadOnlySpan<T> left, ReadOnlySpan<T> right, IComparer<T>? comparer)
    {
        comparer ??= Comparer<T>.Default;
        int minLength = Math.Min(left.Length, right.Length);
        int c;
        for (int i = 0; i < minLength; i++)
        {
            c = comparer.Compare(left[i], right[i]);
            if (c != 0)
                return c;
        }

        return left.Length.CompareTo(right.Length);
    }

    public static int SequenceCompareTo<T>(this Span<T> left, IEnumerable<T>? right) => SequenceCompareTo<T>((ReadOnlySpan<T>)left, right);
    
    // ReSharper disable once CognitiveComplexity
    public static int SequenceCompareTo<T>(this ReadOnlySpan<T> left, IEnumerable<T>? right, IComparer<T>? comparer = null)
    {
        if (right is null)
            return 1; // null sorts first

        comparer ??= Comparer<T>.Default;
        int c;

        int count = left.Length;
        if (right is ICollection<T> collection)
        {
            c = count.CompareTo(collection.Count);
            if (c != 0)
                return c;

            if (right is IList<T> list)
            {
                for (int i = 0; i < count; i++)
                {
                    c = comparer.Compare(left[i], list[i]);
                    if (c != 0)
                        return c;
                }
            }
            else
            {
                using var e = collection.GetEnumerator();
                for (int i = 0; i < count; i++)
                {
#if DEBUG
                    bool moved = e.MoveNext();
                    Debug.Assert(moved);
#else
                    e.MoveNext();
#endif
                    c = comparer.Compare(left[i], e.Current);
                    if (c != 0)
                        return c;
                }
            }

            return 0; // equal
        }
        else
        {
            using var e = right.GetEnumerator();
            for (int i = 0; i < count; i++)
            {
                if (!e.MoveNext())
                {
                    // right is shorter
                    return 1;
                }

                c = comparer.Compare(left[i], e.Current);
                if (c != 0)
                    return c;
            }

            if (e.MoveNext())
            {
                // right is longer
                return -1;
            }

            // same
            return 0;
        }
    }

#endregion

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> TryGet<T>(this Span<T> span, Index index)
    {
        int offset = index.GetOffset(span.Length);
        if (offset < 0 || offset >= span.Length)
            return None();
        return Some(span[offset]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> TryGet<T>(this ReadOnlySpan<T> span, Index index)
    {
        int offset = index.GetOffset(span.Length);
        if (offset < 0 || offset >= span.Length)
            return None();
        return Some(span[offset]);
    }

    public static bool TryGet<T>(this Span<T> span, Range range, out Span<T> slice)
    {
        if (!Validate.Range(range, span.Length).IsOk(out var offsetLength))
        {
            slice = default;
            return false;
        }

        slice = span.Slice(offsetLength.Offset, offsetLength.Length);
        return true;
    }

    public static bool TryGet<T>(this ReadOnlySpan<T> span, Range range, out ReadOnlySpan<T> slice)
    {
        if (!Validate.Range(range, span.Length).IsOk(out var offsetLength))
        {
            slice = default;
            return false;
        }

        slice = span.Slice(offsetLength.Offset, offsetLength.Length);
        return true;
    }

    /// <summary>
    /// Tries to set the item at an <see cref="Index"/> with a <paramref name="value"/>
    /// </summary>
    /// <param name="span"></param>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool TrySet<T>(this Span<T> span, Index index, T value)
    {
        int offset = index.GetOffset(span.Length);
        if (offset < 0 || offset >= span.Length)
            return false;
        span[offset] = value;
        return true;
    }

    public static bool StartsWith<T>(this Span<T> span, ReadOnlySpan<T> slice, IEqualityComparer<T>? itemComparer)
    {
        int sliceLen = slice.Length;
        if (sliceLen > span.Length)
            return false;
        return Equate.Sequence<T>(span[..sliceLen], slice, itemComparer);
    }

    public static bool StartsWith<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> slice, IEqualityComparer<T>? itemComparer)
    {
        int sliceLen = slice.Length;
        if (sliceLen > span.Length)
            return false;
        return Equate.Sequence<T>(span[..sliceLen], slice, itemComparer);
    }

    /// <summary>
    /// Does this <see cref="Span{T}"/> contain the given <paramref name="item"/> as determined by an <see cref="IEqualityComparer{T}"/>?
    /// </summary>
    /// <param name="span"></param>
    /// <param name="item"></param>
    /// <param name="itemComparer"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool Contains<T>(this Span<T> span, T item, IEqualityComparer<T>? itemComparer)
    {
        int spanLen = span.Length;
        itemComparer ??= EqualityComparer<T>.Default;
        for (var i = 0; i < spanLen; i++)
        {
            if (itemComparer.Equals(span[i], item))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Does this <see cref="ReadOnlySpan{T}"/> contain the given <paramref name="item"/> as determined by an <see cref="IEqualityComparer{T}"/>?
    /// </summary>
    /// <param name="span"></param>
    /// <param name="item"></param>
    /// <param name="itemComparer"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool Contains<T>(this ReadOnlySpan<T> span, T item, IEqualityComparer<T>? itemComparer)
    {
        int spanLen = span.Length;
        itemComparer ??= EqualityComparer<T>.Default;
        for (var i = 0; i < spanLen; i++)
        {
            if (itemComparer.Equals(span[i], item))
                return true;
        }

        return false;
    }
}