using System.Security.Cryptography;
using System.Text;

namespace ScrubJay.Randomization.Seeding;

[PublicAPI]
public sealed class PhraseSeed : RandSeed
{
    private readonly string _phrase;
    private readonly byte[] _hashBytes;

    public override bool IsStable => true;

    public string Phrase => _phrase;

    public PhraseSeed(string phrase)
    {
        Throw.IfEmpty(phrase);
        _phrase = phrase;
        var phraseBytes = Encoding.UTF8.GetBytes(phrase);
#if NETFRAMEWORK || NETSTANDARD || NETCOREAPP3_1
        using var sha = SHA512.Create();
        _hashBytes = sha.ComputeHash(phraseBytes);
#else
        _hashBytes = SHA512.HashData(phraseBytes);
#endif
    }

    public override void FillSeeds(Span<ulong> buffer)
    {
        var hashSeeds = MemoryMarshal.Cast<byte, ulong>(_hashBytes);

        while (!buffer.IsEmpty)
        {
            var count = Math.Min(hashSeeds.Length, buffer.Length);
            hashSeeds[..count].CopyTo(buffer);
            buffer = buffer[count..];
        }
    }

    public override void GetSeed(out ulong seed)
    {
        var seeds = MemoryMarshal.Cast<byte, ulong>(_hashBytes);
        seed = seeds[0];
    }

    public override void GetSeeds(out ulong loSeed, out ulong hiSeed)
    {
        var seeds = MemoryMarshal.Cast<byte, ulong>(_hashBytes);
        loSeed = seeds[0];
        hiSeed = seeds[1];
    }

    public override void GetSeeds(out ulong loSeed, out ulong midSeed, out ulong hiSeed)
    {
        var seeds = MemoryMarshal.Cast<byte, ulong>(_hashBytes);
        loSeed = seeds[0];
        midSeed = seeds[1];
        hiSeed = seeds[2];
    }

    public override void GetSeeds(out ulong loSeed, out ulong loMidSeed, out ulong hiMidSeed, out ulong hiSeed)
    {
        var seeds = MemoryMarshal.Cast<byte, ulong>(_hashBytes);
        loSeed = seeds[0];
        loMidSeed = seeds[1];
        hiMidSeed = seeds[2];
        hiSeed = seeds[3];
    }

    public override string ToString() => $"Seed:\"{Phrase}\"";
}