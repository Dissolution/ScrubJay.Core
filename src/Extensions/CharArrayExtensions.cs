namespace ScrubJay.Extensions;

[PublicAPI]
public static class CharArrayExtensions
{
    /// <summary>
    /// Gets a <c>ref char</c> pinnable reference to this <c>char[]</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref char GetPinnableReference(this char[] charArray)
    {
#if NET6_0_OR_GREATER
        return ref MemoryMarshal.GetArrayDataReference(charArray);
#else
        return ref charArray[0];
#endif
    }
}