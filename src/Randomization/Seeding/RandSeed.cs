namespace ScrubJay.Randomization.Seeding;

/// <summary>
/// The seed for a <see cref="PrngBase"/>
/// </summary>
[PublicAPI]
public abstract class RandSeed
{
    public static UnknownSeed Unknown() => UnknownSeed.Default;

    public static KnownSeeds Known() => new KnownSeeds(UnknownSeed.Default.GetSeeds(4));
    public static KnownSeed Known(ulong value) => new KnownSeed(value);
    public static KnownSeed Known(long value) => new KnownSeed(value);
    public static KnownSeeds Known(params ulong[] seeds) => new KnownSeeds(seeds);
    public static KnownSeeds Known(ReadOnlySpan<ulong> seeds) => new KnownSeeds(seeds);
    public static PhraseSeed Known(string phrase) => new PhraseSeed(phrase);

    /// <summary>
    /// Whether this Seed is stable or deterministic
    /// </summary>
    public abstract bool IsStable { get; }

    public abstract void FillSeeds(Span<ulong> buffer);

    public virtual ReadOnlySpan<ulong> GetSeeds(int count)
    {
        ulong[] seeds = new ulong[count];
        FillSeeds(seeds);
        return seeds;
    }

    public virtual void GetSeed(out ulong seed)
    {
        Span<ulong> buffer = stackalloc ulong[1];
        FillSeeds(buffer);
        seed = buffer[0];
    }

    public virtual void GetSeeds(out ulong loSeed, out ulong hiSeed)
    {
        Span<ulong> buffer = stackalloc ulong[2];
        FillSeeds(buffer);
        loSeed = buffer[0];
        hiSeed = buffer[1];
    }

    public virtual void GetSeeds(out ulong loSeed, out ulong midSeed, out ulong hiSeed)
    {
        Span<ulong> buffer = stackalloc ulong[3];
        FillSeeds(buffer);
        loSeed = buffer[0];
        midSeed = buffer[1];
        hiSeed = buffer[2];
    }

    public virtual void GetSeeds(out ulong loSeed, out ulong loMidSeed, out ulong hiMidSeed, out ulong hiSeed)
    {
        Span<ulong> buffer = stackalloc ulong[4];
        FillSeeds(buffer);
        loSeed = buffer[0];
        loMidSeed = buffer[1];
        hiMidSeed = buffer[2];
        hiSeed = buffer[3];
    }
}