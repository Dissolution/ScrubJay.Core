namespace ScrubJay.Extensions;

/// <summary>
/// Extensions on <see cref="char"/>
/// </summary>
public static class CharExtensions
{
    /// <summary>
    /// Converts this <see cref="char"/> into a <see cref="ReadOnlySpan{T}">ReadOnlySpan&lt;char&gt;</see> containing it
    /// </summary>
    /// <param name="ch">
    /// The input character
    /// </param>
    /// <returns>
    /// A <see cref="ReadOnlySpan{T}">ReadOnlySpan&lt;char&gt;</see> containing the <see cref="char"/>
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<char> AsSpan(this in char ch)
    {
#if NET7_0_OR_GREATER
        return new ReadOnlySpan<char>(in ch);
#else
        unsafe
        {
            fixed (void* vp = &ch)
            {
                return new ReadOnlySpan<char>(vp, 1);
            }
        }
#endif
    }
}