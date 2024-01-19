using ScrubJay.Memory;

namespace ScrubJay.Extensions;

public static class SpanExtensions
{
    #region Span-Only
    public delegate void RefItem<T>(ref T item);

    public static void ForEach<T>(this Span<T> span, RefItem<T> perItem)
    {
        for (var i = 0; i < span.Length; i++)
        {
            perItem(ref span[i]);
        }
    }
    #endregion
    
   
    
#if !NET6_0_OR_GREATER
    public static bool SequenceEqual<T>(this Span<T> first, ReadOnlySpan<T> second)
    {
        int firstLen = first.Length;
        if (second.Length != firstLen) return false;
        for (var i = 0; i < firstLen; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(first[i], second[i])) return false;
        }
        return true;
    }

    public static bool SequenceEqual<T>(this Span<T> first, ReadOnlySpan<T> second, IEqualityComparer<T>? itemComparer)
    {
        int firstLen = first.Length;
        if (second.Length != firstLen) return false;
        itemComparer ??= EqualityComparer<T>.Default;
        for (var i = 0; i < firstLen; i++)
        {
            if (!itemComparer.Equals(first[i], second[i])) return false;
        }
        return true;
    }
    

    public static bool SequenceEqual<T>(this ReadOnlySpan<T> first, ReadOnlySpan<T> second)
    {
        int firstLen = first.Length;
        if (second.Length != firstLen) return false;
        for (var i = 0; i < firstLen; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(first[i], second[i])) return false;
        }
        return true;
    }

    public static bool SequenceEqual<T>(this ReadOnlySpan<T> first, ReadOnlySpan<T> second, IEqualityComparer<T>? itemComparer)
    {
        int firstLen = first.Length;
        if (second.Length != firstLen) return false;
        itemComparer ??= EqualityComparer<T>.Default;
        for (var i = 0; i < firstLen; i++)
        {
            if (!itemComparer.Equals(first[i], second[i])) return false;
        }
        return true;
    }
#endif
    
//    public static SpanEnumerator<T> Enumerate<T>(this Span<T> span) => new SpanEnumerator<T>(span);
//    
//    public static ReadOnlySpanEnumerator<T> Enumerate<T>(this ReadOnlySpan<T> readOnlySpan) => new ReadOnlySpanEnumerator<T>(readOnlySpan);

    
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