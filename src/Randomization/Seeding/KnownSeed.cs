namespace ScrubJay.Randomization.Seeding;

[PublicAPI]
public sealed class KnownSeed : RandSeed
{
    private readonly ulong _seed;
    public override bool IsStable => true;

    public ulong SeedValue => _seed;

    public KnownSeed(ulong seed)
    {
        _seed = seed;
    }

    public KnownSeed(long seed)
    {
        _seed = (ulong)seed;
    }

    public override void FillSeeds(Span<ulong> buffer)
    {
        ulong seed = _seed;
        for (int i = buffer.Length - 1; i >= 0; i--)
        {
            buffer[i] = seed;
        }
    }

    public override void GetSeed(out ulong seed)
    {
        seed = _seed;
    }

    public override void GetSeeds(out ulong loSeed, out ulong hiSeed)
    {
        loSeed = _seed;
        hiSeed = _seed;
    }

    public override void GetSeeds(out ulong loSeed, out ulong midSeed, out ulong hiSeed)
    {
        loSeed = _seed;
        midSeed = _seed;
        hiSeed = _seed;
    }

    public override void GetSeeds(out ulong loSeed, out ulong loMidSeed, out ulong hiMidSeed, out ulong hiSeed)
    {
        loSeed = _seed;
        loMidSeed = _seed;
        hiMidSeed = _seed;
        hiSeed = _seed;
    }

    public override string ToString() => $"Seed:{_seed:X}";
}