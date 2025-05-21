namespace ScrubJay.Randomization.Seeding;

public abstract class PrngSeed
{
    /// <summary>
    /// Whether or not the exact seed value(s) are known
    /// </summary>
    public abstract bool Known { get; }


    public abstract void GetSeed(out ulong u64Seed);

    public abstract void GetSeeds(out ulong loSeed, out ulong hiSeed);

    public abstract void GetSeeds(out ulong loSeed, out ulong midSeed, out ulong hiSeed);

    public abstract void GetSeeds(out ulong loSeed, out ulong loMidSeed, out ulong hiMidSeed, out ulong hiSeed);

    public abstract void GetSeeds(Span<ulong> seeds);
}