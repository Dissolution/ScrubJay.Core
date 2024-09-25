namespace ScrubJay.Utilities;

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

#region Comparison
    
    public static int CompareTo<TSource>(IEnumerable<TSource>? left, IEnumerable<TSource>? right, IComparer<TSource>? itemComparer = null)
    {
        if (ReferenceEquals(left, right))
            return 0;
        if (left is null)
            return -1;
        if (right is null)
            return 1;

        int c;
        itemComparer ??= Comparer<TSource>.Default;

        if (left is ICollection<TSource> leftCollection && right is ICollection<TSource> rightCollection)
        {
            int count = leftCollection.Count;
            c = count.CompareTo(rightCollection.Count);
            if (c != 0)
                return c;

            if (leftCollection is IList<TSource> firstList && rightCollection is IList<TSource> secondList)
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
                using IEnumerator<TSource> leftEnumerator = left.GetEnumerator();
                using IEnumerator<TSource> rightEnumerator = right.GetEnumerator();

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
            using IEnumerator<TSource> leftEnumerator = left.GetEnumerator();
            using IEnumerator<TSource> rightEnumerator = right.GetEnumerator();

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
}