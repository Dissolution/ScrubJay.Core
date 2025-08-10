namespace ScrubJay.Extensions;

[PublicAPI]
public static class ByteSpanExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAllZeros(this scoped ReadOnlySpan<byte> span)
    {
#if NET7_0_OR_GREATER
        return span.IndexOfAnyExcept((byte)0) == -1;
#else
        for (int i = 0; i < span.Length; i++)
        {
            if (span[i] != 0)
                return false;
        }

        return true;
#endif
    }
}