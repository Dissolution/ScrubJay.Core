namespace ScrubJay.Extensions;

public static class StringExtensions
{
    #if !NET6_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryCopyTo(this string str, Span<char> destination)
        {
            return str.AsSpan().TryCopyTo(destination);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyTo(this string str, Span<char> destination)
        {
            str.AsSpan().CopyTo(destination);
        }
    #endif
}