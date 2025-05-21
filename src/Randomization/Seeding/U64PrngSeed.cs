namespace ScrubJay.Randomization.Seeding;

[PublicAPI]
public sealed class U64PrngSeed : PrngSeed
{
    private readonly ulong _seed;

    public override bool Known { get; } = true;

    public U64PrngSeed()
    {
        // get a random seed, but make it known
        RandomPrngSeed.Default.GetSeed(out _seed);
    }

    public U64PrngSeed(ulong seed)
    {
        _seed = seed;
    }

    public U64PrngSeed(ulong? seed)
    {
        if (seed.TryGetValue(out ulong known))
        {
            _seed = known;
        }
        else
        {
            // get a random seed, but make it known
            RandomPrngSeed.Default.GetSeed(out _seed);
        }
    }

    public override void GetSeed(out ulong u64Seed)
    {
        u64Seed = _seed;
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

    public override void GetSeeds(Span<ulong> seeds)
    {
        seeds.ForEach((ref ulong s) => s = _seed);
    }

    public override string ToString() => $"Seed: {_seed:D}";
}