#pragma warning disable S3776, MA0051

using Polyfills;

namespace ScrubJay.Extensions;

/// <summary>
/// Extensions on <see cref="Span{T}"/>
/// </summary>
[PublicAPI]
public static class SpanExtensions
{
    extension<T>(scoped Span<T> span)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyFrom(scoped ReadOnlySpan<T> other) => other.CopyTo(span);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyFrom(scoped Span<T> other) => other.CopyTo(span);
    }


    extension<T>(ReadOnlySpan<T> span)
    {
#if !NET8_0_OR_GREATER
        public int IndexOf(T value, IEqualityComparer<T>? comparer = null)
        {
            if (comparer is null)
            {
                return span.IndexOf(value);
            }

            for (int i = 0; i < span.Length; i++)
            {
                if (comparer.Equals(span[i], value))
                {
                    return i;
                }
            }

            return -1;
        }
#endif

#if !NET10_0_OR_GREATER


        public int IndexOf(
            ReadOnlySpan<T> value,
            IEqualityComparer<T>? comparer = null)
        {
            if (value.Length == 0)
            {
                return 0;
            }

            if (comparer is null)
            {
                return span.IndexOf(value);
            }

            int total = 0;
            while (!span.IsEmpty)
            {
                int pos = span.IndexOf(value[0], comparer);
                if (pos < 0)
                {
                    break;
                }

                if (span.Slice(pos + 1).StartsWith(value.Slice(1), comparer))
                {
                    return total + pos;
                }

                total += pos + 1;
                span = span.Slice(pos + 1);
            }

            return -1;
        }

#if !NET8_0_OR_GREATER
        public int IndexOfAny(ReadOnlySpan<T> values, IEqualityComparer<T>? comparer = null)
        {
            switch (values.Length)
            {
                case 0:
                    return -1;

                case 1:
                    return IndexOf(span, values[0], comparer);

                case 2:
                    return IndexOfAny(span, values[0], values[1], comparer);

                case 3:
                    return IndexOfAny(span, values[0], values[1], values[2], comparer);

                default:
                    comparer ??= EqualityComparer<T>.Default;

                    for (int i = 0; i < span.Length; i++)
                    {
                        if (values.Contains(span[i], comparer))
                        {
                            return i;
                        }
                    }

                    return -1;
            }
        }

        public int IndexOfAny(T value0, T value1, IEqualityComparer<T>? comparer = null)
        {
            if (typeof(T).IsValueType || comparer is null || comparer == EqualityComparer<T>.Default)
            {
                for (int i = 0; i < span.Length; i++)
                {
                    if (EqualityComparer<T>.Default.Equals(span[i], value0) ||
                        EqualityComparer<T>.Default.Equals(span[i], value1))
                    {
                        return i;
                    }
                }

                return -1;
            }
            else
            {
                for (int i = 0; i < span.Length; i++)
                {
                    if (comparer.Equals(span[i], value0) || comparer.Equals(span[i], value1))
                    {
                        return i;
                    }
                }

                return -1;
            }
        }


        public int IndexOfAny(T value0, T value1, T value2, IEqualityComparer<T>? comparer = null)
        {
            if (typeof(T).IsValueType || comparer is null || comparer == EqualityComparer<T>.Default)
            {
                for (int i = 0; i < span.Length; i++)
                {
                    if (EqualityComparer<T>.Default.Equals(span[i], value0) ||
                        EqualityComparer<T>.Default.Equals(span[i], value1) ||
                        EqualityComparer<T>.Default.Equals(span[i], value2))
                    {
                        return i;
                    }
                }

                return -1;
            }
            else
            {
                for (int i = 0; i < span.Length; i++)
                {
                    if (comparer.Equals(span[i], value0) ||
                        comparer.Equals(span[i], value1) ||
                        comparer.Equals(span[i], value2))
                    {
                        return i;
                    }
                }

                return -1;
            }
        }
#endif
#endif
    }


    /// <summary>
    /// Performs the given <paramref name="perItem"/> action on each item in the <see cref="Span{T}"/>
    /// </summary>
    /// <param name="span">
    /// The <see cref="Span{T}"/> of items to perform the <see cref="FnRef{T,None}"/> delegate upon
    /// </param>
    /// <param name="perItem">
    /// The <see cref="FnRef{T,None}"/> delegate to perform on each item of the <see cref="Span{T}"/>
    /// </param>
    /// <typeparam name="T">
    /// The <see cref="Type"/> of items in the <see cref="Span{T}"/>
    /// </typeparam>
    public static void ForEach<T>(this Span<T> span, ActRef<T> perItem)
    {
        for (int i = 0; i < span.Length; i++)
        {
            perItem(ref span[i]);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> TryGet<T>(this Span<T> span, Index index)
    {
        int offset = index.GetOffset(span.Length);
        if ((offset < 0) || (offset >= span.Length))
            return None;
        return Some(span[offset]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> TryGet<T>(this ReadOnlySpan<T> span, Index index)
    {
        int offset = index.GetOffset(span.Length);
        if ((offset < 0) || (offset >= span.Length))
            return None;
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
        if ((offset < 0) || (offset >= span.Length))
            return false;
        span[offset] = value;
        return true;
    }

    public static bool StartsWith<T>(this Span<T> span, ReadOnlySpan<T> slice, IEqualityComparer<T>? itemComparer)
    {
        int sliceLen = slice.Length;
        if (sliceLen > span.Length)
            return false;
        return Sequence.Equal<T>(span[..sliceLen], slice, itemComparer);
    }

    public static bool StartsWith<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> slice, IEqualityComparer<T>? itemComparer)
    {
        int sliceLen = slice.Length;
        if (sliceLen > span.Length)
            return false;
        return Sequence.Equal<T>(span[..sliceLen], slice, itemComparer);
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
        for (int i = 0; i < spanLen; i++)
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
        for (int i = 0; i < spanLen; i++)
        {
            if (itemComparer.Equals(span[i], item))
                return true;
        }

        return false;
    }
    /*
       public static SpanSplitter<T> Splitter<T>(this ReadOnlySpan<T> span, T separator,
           SpanSplitOptions options = SpanSplitOptions.None)
           where T : IEquatable<T>
           => SpanSplitter<T>.Split(span, separator, options);

       public static SpanSplitter<T> Splitter<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> separator,
           SpanSplitOptions options = SpanSplitOptions.None)
           where T : IEquatable<T>
           => SpanSplitter<T>.Split(span, separator, options);

       public static SpanSplitter<T> SplitterAny<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> separators,
           SpanSplitOptions options = SpanSplitOptions.None)
           where T : IEquatable<T>
           => SpanSplitter<T>.SplitAny(span, separators, options);
           */
}