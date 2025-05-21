namespace ScrubJay.Randomization;

/// <summary>
/// Pseudo-Random Number Generator
/// </summary>
[PublicAPI]
public interface IPrng
{
    /// <summary>
    /// Generate the next <see cref="ulong"/> value
    /// </summary>
    ulong NextU64();
}