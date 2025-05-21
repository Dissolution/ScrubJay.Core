using System.Security.Cryptography;

namespace ScrubJay.Randomization.Seeding;

[PublicAPI]
public sealed class RandomPrngSeed : PrngSeed, IHasDefault<RandomPrngSeed>
{
    public static RandomPrngSeed Default { get; } = new();

    public override bool Known { get; } = false;

    private readonly RandomNumberGenerator _randomNumberGenerator = RandomNumberGenerator.Create();

    private RandomPrngSeed() { }


    public override void GetSeed(out ulong u64Seed)
    {
#if NETFRAMEWORK || NETSTANDARD2_0
        byte[] buffer = new byte[sizeof(ulong)];
        _randomNumberGenerator.GetBytes(buffer);
        u64Seed = BitConverter.ToUInt64(buffer, 0);
#else
        Span<byte> buffer = stackalloc byte[sizeof(ulong)];
        _randomNumberGenerator.GetBytes(buffer);
        u64Seed = BitHelper.FastRead<ulong>(buffer);
#endif
    }

    public override void GetSeeds(out ulong loSeed, out ulong hiSeed)
    {
#if NETFRAMEWORK || NETSTANDARD2_0
        byte[] buffer = new byte[sizeof(ulong) * 2];
        _randomNumberGenerator.GetBytes(buffer);
        loSeed = BitHelper.FastRead<ulong>(buffer.AsSpan(..sizeof(ulong)));
        hiSeed = BitHelper.FastRead<ulong>(buffer.AsSpan(sizeof(ulong)..));
#else
        Span<byte> buffer = stackalloc byte[sizeof(ulong) * 2];
        _randomNumberGenerator.GetBytes(buffer);
        loSeed = BitHelper.FastRead<ulong>(buffer[..sizeof(ulong)]);
        hiSeed = BitHelper.FastRead<ulong>(buffer[sizeof(ulong)..]);
#endif
    }

    public override void GetSeeds(out ulong loSeed, out ulong midSeed, out ulong hiSeed)
    {
#if NETFRAMEWORK || NETSTANDARD2_0
        byte[] buffer = new byte[sizeof(ulong) * 3];
        _randomNumberGenerator.GetBytes(buffer);
        loSeed = BitHelper.FastRead<ulong>(buffer.AsSpan(0, sizeof(ulong)));
        midSeed = BitHelper.FastRead<ulong>(buffer.AsSpan(sizeof(ulong), sizeof(ulong)));
        hiSeed = BitHelper.FastRead<ulong>(buffer.AsSpan(sizeof(ulong) * 2, sizeof(ulong)));
#else
        Span<byte> buffer = stackalloc byte[sizeof(ulong) * 3];
        _randomNumberGenerator.GetBytes(buffer);
        loSeed = BitHelper.FastRead<ulong>(buffer.Slice(0, sizeof(ulong)));
        midSeed = BitHelper.FastRead<ulong>(buffer.Slice(sizeof(ulong), sizeof(ulong)));
        hiSeed = BitHelper.FastRead<ulong>(buffer.Slice(sizeof(ulong) * 2, sizeof(ulong)));
#endif
    }

    public override void GetSeeds(out ulong loSeed, out ulong loMidSeed, out ulong hiMidSeed, out ulong hiSeed)
    {
#if NETFRAMEWORK || NETSTANDARD2_0
        byte[] buffer = new byte[sizeof(ulong) * 3];
        _randomNumberGenerator.GetBytes(buffer);
        loSeed = BitHelper.FastRead<ulong>(buffer.AsSpan(0, sizeof(ulong)));
        loMidSeed = BitHelper.FastRead<ulong>(buffer.AsSpan(sizeof(ulong), sizeof(ulong)));
        hiMidSeed = BitHelper.FastRead<ulong>(buffer.AsSpan(sizeof(ulong) * 2, sizeof(ulong)));
        hiSeed = BitHelper.FastRead<ulong>(buffer.AsSpan(sizeof(ulong) * 3, sizeof(ulong)));
#else
        Span<byte> buffer = stackalloc byte[sizeof(ulong) * 3];
        _randomNumberGenerator.GetBytes(buffer);
        loSeed = BitHelper.FastRead<ulong>(buffer.Slice(0, sizeof(ulong)));
        loMidSeed = BitHelper.FastRead<ulong>(buffer.Slice(sizeof(ulong), sizeof(ulong)));
        hiMidSeed = BitHelper.FastRead<ulong>(buffer.Slice(sizeof(ulong) * 2, sizeof(ulong)));
        hiSeed = BitHelper.FastRead<ulong>(buffer.Slice(sizeof(ulong) * 3, sizeof(ulong)));
#endif
    }

    public override void GetSeeds(Span<ulong> seeds)
    {
        int seedCount = seeds.Length;
        if (seedCount == 0) return;

#if NETFRAMEWORK || NETSTANDARD2_0
        byte[] buffer = new byte[sizeof(ulong) * seedCount];
        _randomNumberGenerator.GetBytes(buffer);
        Notsafe.Bytes.CopyTo(buffer, MemoryMarshal.Cast<ulong, byte>(seeds), buffer.Length);
#else
        Span<byte> buffer = stackalloc byte[sizeof(ulong) * seedCount];
        _randomNumberGenerator.GetBytes(buffer);
        Notsafe.Bytes.CopyTo(buffer, MemoryMarshal.Cast<ulong, byte>(seeds), buffer.Length);
#endif
    }
}