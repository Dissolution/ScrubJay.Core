namespace ScrubJay.Randomization.Seeding;

[PublicAPI]
public class KnownPrngSeeds : PrngSeed
{
    protected readonly ulong[] _seeds;
    protected readonly int _seedCount;

    public override bool Known => true;

    public KnownPrngSeeds(params ulong[] seeds)
    {
        Throw.IfEmpty(seeds);
        _seeds = seeds;
        _seedCount = seeds.Length;
    }

    public KnownPrngSeeds(ReadOnlySpan<ulong> seeds)
    {
        Throw.IfEmpty(seeds);
        _seeds = seeds.ToArray();
        _seedCount = seeds.Length;
    }

    public override void GetSeed(out ulong u64Seed)
    {
        u64Seed = _seeds[0];
    }

    public override void GetSeeds(out ulong loSeed, out ulong hiSeed)
    {
        loSeed = _seeds[0];
        hiSeed = _seeds[1 % _seedCount];
    }

    public override void GetSeeds(out ulong loSeed, out ulong midSeed, out ulong hiSeed)
    {
        loSeed = _seeds[0];
        midSeed = _seeds[1 % _seedCount];
        hiSeed = _seeds[2 % _seedCount];
    }

    public override void GetSeeds(out ulong loSeed, out ulong loMidSeed, out ulong hiMidSeed, out ulong hiSeed)
    {
        loSeed = _seeds[0];
        loMidSeed = _seeds[1 % _seedCount];
        hiMidSeed = _seeds[2 % _seedCount];
        hiSeed = _seeds[3 % _seedCount];
    }

    public override void GetSeeds(Span<ulong> seeds)
    {
        for (var i = 0; i < seeds.Length; i++)
        {
            seeds[i] = _seeds[i % _seedCount];
        }
    }

    public override string ToString() => TextBuilder.New
        .Append("Known Seeds [")
        .EnumerateAndDelimit(_seeds,
           static (tb, seed) => tb.Append(seed, "X"),
           static tb => tb.Append(", "))
        .Append(']')
        .ToStringAndDispose();

}

