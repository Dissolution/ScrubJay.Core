using System.Security.Cryptography;

namespace ScrubJay.Randomization.Seeding;


[PublicAPI]
public sealed class UnknownSeed : RandSeed, IHasDefault<UnknownSeed>
{
    public static UnknownSeed Default { get; } = new();

    public override bool IsStable => false;

    public override void FillSeeds(Span<ulong> buffer)
    {
#if NETFRAMEWORK || NETSTANDARD2_0
        using var rng = RandomNumberGenerator.Create();
        int bufferCount = buffer.Length;
        byte[] byteBuffer = new byte[bufferCount * 8];
        rng.GetBytes(byteBuffer);
        Notsafe.Bytes.CopyTo(byteBuffer, MemoryMarshal.Cast<ulong, byte>(buffer), bufferCount * 8);
#else
        int bufferCount = buffer.Length;
        Span<byte> byteBuffer = stackalloc byte[bufferCount * 8];
        RandomNumberGenerator.Fill(byteBuffer);
        Notsafe.Bytes.CopyTo(byteBuffer, MemoryMarshal.Cast<ulong, byte>(buffer), bufferCount * 8);
#endif
    }
}
