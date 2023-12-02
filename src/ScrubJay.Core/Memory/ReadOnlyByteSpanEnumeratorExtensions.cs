namespace ScrubJay.Memory;

/// <summary>
/// Extensions on <see cref="ReadOnlySpanEnumerator{T}">SpanReader&lt;byte&gt;</see>
/// </summary>
public static class ReadOnlyByteSpanEnumeratorExtensions
{
    public static Result TryPeek<T>(
        this ref ReadOnlySpanEnumerator<byte> byteEnumerator,
        out T value)
        where T : unmanaged
    {
        if (byteEnumerator.TryPeek(Scary.SizeOf<T>(), out var bytes))
        {
            value = Scary.ReadUnaligned<T>(in bytes.GetPinnableReference());
            return true;
        }
        value = default;
        return new InvalidOperationException($"Cannot peek for a {typeof(T).Name}: Only {byteEnumerator.UnenumeratedCount} bytes remain");
    }
    
    public static Result TryTake<T>(
        this ref ReadOnlySpanEnumerator<byte> byteEnumerator,
        out T value)
        where T : unmanaged
    {
        if (byteEnumerator.TryTake(Scary.SizeOf<T>(), out var bytes))
        {
            value = Scary.ReadUnaligned<T>(in bytes.GetPinnableReference());
            return true;
        }
        value = default;
        return new InvalidOperationException($"Cannot take a {typeof(T).Name}: Only {byteEnumerator.UnenumeratedCount} bytes remain");
    }

    public static T Take<T>(this ref ReadOnlySpanEnumerator<byte> byteEnumerator)
        where T : unmanaged
        => TryTake<T>(ref byteEnumerator, out T value)
            .WithValue(value)
            .OkValueOrThrowError();
}