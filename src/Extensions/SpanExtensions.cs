#pragma warning disable S3776, MA0051

using ScrubJay.Collections;
using ScrubJay.Memory;

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
        if (!Validate.Range(range, span.Length).HasOk(out var offsetLength))
        {
            slice = default;
            return false;
        }

        slice = span.Slice(offsetLength.Offset, offsetLength.Length);
        return true;
    }

    public static bool TryGet<T>(this ReadOnlySpan<T> span, Range range, out ReadOnlySpan<T> slice)
    {
        if (!Validate.Range(range, span.Length).HasOk(out var offsetLength))
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

    public static SpanSplitter<T> Split<T>(this ReadOnlySpan<T> span, T separator)
        where T : IEquatable<T>
    {
        return SpanSplitter<T>.Split(span, separator);
    }

    public static SpanSplitter<T> Split<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> separator)
        where T : IEquatable<T>
    {
        return SpanSplitter<T>.Split(span, separator);
    }

    public static SpanSplitter<T> SplitAny<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> separators)
        where T : IEquatable<T>
    {
        return SpanSplitter<T>.SplitAny(span, separators);
    }
}