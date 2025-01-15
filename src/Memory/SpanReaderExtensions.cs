#pragma warning disable CA1045

namespace ScrubJay.Memory;

/// <summary>
/// Extensions on <see cref="SpanReader{T}"/>
/// </summary>
/// <remarks>
/// These methods are extensions because they work on a constrained <see cref="SpanReader{T}"/>
/// </remarks>
[PublicAPI]
public static class SpanReaderExtensions
{
#region IEquatable

    public static void SkipWhile<T>(this ref SpanReader<T> spanReader, ReadOnlySpan<T> match)
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

    public static ReadOnlySpan<T> TakeWhile<T>(this ref SpanReader<T> spanReader, ReadOnlySpan<T> match)
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

    public static void SkipUntil<T>(this ref SpanReader<T> spanReader, ReadOnlySpan<T> match)
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

    public static ReadOnlySpan<T> TakeUntil<T>(this ref SpanReader<T> spanReader, ReadOnlySpan<T> match)
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

    public static Option<T> TryPeekValue<T>(this ref SpanReader<byte> spanReader)
        where T : unmanaged
    {
        int size = Unsafe.SizeOf<T>();
        if (spanReader.TryPeek(size).HasSome(out var bytes))
        {
            T value;
#if NET8_0_OR_GREATER
            value = Unsafe.ReadUnaligned<T>(in bytes.GetPinnableReference());
#else
            value = Unsafe.ReadUnaligned<T>(ref Unsafe.AsRef<byte>(in bytes.GetPinnableReference()));
#endif
            return Some(value);
        }

        return None<T>();
    }

    public static Option<T> TryTakeValue<T>(this ref SpanReader<byte> spanReader)
        where T : unmanaged
    {
        int size = Unsafe.SizeOf<T>();
        if (spanReader.TryTake(size).HasSome(out var bytes))
        {
            T value;
#if NET8_0_OR_GREATER
            value = Unsafe.ReadUnaligned<T>(in bytes.GetPinnableReference());
#else
            value = Unsafe.ReadUnaligned<T>(ref Unsafe.AsRef<byte>(in bytes.GetPinnableReference()));
#endif
            return Some(value);
        }

        return None<T>();
    }

    public static T TakeValue<T>(this ref SpanReader<byte> byteEnumerator)
        where T : unmanaged
        => TryTakeValue<T>(ref byteEnumerator).SomeOrThrow($"There were not enough bytes to take a {typeof(T).NameOf()} value");

#endregion
}
