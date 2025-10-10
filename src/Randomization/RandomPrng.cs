#pragma warning disable CA5394

namespace ScrubJay.Randomization;

[PublicAPI]
public sealed class RandomPrng : IPrng
{
    private readonly Random _random;

    public RandomPrng(Random random)
    {
        _random = random;
    }

    public ulong NextU64()
    {
        Span<byte> buffer = stackalloc byte[sizeof(ulong)];
        _random.NextBytes(buffer);
        return BitHelper.AsValue<ulong>(buffer);
    }
}