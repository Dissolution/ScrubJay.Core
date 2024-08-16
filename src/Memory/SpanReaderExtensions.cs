namespace ScrubJay.Memory;

/// <summary>
/// Extensions on <see cref="SpanReader{T}"/>
/// </summary>
/// <remarks>
/// These methods are not on <see cref="SpanReader{T}"/> itself so that we can constrain
/// the generic type further
/// </remarks>
public static class ReadOnlySpanEnumeratorExtensions
{
#region IEquatable

    public static void SkipWhile<T>(
        this ref SpanReader<T> spanReader,
        ReadOnlySpan<T> match)
        where T : IEquatable<T>
    {
        var remainingSpan = spanReader.RemainingSpan;
        int i = 0;
        int end = remainingSpan.Length - match.Length;
        while (i < end && remainingSpan.Slice(i).StartsWith(match))
        {
            i += match.Length;
        }

        spanReader.Position += i;
    }

    public static ReadOnlySpan<T> TakeWhile<T>(
        this ref SpanReader<T> spanReader,
        ReadOnlySpan<T> match)
        where T : IEquatable<T>
    {
        var remainingSpan = spanReader.RemainingSpan;
        int i = 0;
        int end = remainingSpan.Length - match.Length;
        while (i < end && remainingSpan.Slice(i).StartsWith(match))
        {
            i += match.Length;
        }

        spanReader.Position += i;
        return remainingSpan.Slice(0, i);
    }

    public static void SkipUntil<T>(
        this ref SpanReader<T> spanReader,
        ReadOnlySpan<T> match)
        where T : IEquatable<T>
    {
        var remainingSpan = spanReader.RemainingSpan;
        int i = 0;
        int end = remainingSpan.Length - match.Length;
        while (i < end && !remainingSpan.Slice(i).StartsWith(match))
        {
            i += match.Length;
        }

        spanReader.Position += i;
    }

    public static ReadOnlySpan<T> TakeUntil<T>(
        this ref SpanReader<T> spanReader,
        ReadOnlySpan<T> match)
        where T : IEquatable<T>
    {
        var remainingSpan = spanReader.RemainingSpan;
        int i = 0;
        int end = remainingSpan.Length - match.Length;
        while (i < end && !remainingSpan.Slice(i).StartsWith(match))
        {
            i += match.Length;
        }

        spanReader.Position += i;
        return remainingSpan.Slice(0, i);
    }

#endregion

#region byte

    public static Result<int, Exception> TryPeek<T>(
        this ref SpanReader<byte> spanReader,
        out T value)
        where T : unmanaged
    {
        int size = Unsafe.SizeOf<T>();
        if (spanReader.TryPeek(size, out var bytes))
        {
#if NET8_0_OR_GREATER
            value = Unsafe.ReadUnaligned<T>(in bytes.GetPinnableReference());
#else
            value = Unsafe.ReadUnaligned<T>(ref Unsafe.AsRef<byte>(in bytes.GetPinnableReference()));
#endif
            return size;
        }

        value = default;
        return new InvalidOperationException($"Cannot peek for a {typeof(T).Name}: Only {spanReader.RemainingCount} bytes remain");
    }

    public static Result<int, Exception> TryTake<T>(
        this ref SpanReader<byte> spanReader,
        out T value)
        where T : unmanaged
    {
        int size = Unsafe.SizeOf<T>();
        if (spanReader.TryTake(size, out var bytes))
        {
#if NET8_0_OR_GREATER
            value = Unsafe.ReadUnaligned<T>(in bytes.GetPinnableReference());
#else
            value = Unsafe.ReadUnaligned<T>(ref Unsafe.AsRef<byte>(in bytes.GetPinnableReference()));
#endif
            return size;
        }

        value = default;
        return new InvalidOperationException($"Cannot take a {typeof(T).Name}: Only {spanReader.RemainingCount} bytes remain");
    }

    public static T Take<T>(this ref SpanReader<byte> byteEnumerator)
        where T : unmanaged
    {
        TryTake(ref byteEnumerator, out T value).ThrowIfError();
        return value;
    }

#endregion
    
    #if NET7_0_OR_GREATER
    #region char + SpanParsable

    public static Result<T, Exception> TryParse<T>(
        this ref SpanReader<char> spanReader,
        IFormatProvider? formatProvider = null)
        where T : ISpanParsable<T>
    {
        if (T.TryParse(spanReader.RemainingSpan, formatProvider, out var value))
        {
            throw new NotImplementedException();
        }

        throw new NotImplementedException();
    }

#endregion
#endif
}