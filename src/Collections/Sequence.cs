﻿namespace ScrubJay.Collections;

[PublicAPI]
public static class Sequence
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SelfCopy<T>(scoped Span<T> span, Range source, Range destination)
    {
        span[source].CopyTo(span[destination]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SelfCopy<T>(T[] array, Range source, Range destination)
    {
        array.AsSpan(source).CopyTo(array.AsSpan(destination));
    }

#region TryCopyTo

    /* Permutations of:
     * Input:
     * ReadOnlySpan<T>, T[]?, IEnumerable<T>, string?*
     * Output:
     * Span<T>, T[]?, IList<T>,
     */


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryCopyTo<T>(scoped ReadOnlySpan<T> source, Span<T> destination)
    {
        return source.TryCopyTo(destination);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryCopyTo<T>(scoped ReadOnlySpan<T> source, T[]? destination)
    {
        return source.TryCopyTo(new Span<T>(destination));
    }

    public static bool TryCopyTo<T>(scoped ReadOnlySpan<T> source, IList<T>? destination)
    {
        int sourceCount = source.Length;
        if (destination is null)
            return sourceCount == 0;
        if (sourceCount > destination.Count)
            return false;
        for (var i = 0; i < sourceCount; i++)
        {
            destination[i] = source[i];
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryCopyTo<T>(T[]? source, Span<T> destination) => TryCopyTo(new ReadOnlySpan<T>(source), destination);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryCopyTo<T>(T[]? source, T[]? destination) => TryCopyTo(new ReadOnlySpan<T>(source), new Span<T>(destination));

    public static bool TryCopyTo<T>(T[]? source, IList<T>? destination) => TryCopyTo(new ReadOnlySpan<T>(source), destination);


    public static bool TryCopyTo<T>(IEnumerable<T>? source, Span<T> destination, bool clearOnFailure = true)
    {
        if (source is null)
            return true;
        int destinationLength = destination.Length;

        if (source is ICollection<T> collection)
        {
            int sourceCount = collection.Count;
            if (sourceCount >= destinationLength)
                return false;

            int i = 0;
            foreach (var item in collection)
            {
                destination[i++] = item;
            }
        }
        else
        {
            int i = 0;
            foreach (var item in source)
            {
                if (i >= destinationLength)
                {
                    if (clearOnFailure)
                        destination.Clear();
                    return false;
                }

                destination[i++] = item;
            }
        }

        return true;
    }

    public static bool TryCopyTo<T>(IEnumerable<T>? source, T[]? destination, bool clearOnFailure = true)
    {
        if (source is null)
            return true;

        if (source is ICollection<T> collection)
        {
            int sourceCount = collection.Count;
            if (destination is null)
                return sourceCount == 0;
            int destinationLength = destination.Length;
            if (sourceCount >= destinationLength)
                return false;

            collection.CopyTo(destination, 0);
        }
        else
        {
            if (destination is null)
                return false;
            int destinationLength = destination.Length;
            int i = 0;
            foreach (var item in source)
            {
                if (i >= destinationLength)
                {
                    if (clearOnFailure)
                        Array.Clear(destination, 0, destinationLength);
                    return false;
                }

                destination[i++] = item;
            }
        }

        return true;
    }

    public static bool TryCopyTo<T>(IEnumerable<T>? source, IList<T>? destination, bool clearOnFailure = true)
    {
        if (source is null)
            return true;

        if (source is ICollection<T> collection)
        {
            int sourceCount = collection.Count;
            if (destination is null)
                return sourceCount == 0;
            int destinationLength = destination.Count;
            if (sourceCount >= destinationLength)
                return false;

            int i = 0;
            foreach (var item in source)
            {
                destination[i++] = item;
            }
        }
        else
        {
            if (destination is null)
                return false;
            int destinationLength = destination.Count;
            int i = 0;
            foreach (var item in source)
            {
                if (i >= destinationLength)
                {
                    if (clearOnFailure)
                        destination.Clear();
                    return false;
                }

                destination[i++] = item;
            }
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryCopyTo(string? source, Span<char> destination) => TryCopyTo(source.AsSpan(), destination);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryCopyTo(string? source, char[]? destination) => TryCopyTo(source.AsSpan(), new Span<char>(destination));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryCopyTo(string? source, IList<char>? destination) => TryCopyTo(source.AsSpan(), destination);

#endregion

#region CopyTo

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo<T>(scoped ReadOnlySpan<T> source, Span<T> destination)
    {
        source.CopyTo(destination);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo<T>(scoped ReadOnlySpan<T> source, T[]? destination)
    {
        source.CopyTo(new Span<T>(destination));
    }

    public static void CopyTo<T>(scoped ReadOnlySpan<T> source, IList<T>? destination)
    {
        int sourceCount = source.Length;
        if (destination is null)
        {
            if (sourceCount == 0)
                return;
            throw new ArgumentNullException(nameof(destination));
        }

        if (sourceCount > destination.Count)
            throw new ArgumentException($"Source count of {sourceCount} will not fit in destination length of {destination.Count}", nameof(destination));

        for (var i = 0; i < sourceCount; i++)
        {
            destination[i] = source[i];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo<T>(T[]? source, Span<T> destination) => CopyTo(new ReadOnlySpan<T>(source), destination);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo<T>(T[]? source, T[]? destination) => CopyTo(new ReadOnlySpan<T>(source), new Span<T>(destination));

    public static void CopyTo<T>(T[]? source, IList<T>? destination) => CopyTo(new ReadOnlySpan<T>(source), destination);


    public static void CopyTo<T>(IEnumerable<T>? source, Span<T> destination, bool clearOnFailure = true)
    {
        if (source is null)
            return;

        int destinationLength = destination.Length;

        if (source is ICollection<T> collection)
        {
            int sourceCount = collection.Count;
            if (sourceCount > destinationLength)
                throw new ArgumentException($"Source count of {sourceCount} will not fit in destination length of {destinationLength}", nameof(destination));

            int i = 0;
            foreach (var item in collection)
            {
                destination[i++] = item;
            }
        }
        else
        {
            int i = 0;
            foreach (var item in source)
            {
                if (i >= destinationLength)
                {
                    if (clearOnFailure)
                        destination.Clear();
                    throw new ArgumentException($"Source count of at least {i} will not fit in destination length of {destinationLength}", nameof(destination));
                }

                destination[i++] = item;
            }
        }
    }

    public static void CopyTo<T>(IEnumerable<T>? source, T[]? destination, bool clearOnFailure = true)
    {
        if (source is null)
            return;

        if (source is ICollection<T> collection)
        {
            int sourceCount = collection.Count;
            if (destination is null)
            {
                if (sourceCount == 0)
                    return;
                throw new ArgumentNullException(nameof(destination));
            }

            int destinationLength = destination.Length;
            if (sourceCount > destinationLength)
                throw new ArgumentException($"Source count of {sourceCount} will not fit in destination length of {destinationLength}", nameof(destination));

            collection.CopyTo(destination, 0);
        }
        else
        {
            Validate.ThrowIfNull(destination);
            int destinationLength = destination.Length;
            int i = 0;
            foreach (var item in source)
            {
                if (i >= destinationLength)
                {
                    if (clearOnFailure)
                        Array.Clear(destination, 0, destinationLength);
                    throw new ArgumentException($"Source count of at least {i} will not fit in destination length of {destinationLength}", nameof(destination));
                }

                destination[i++] = item;
            }
        }
    }

    public static void CopyTo<T>(IEnumerable<T>? source, IList<T>? destination, bool clearOnFailure = true)
    {
        if (source is null)
            return;

        if (source is ICollection<T> collection)
        {
            int sourceCount = collection.Count;
            if (destination is null)
            {
                if (sourceCount == 0)
                    return;
                throw new ArgumentNullException(nameof(destination));
            }

            int destinationLength = destination.Count;
            if (sourceCount > destinationLength)
                throw new ArgumentException($"Source count of {sourceCount} will not fit in destination length of {destinationLength}", nameof(destination));

            int i = 0;
            foreach (var item in source)
            {
                destination[i++] = item;
            }
        }
        else
        {
            Validate.ThrowIfNull(destination);
            int destinationLength = destination.Count;
            int i = 0;
            foreach (var item in source)
            {
                if (i >= destinationLength)
                {
                    if (clearOnFailure)
                        destination.Clear();
                    throw new ArgumentException($"Source count of at least {i} will not fit in destination length of {destinationLength}", nameof(destination));
                }

                destination[i++] = item;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo(string? source, Span<char> destination) => CopyTo(source.AsSpan(), destination);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo(string? source, char[]? destination) => CopyTo(source.AsSpan(), new Span<char>(destination));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo(string? source, IList<char>? destination) => CopyTo(source.AsSpan(), destination);

#endregion


#region Compare

    /* comparision is supported for all combinations of:
     * T[], Span<T>, ReadOnlySpan<T>, IEnumerable<T>
     */


#region where T : IComparable<T>

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ComparableCompare<T>(T[]? left, T[]? right)
        where T : IComparable<T>
    {
        return MemoryExtensions.SequenceCompareTo<T>(new ReadOnlySpan<T>(left), new ReadOnlySpan<T>(right));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ComparableCompare<T>(T[]? left, Span<T> right)
        where T : IComparable<T>
    {
        return MemoryExtensions.SequenceCompareTo<T>(new ReadOnlySpan<T>(left), right);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ComparableCompare<T>(T[]? left, ReadOnlySpan<T> right)
        where T : IComparable<T>
    {
        return MemoryExtensions.SequenceCompareTo<T>(new ReadOnlySpan<T>(left), right);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ComparableCompare<T>(Span<T> left, T[]? right)
        where T : IComparable<T>
    {
        return MemoryExtensions.SequenceCompareTo<T>(left, new ReadOnlySpan<T>(right));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ComparableCompare<T>(Span<T> left, Span<T> right)
        where T : IComparable<T>
    {
        return MemoryExtensions.SequenceCompareTo<T>(left, right);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ComparableCompare<T>(Span<T> left, ReadOnlySpan<T> right)
        where T : IComparable<T>
    {
        return MemoryExtensions.SequenceCompareTo<T>(left, right);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ComparableCompare<T>(ReadOnlySpan<T> left, T[]? right)
        where T : IComparable<T>
    {
        return MemoryExtensions.SequenceCompareTo<T>(left, new ReadOnlySpan<T>(right));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ComparableCompare<T>(ReadOnlySpan<T> left, Span<T> right)
        where T : IComparable<T>
    {
        return MemoryExtensions.SequenceCompareTo<T>(left, right);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ComparableCompare<T>(ReadOnlySpan<T> left, ReadOnlySpan<T> right)
        where T : IComparable<T>
    {
        return MemoryExtensions.SequenceCompareTo<T>(left, right);
    }

#endregion

#region no restriction w/IComparer<T>

    public static int Compare<T>(T[]? left, T[]? right, IComparer<T>? itemComparer = null)
    {
        // null sorts first
        if (left is null)
            return right is null ? 0 : -1;
        if (right is null)
            return 1;

        return Compare<T>(new ReadOnlySpan<T>(left), new ReadOnlySpan<T>(right), itemComparer);
    }

    public static int Compare<T>(T[]? left, Span<T> right, IComparer<T>? itemComparer = null)
    {
        // null sorts first
        if (left is null)
            return -1;

        return Compare<T>(new ReadOnlySpan<T>(left), (ReadOnlySpan<T>)right, itemComparer);
    }

    public static int Compare<T>(T[]? left, ReadOnlySpan<T> right, IComparer<T>? itemComparer = null)
    {
        // null sorts first
        if (left is null)
            return -1;

        return Compare<T>(new ReadOnlySpan<T>(left), right, itemComparer);
    }

    public static int Compare<T>(T[]? left, IEnumerable<T>? right, IComparer<T>? itemComparer = null)
    {
        // null sorts first
        if (left is null)
            return right is null ? 0 : -1;
        if (right is null)
            return 1;

        return Compare<T>(new ReadOnlySpan<T>(left), right, itemComparer);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Compare<T>(Span<T> left, T[]? right, IComparer<T>? itemComparer = null) => Compare<T>((ReadOnlySpan<T>)left, right, itemComparer);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Compare<T>(Span<T> left, Span<T> right, IComparer<T>? itemComparer = null) => Compare<T>((ReadOnlySpan<T>)left, (ReadOnlySpan<T>)right, itemComparer);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Compare<T>(Span<T> left, ReadOnlySpan<T> right, IComparer<T>? itemComparer = null) => Compare<T>((ReadOnlySpan<T>)left, right, itemComparer);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Compare<T>(Span<T> left, IEnumerable<T>? right, IComparer<T>? itemComparer = null) => Compare<T>((ReadOnlySpan<T>)left, right, itemComparer);


    public static int Compare<T>(ReadOnlySpan<T> left, T[]? right, IComparer<T>? itemComparer = null)
    {
        // null sorts first
        if (right is null)
            return 1;

        return Compare<T>(left, new ReadOnlySpan<T>(right), itemComparer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Compare<T>(ReadOnlySpan<T> left, Span<T> right, IComparer<T>? itemComparer = null) => Compare<T>(left, (ReadOnlySpan<T>)right, itemComparer);

    public static int Compare<T>(ReadOnlySpan<T> left, ReadOnlySpan<T> right, IComparer<T>? itemComparer = null)
    {
        int minLength = Math.Min(left.Length, right.Length);
        itemComparer ??= Comparer<T>.Default;
        int c;
        for (int i = 0; i < minLength; i++)
        {
            c = itemComparer.Compare(left[i], right[i]);
            if (c != 0)
                return c;
        }

        return left.Length.CompareTo(right.Length);
    }

    public static int Compare<T>(ReadOnlySpan<T> left, IEnumerable<T>? right, IComparer<T>? itemComparer = null)
    {
        // null sorts first
        if (right is null)
            return 1;

        int c;
        int leftCount = left.Length;
        itemComparer ??= Comparer<T>.Default;

        if (right is ICollection<T> rightCollection)
        {
            int count = Math.Min(leftCount, rightCollection.Count);

            if (right is IList<T> rightList)
            {
                for (int i = 0; i < count; i++)
                {
                    c = itemComparer.Compare(left[i], rightList[i]);
                    if (c != 0)
                        return c;
                }

                return leftCount.CompareTo(rightList.Count);
            }

            // Right is just ICollection
            using var rightCollectionEnumerator = rightCollection.GetEnumerator();
            for (int i = 0; i < leftCount; i++)
            {
#if DEBUG
                bool moved = rightCollectionEnumerator.MoveNext();
                Debug.Assert(moved);
#else
                e.MoveNext();
#endif
                c = itemComparer.Compare(left[i], rightCollectionEnumerator.Current);
                if (c != 0)
                    return c;
            }

            return 0; // equal
        }
        else
        {
            using var e = right.GetEnumerator();
            for (int i = 0; i < leftCount; i++)
            {
                if (!e.MoveNext())
                {
                    // right is shorter
                    return 1;
                }

                c = itemComparer.Compare(left[i], e.Current);
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

    public static int Compare<T>(IList<T>? left, T[]? right, IComparer<T>? itemComparer = null)
    {
        // null sorts first
        if (left is null)
            return right is null ? 0 : -1;
        if (right is null)
            return 1;

        return Compare<T>(left, new ReadOnlySpan<T>(right), itemComparer);
    }

    public static int Compare<T>(IList<T>? left, Span<T> right, IComparer<T>? itemComparer = null)
    {
        // null sorts first
        if (left is null)
            return -1;

        return Compare<T>(left, (ReadOnlySpan<T>)right, itemComparer);
    }

    public static int Compare<T>(IList<T>? left, ReadOnlySpan<T> right, IComparer<T>? itemComparer = null)
    {
        // null sorts first
        if (left is null)
            return -1;

        int minLength = Math.Min(left.Count, right.Length);
        itemComparer ??= Comparer<T>.Default;
        int c;
        for (int i = 0; i < minLength; i++)
        {
            c = itemComparer.Compare(left[i], right[i]);
            if (c != 0)
                return c;
        }

        return left.Count.CompareTo(right.Length);
    }

    public static int Compare<T>(IList<T>? left, IList<T>? right, IComparer<T>? itemComparer = null)
    {
        // null sorts first
        if (left is null)
            return right is null ? 0 : -1;
        if (right is null)
            return 1;

        int minLength = Math.Min(left.Count, right.Count);
        itemComparer ??= Comparer<T>.Default;
        int c;
        for (int i = 0; i < minLength; i++)
        {
            c = itemComparer.Compare(left[i], right[i]);
            if (c != 0)
                return c;
        }

        return left.Count.CompareTo(right.Count);
    }

    public static int Compare<T>(IList<T>? left, IEnumerable<T>? right, IComparer<T>? itemComparer = null)
    {
        // null sorts first
        if (left is null)
            return right is null ? 0 : -1;
        if (right is null)
            return 1;

        int c;
        int count = left.Count;
        itemComparer ??= Comparer<T>.Default;

        if (right is ICollection<T> collection)
        {
            c = count.CompareTo(collection.Count);
            if (c != 0)
                return c;

            if (right is IList<T> list)
            {
                for (int i = 0; i < count; i++)
                {
                    c = itemComparer.Compare(left[i], list[i]);
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
                    c = itemComparer.Compare(left[i], e.Current);
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

                c = itemComparer.Compare(left[i], e.Current);
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


    public static int Compare<T>(IEnumerable<T>? left, T[]? right, IComparer<T>? itemComparer = null)
    {
        // null sorts first
        if (left is null)
            return right is null ? 0 : -1;
        if (right is null)
            return 1;

        return Compare<T>(left, new ReadOnlySpan<T>(right), itemComparer);
    }

    public static int Compare<T>(IEnumerable<T>? left, Span<T> right, IComparer<T>? itemComparer = null)
    {
        // null sorts first
        if (left is null)
            return -1;

        return Compare<T>(left, (ReadOnlySpan<T>)right, itemComparer);
    }

    public static int Compare<T>(IEnumerable<T>? left, ReadOnlySpan<T> right, IComparer<T>? itemComparer = null)
    {
        // null sorts first
        if (left is null)
            return -1;

        int c;
        int count = right.Length;
        itemComparer ??= Comparer<T>.Default;

        if (left is ICollection<T> collection)
        {
            c = count.CompareTo(collection.Count);
            if (c != 0)
                return c;

            if (left is IList<T> list)
            {
                for (int i = 0; i < count; i++)
                {
                    c = itemComparer.Compare(list[i], right[i]);
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
                    c = itemComparer.Compare(e.Current, right[i]);
                    if (c != 0)
                        return c;
                }
            }

            return 0; // equal
        }
        else
        {
            using var e = left.GetEnumerator();
            for (int i = 0; i < count; i++)
            {
                if (!e.MoveNext())
                {
                    // right is shorter
                    return 1;
                }

                c = itemComparer.Compare(e.Current, right[i]);
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

    public static int Compare<T>(IEnumerable<T>? left, IEnumerable<T>? right, IComparer<T>? itemComparer = null)
    {
        // null sorts first
        if (left is null)
            return right is null ? 0 : -1;
        if (right is null)
            return 1;

        int c;
        itemComparer ??= Comparer<T>.Default;

        if (left is ICollection<T> leftCollection && right is ICollection<T> rightCollection)
        {
            int count = leftCollection.Count;
            c = count.CompareTo(rightCollection.Count);
            if (c != 0)
                return c;

            if (leftCollection is IList<T> firstList && rightCollection is IList<T> secondList)
            {
                for (int i = 0; i < count; i++)
                {
                    c = itemComparer.Compare(firstList[i], secondList[i]);
                    if (c != 0)
                        return c;
                }

                return 0;
            }
            else
            {
                // Enumerate, but we know they enumerate the same number of times
                using IEnumerator<T> leftEnumerator = left.GetEnumerator();
                using IEnumerator<T> rightEnumerator = right.GetEnumerator();

                for (int i = 0; i < count; i++)
                {
#if DEBUG
                    bool leftMoved = leftEnumerator.MoveNext();
                    bool rightMoved = rightEnumerator.MoveNext();
                    Debug.Assert(leftMoved);
                    Debug.Assert(rightMoved);
#else
                    leftEnumerator.MoveNext();
                    rightEnumerator.MoveNext();
#endif
                    c = itemComparer.Compare(leftEnumerator.Current, rightEnumerator.Current);
                    if (c != 0)
                        return c;
                }

                return 0;
            }
        }
        else
        {
            // Enumerate
            using IEnumerator<T> leftEnumerator = left.GetEnumerator();
            using IEnumerator<T> rightEnumerator = right.GetEnumerator();

            while (true)
            {
                bool leftMoved = leftEnumerator.MoveNext();
                bool rightMoved = rightEnumerator.MoveNext();
                if (leftMoved != rightMoved)
                    return leftMoved ? 1 : -1; // different counts
                if (!leftMoved)
                    return 0; // same items, same count
                c = itemComparer.Compare(leftEnumerator.Current, rightEnumerator.Current);
                if (c != 0)
                    return c;
            }
        }
    }

#endregion

#endregion

#region Equal

#region T: IEquatable<T>

    public static bool EquatableEqual<T>(T[]? left, T[]? right)
        where T : IEquatable<T>
    {
        return MemoryExtensions.SequenceEqual<T>(new ReadOnlySpan<T>(left), new ReadOnlySpan<T>(right));
    }

    public static bool EquatableEqual<T>(T[]? left, Span<T> right)
        where T : IEquatable<T>
    {
        return MemoryExtensions.SequenceEqual<T>(new ReadOnlySpan<T>(left), (ReadOnlySpan<T>)right);
    }

    public static bool EquatableEqual<T>(T[]? left, ReadOnlySpan<T> right)
        where T : IEquatable<T>
    {
        return MemoryExtensions.SequenceEqual<T>(left.AsSpan(), right);
    }

    public static bool EquatableEqual<T>(Span<T> left, T[]? right)
        where T : IEquatable<T>
    {
        return MemoryExtensions.SequenceEqual<T>((ReadOnlySpan<T>)left, new ReadOnlySpan<T>(right));
    }

    public static bool EquatableEqual<T>(Span<T> left, Span<T> right)
        where T : IEquatable<T>
    {
        return MemoryExtensions.SequenceEqual<T>((ReadOnlySpan<T>)left, (ReadOnlySpan<T>)right);
    }

    public static bool EquatableEqual<T>(Span<T> left, ReadOnlySpan<T> right)
        where T : IEquatable<T>
    {
        return MemoryExtensions.SequenceEqual<T>((ReadOnlySpan<T>)left, right);
    }

    public static bool EquatableEqual<T>(ReadOnlySpan<T> left, T[]? right)
        where T : IEquatable<T>
    {
        return MemoryExtensions.SequenceEqual<T>(left, new ReadOnlySpan<T>(right));
    }

    public static bool EquatableEqual<T>(ReadOnlySpan<T> left, Span<T> right)
        where T : IEquatable<T>
    {
        return MemoryExtensions.SequenceEqual<T>(left, (ReadOnlySpan<T>)right);
    }

    public static bool EquatableEqual<T>(ReadOnlySpan<T> left, ReadOnlySpan<T> right)
        where T : IEquatable<T>
    {
        return MemoryExtensions.SequenceEqual<T>(left, right);
    }

#endregion

#region no constraints w/EqualityComparer

    public static bool Equal<T>(T[]? left, T[]? right, IEqualityComparer<T>? itemComparer = default)
    {
        if (left is null)
            return right is null;
        if (right is null)
            return false;
#if NET6_0_OR_GREATER
        return MemoryExtensions.SequenceEqual<T>(new ReadOnlySpan<T>(left), new ReadOnlySpan<T>(right), itemComparer);
#else
        return Equal<T>(new ReadOnlySpan<T>(left), new ReadOnlySpan<T>(right), itemComparer);
#endif
    }

    public static bool Equal<T>(T[]? left, Span<T> right, IEqualityComparer<T>? itemComparer = default)
    {
        if (left is null)
            return false;
#if NET6_0_OR_GREATER
        return MemoryExtensions.SequenceEqual<T>(new ReadOnlySpan<T>(left), (ReadOnlySpan<T>)right, itemComparer);
#else
        return Equal<T>(new ReadOnlySpan<T>(left), (ReadOnlySpan<T>)right, itemComparer);
#endif
    }


    public static bool Equal<T>(T[]? left, ReadOnlySpan<T> right, IEqualityComparer<T>? itemComparer = default)
    {
        if (left is null)
            return false;
#if NET6_0_OR_GREATER
        return MemoryExtensions.SequenceEqual<T>(new ReadOnlySpan<T>(left), right, itemComparer);
#else
        return Equal<T>(new ReadOnlySpan<T>(left), right, itemComparer);
#endif
    }

    public static bool Equal<T>(T[]? left, IEnumerable<T>? right, IEqualityComparer<T>? itemComparer = default)
    {
        if (left is null)
            return right is null;
        if (right is null)
            return false;
        return Equal<T>(new ReadOnlySpan<T>(left), right, itemComparer);
    }


    public static bool Equal<T>(Span<T> left, T[]? right, IEqualityComparer<T>? itemComparer = default)
    {
        if (right is null)
            return false;
#if NET6_0_OR_GREATER
        return MemoryExtensions.SequenceEqual<T>((ReadOnlySpan<T>)left, new ReadOnlySpan<T>(right), itemComparer);
#else
        return Equal<T>((ReadOnlySpan<T>)left, new ReadOnlySpan<T>(right), itemComparer);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equal<T>(Span<T> left, Span<T> right, IEqualityComparer<T>? itemComparer = default)
    {
#if NET6_0_OR_GREATER
        return MemoryExtensions.SequenceEqual<T>((ReadOnlySpan<T>)left, (ReadOnlySpan<T>)right, itemComparer);
#else
        return Equal<T>((ReadOnlySpan<T>)left, (ReadOnlySpan<T>)right, itemComparer);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equal<T>(Span<T> left, ReadOnlySpan<T> right, IEqualityComparer<T>? itemComparer = default)
    {
#if NET6_0_OR_GREATER
        return MemoryExtensions.SequenceEqual<T>((ReadOnlySpan<T>)left, right, itemComparer);
#else
        return Equal<T>((ReadOnlySpan<T>)left, right, itemComparer);
#endif
    }

    public static bool Equal<T>(Span<T> left, IEnumerable<T>? right, IEqualityComparer<T>? itemComparer = default)
    {
        if (right is null)
            return false;
        return Equal<T>((ReadOnlySpan<T>)left, right, itemComparer);
    }

    public static bool Equal<T>(ReadOnlySpan<T> left, T[]? right, IEqualityComparer<T>? itemComparer = default)
    {
        if (right is null)
            return false;
#if NET6_0_OR_GREATER
        return MemoryExtensions.SequenceEqual<T>(left, new ReadOnlySpan<T>(right), itemComparer);
#else
        return Equal<T>(left, new ReadOnlySpan<T>(right), itemComparer);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equal<T>(ReadOnlySpan<T> left, Span<T> right, IEqualityComparer<T>? itemComparer = default)
    {
#if NET6_0_OR_GREATER
        return MemoryExtensions.SequenceEqual<T>(left, (ReadOnlySpan<T>)right, itemComparer);
#else
        return Equal<T>(left, (ReadOnlySpan<T>)right, itemComparer);
#endif
    }

    public static bool Equal<T>(ReadOnlySpan<T> left, ReadOnlySpan<T> right, IEqualityComparer<T>? itemComparer = default)
    {
#if NET6_0_OR_GREATER
        return MemoryExtensions.SequenceEqual<T>(left, right, itemComparer);
#else
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
#endif
    }

    public static bool Equal<T>(ReadOnlySpan<T> left, IEnumerable<T>? right, IEqualityComparer<T>? itemComparer = null)
    {
        if (right is null)
            return false;
        itemComparer ??= EqualityComparer<T>.Default;
        int leftCount = left.Length;

        if (right is ICollection<T> rightCollection)
        {
            if (rightCollection.Count != leftCount)
                return false;

            if (right is IList<T> rightList)
            {
                for (var i = 0; i < leftCount; i++)
                {
                    if (!itemComparer.Equals(left[i], rightList[i]))
                        return false;
                }

                return true;
            }

            // Have to use enumerator
            using var rightEnumerator = rightCollection.GetEnumerator();
            for (var i = 0; i < leftCount; i++)
            {
#if DEBUG
                var moved = rightEnumerator.MoveNext();
                Debug.Assert(moved);
#else
                e.MoveNext();
#endif

                if (!itemComparer.Equals(left[i], rightEnumerator.Current))
                    return false;
            }

            return true;
        }
        else // have to enumerate
        {
            using var rightEnumerator = right.GetEnumerator();
            for (var i = 0; i < leftCount; i++)
            {
                if (!rightEnumerator.MoveNext())
                    return false; // left has more items
                if (!itemComparer.Equals(left[i], rightEnumerator.Current))
                    return false;
            }

            if (rightEnumerator.MoveNext())
                return false; // right has more items
            return true;
        }
    }

    public static bool Equal<T>(IEnumerable<T>? left, T[]? right, IEqualityComparer<T>? itemComparer = default)
    {
        if (left is null)
            return right is null;
        if (right is null)
            return false;
        return Equal<T>(left, new ReadOnlySpan<T>(right), itemComparer);
    }

    public static bool Equal<T>(IEnumerable<T>? left, Span<T> right, IEqualityComparer<T>? itemComparer = default)
    {
        if (left is null)
            return false;
        return Equal<T>(left, (ReadOnlySpan<T>)right, itemComparer);
    }

    public static bool Equal<T>(IEnumerable<T>? left, ReadOnlySpan<T> right, IEqualityComparer<T>? itemComparer = default)
    {
        if (left is null)
            return false;
        itemComparer ??= EqualityComparer<T>.Default;
        int rightCount = right.Length;

        if (left is ICollection<T> leftCollection)
        {
            if (leftCollection.Count != rightCount)
                return false;

            if (left is IList<T> leftList)
            {
                for (var i = 0; i < rightCount; i++)
                {
                    if (!itemComparer.Equals(leftList[i], right[i]))
                        return false;
                }

                return true;
            }

            // Have to use enumerator
            using var leftEnumerator = leftCollection.GetEnumerator();
            for (var i = 0; i < rightCount; i++)
            {
#if DEBUG
                var moved = leftEnumerator.MoveNext();
                Debug.Assert(moved);
#else
                e.MoveNext();
#endif

                if (!itemComparer.Equals(leftEnumerator.Current, right[i]))
                    return false;
            }

            return true;
        }
        else // have to enumerate
        {
            using var leftEnumerator = left.GetEnumerator();
            for (var i = 0; i < rightCount; i++)
            {
                if (!leftEnumerator.MoveNext())
                    return false; // right has more items
                if (!itemComparer.Equals(leftEnumerator.Current, right[i]))
                    return false;
            }

            if (leftEnumerator.MoveNext())
                return false; // left has more items
            return true;
        }
    }

    public static bool Equal<T>(IEnumerable<T>? left, IEnumerable<T>? right, IEqualityComparer<T>? itemComparer = null)
    {
        if (left is null)
            return right is null;
        if (right is null)
            return false;

        itemComparer ??= EqualityComparer<T>.Default;

        using var leftEnumerator = left.GetEnumerator();
        using var rightEnumerator = right.GetEnumerator();

        while (true)
        {
            var leftMoved = leftEnumerator.MoveNext();
            var rightMoved = rightEnumerator.MoveNext();
            if (leftMoved != rightMoved)
                return false; // different lengths

            if (!leftMoved)
            {
                Debug.Assert(!rightMoved);
                // They are equal
                return true;
            }

            if (!itemComparer.Equals(leftEnumerator.Current, rightEnumerator.Current))
                return false; // items differ
        }
    }

#endregion

#endregion
}