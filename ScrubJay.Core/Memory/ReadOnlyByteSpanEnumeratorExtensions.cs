namespace ScrubJay.Memory;

/// <summary>
/// Extensions on <see cref="ReadOnlySpanEnumerator{T}">SpanReader&lt;byte&gt;</see>
/// </summary>
public static class ReadOnlyByteSpanEnumeratorExtensions
{
    public static Result<int> TryPeek<T>(
        this ref ReadOnlySpanEnumerator<byte> byteEnumerator,
        out T value)
        where T : unmanaged
    {
        int size = Scary.SizeOf<T>();
        if (byteEnumerator.TryPeek(size, out var bytes))
        {
            value = Scary.ReadUnaligned<T>(in bytes.GetPinnableReference());
            return size;
        }
        value = default;
        return new InvalidOperationException($"Cannot peek for a {typeof(T).Name}: Only {byteEnumerator.UnenumeratedCount} bytes remain");
    }
    
    public static Result<int> TryTake<T>(
        this ref ReadOnlySpanEnumerator<byte> byteEnumerator,
        out T value)
        where T : unmanaged
    {
        int size = Scary.SizeOf<T>();
        if (byteEnumerator.TryTake(size, out var bytes))
        {
            value = Scary.ReadUnaligned<T>(in bytes.GetPinnableReference());
            return size;
        }
        value = default;
        return new InvalidOperationException($"Cannot take a {typeof(T).Name}: Only {byteEnumerator.UnenumeratedCount} bytes remain");
    }

    public static T Take<T>(this ref ReadOnlySpanEnumerator<byte> byteEnumerator)
        where T : unmanaged
    {
        TryTake(ref byteEnumerator, out T value).ThrowIfError();
        return value;
    }
}