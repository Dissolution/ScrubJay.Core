namespace ScrubJay.Extensions;

/// <summary>
/// Extensions on <see cref="Span{T}"/>
/// </summary>
public static class SpanExtensions
{
    public delegate void RefItem<T>(ref T item);

    public static void ForEach<T>(this Span<T> span, RefItem<T> perItem)
    {
        for (var i = 0; i < span.Length; i++)
        {
            perItem(ref span[i]);
        }
    }

#region Equal
#if !NET6_0_OR_GREATER
    public static bool SequenceEqual<T>(this Span<T> left, ReadOnlySpan<T> right)
        => SequenceEqual<T>((ReadOnlySpan<T>)left, right);
    
    public static bool SequenceEqual<T>(this ReadOnlySpan<T> left, ReadOnlySpan<T> right)
    {
        int firstLen = left.Length;
        if (right.Length != firstLen) return false;
        for (var i = 0; i < firstLen; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(left[i], right[i])) return false;
        }

        return true;
    }

    public static bool SequenceEqual<T>(this Span<T> left, ReadOnlySpan<T> right, IEqualityComparer<T>? itemComparer)
        => SequenceEqual<T>((ReadOnlySpan<T>)left, right, itemComparer);
    
    public static bool SequenceEqual<T>(this ReadOnlySpan<T> left, ReadOnlySpan<T> right, IEqualityComparer<T>? itemComparer)
    {
        int firstLen = left.Length;
        if (right.Length != firstLen) return false;
        itemComparer ??= EqualityComparer<T>.Default;
        for (var i = 0; i < firstLen; i++)
        {
            if (!itemComparer.Equals(left[i], right[i])) return false;
        }

        return true;
    }
#endif

    public static bool SequenceEqual<T>(this Span<T> left, IEnumerable<T>? right)
        => SequenceEqual<T>((ReadOnlySpan<T>)left, right);

    public static bool SequenceEqual<T>(this ReadOnlySpan<T> left, IEnumerable<T>? right, IEqualityComparer<T>? itemComparer = null)
    {
        if (right is null) return false;
        itemComparer ??= EqualityComparer<T>.Default;
        int count = left.Length;
        switch (right)
        {
            case IList<T> list:
            {
                if (list.Count != count) return false;
                for (var i = 0; i < count; i++)
                {
                    if (!itemComparer.Equals(left[i], list[i]))
                        return false;
                }

                return true;
            }
            case IReadOnlyList<T> roList:
            {
                if (roList.Count != count) return false;
                for (var i = 0; i < count; i++)
                {
                    if (!itemComparer.Equals(left[i], roList[i]))
                        return false;
                }

                return true;
            }
            case ICollection<T> collection:
            {
                if (collection.Count != count) return false;
                using var e = collection.GetEnumerator();
                for (var i = 0; i < count; i++)
                {
                    e.MoveNext();
                    if (!itemComparer.Equals(left[i], e.Current))
                        return false;
                }

                return true;
            }
            case IReadOnlyCollection<T> roCollection:
            {
                if (roCollection.Count != count) return false;
                using var e = roCollection.GetEnumerator();
                for (var i = 0; i < count; i++)
                {
                    e.MoveNext();
                    if (!itemComparer.Equals(left[i], e.Current))
                        return false;
                }

                return true;
            }
            default:
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
    }
#endregion

#region Compare
    public static int SequenceCompareTo<T>(this Span<T> left, ReadOnlySpan<T> right)
        => SequenceCompareTo<T>((ReadOnlySpan<T>)left, right);

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

    public static int SequenceCompareTo<T>(this Span<T> left, ReadOnlySpan<T> right, IComparer<T>? comparer)
        => SequenceCompareTo<T>((ReadOnlySpan<T>)left, right, comparer);

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

    public static int SequenceCompareTo<T>(this Span<T> left, IEnumerable<T>? right)
        => SequenceCompareTo<T>((ReadOnlySpan<T>)left, right);

    public static int SequenceCompareTo<T>(this ReadOnlySpan<T> left, IEnumerable<T>? right, IComparer<T>? comparer = null)
    {
        if (right is null) return 1; // null sorts first
        comparer ??= Comparer<T>.Default;
        int count = left.Length;
        switch (right)
        {
            case IList<T> list:
            {
                int minLength = Math.Min(count, list.Count);
                int c;
                for (int i = 0; i < minLength; i++)
                {
                    c = comparer.Compare(left[i], list[i]);
                    if (c != 0)
                        return c;
                }
                return count.CompareTo(list.Count);
            }
            case IReadOnlyList<T> roList:
            {
                int minLength = Math.Min(count, roList.Count);
                int c;
                for (int i = 0; i < minLength; i++)
                {
                    c = comparer.Compare(left[i], roList[i]);
                    if (c != 0)
                        return c;
                }
                return count.CompareTo(roList.Count);
            }
            case ICollection<T> collection:
            {
                int minLength = Math.Min(count, collection.Count);
                int c;
                using var e = collection.GetEnumerator();
                for (int i = 0; i < minLength; i++)
                {
                    e.MoveNext();
                    c = comparer.Compare(left[i], e.Current);
                    if (c != 0)
                        return c;
                }
                return count.CompareTo(collection.Count);
            }
            case IReadOnlyCollection<T> roCollection:
            {
                int minLength = Math.Min(count, roCollection.Count);
                int c;
                using var e = roCollection.GetEnumerator();
                for (int i = 0; i < minLength; i++)
                {
                    e.MoveNext();
                    c = comparer.Compare(left[i], e.Current);
                    if (c != 0)
                        return c;
                }
                return count.CompareTo(roCollection.Count);
            }
            default:
            {
                int c;
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
    }
#endregion

    public static bool TryGet<T>(this Span<T> span, int index, [MaybeNullWhen(false)] out T item)
    {
        if ((uint)index < span.Length)
        {
            item = span[index];
            return true;
        }

        item = default;
        return false;
    }

    public static bool TryGet<T>(this ReadOnlySpan<T> span, int index, [MaybeNullWhen(false)] out T item)
    {
        if ((uint)index < span.Length)
        {
            item = span[index];
            return true;
        }

        item = default;
        return false;
    }
    
    public static Result<T, Exception> TryGet<T>(this Span<T> span, 
        int index,
        [CallerArgumentExpression(nameof(span))]
        string? spanName = null,
        [CallerArgumentExpression(nameof(index))]
        string? indexName = null)
    {
        if (span.Length == 0)
            return new ArgumentException($"{spanName} is empty", spanName);
        if (index < 0 || index >= span.Length)
            return new ArgumentOutOfRangeException(indexName, index, $"{indexName} must be between 0 and {span.Length - 1}");
        return span[index];
    }
    
    public static Result<None, Exception> TrySet<T>(this Span<T> span, 
        int index,
        T value,
        [CallerArgumentExpression(nameof(span))]
        string? spanName = null,
        [CallerArgumentExpression(nameof(index))]
        string? indexName = null)
    {
        if (span.Length == 0)
            return new ArgumentException($"{spanName} is empty", spanName);
        if (index < 0 || index >= span.Length)
            return new ArgumentOutOfRangeException(indexName, index, $"{indexName} must be between 0 and {span.Length - 1}");
        span[index] = value;
        return GlobalHelper.None;
    }


    public static bool StartsWith<T>(this Span<T> span, ReadOnlySpan<T> slice)
    {
        int sliceLen = slice.Length;
        if (sliceLen > span.Length) return false;
        for (var i = 0; i < sliceLen; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(span[i], slice[i])) return false;
        }

        return true;
    }

    public static bool StartsWith<T>(this Span<T> span, ReadOnlySpan<T> slice, IEqualityComparer<T>? itemComparer)
    {
        int sliceLen = slice.Length;
        if (sliceLen > span.Length) return false;
        itemComparer ??= EqualityComparer<T>.Default;
        for (var i = 0; i < sliceLen; i++)
        {
            if (!itemComparer.Equals(span[i], slice[i])) return false;
        }

        return true;
    }

    public static bool StartsWith<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> slice)
    {
        int sliceLen = slice.Length;
        if (sliceLen > span.Length) return false;
        for (var i = 0; i < sliceLen; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(span[i], slice[i])) return false;
        }

        return true;
    }

    public static bool StartsWith<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> slice, IEqualityComparer<T>? itemComparer)
    {
        int sliceLen = slice.Length;
        if (sliceLen > span.Length) return false;
        itemComparer ??= EqualityComparer<T>.Default;
        for (var i = 0; i < sliceLen; i++)
        {
            if (!itemComparer.Equals(span[i], slice[i])) return false;
        }

        return true;
    }

    public static bool Contains<T>(this Span<T> span, T item)
    {
        int spanLen = span.Length;
        for (var i = 0; i < spanLen; i++)
        {
            if (EqualityComparer<T>.Default.Equals(span[i], item))
                return true;
        }

        return false;
    }

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

    public static bool Contains<T>(this ReadOnlySpan<T> span, T item)
    {
        int spanLen = span.Length;
        for (var i = 0; i < spanLen; i++)
        {
            if (EqualityComparer<T>.Default.Equals(span[i], item))
                return true;
        }

        return false;
    }

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