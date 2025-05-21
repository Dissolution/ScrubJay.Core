using System.Security.Cryptography;

namespace ScrubJay.Randomization;

[PublicAPI]
public sealed class RandomNumberGeneratorPrng : IPrng
{
    private readonly RandomNumberGenerator _provider;

    public RandomNumberGeneratorPrng()
    {
        _provider = RandomNumberGenerator.Create();
    }

    public RandomNumberGeneratorPrng(RandomNumberGenerator provider)
    {
        _provider = provider;
    }

    public ulong NextU64()
    {
#if NETFRAMEWORK || NETSTANDARD2_0
        byte[] buffer = new byte[sizeof(ulong)];
        _provider.GetBytes(buffer);
#else
        Span<byte> buffer = stackalloc byte[sizeof(ulong)];
        _provider.GetBytes(buffer);
#endif
        return BitHelper.Read<ulong>(buffer);
    }
}