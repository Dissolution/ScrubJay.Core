using System.Text;

namespace ScrubJay.Randomization.Seeding;

[PublicAPI]
public sealed class KnownSeeds : RandSeed
{
    private readonly ulong[] _seeds;
    private readonly int _seedCount;

    public override bool IsStable => true;

    public ReadOnlySpan<ulong> SeedValues => new(_seeds);

    public KnownSeeds(params ulong[] seeds)
    {
        Throw.IfEmpty(seeds);
        _seeds = seeds;
        _seedCount = seeds.Length;
    }

    public KnownSeeds(ReadOnlySpan<ulong> seeds)
    {
        Throw.IfEmpty(seeds);
        _seeds = seeds.ToArray();
        _seedCount = seeds.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ulong GetSeed(int index) => _seeds[index % _seedCount];

    public override void FillSeeds(Span<ulong> buffer)
    {
        for (var i = buffer.Length - 1; i >= 0; i--)
        {
            buffer[i] = GetSeed(i);
        }
    }

    public override void GetSeed(out ulong seed)
    {
        seed = GetSeed(0);
    }

    public override void GetSeeds(out ulong loSeed, out ulong hiSeed)
    {
        loSeed = GetSeed(0);
        hiSeed = GetSeed(1);
    }

    public override void GetSeeds(out ulong loSeed, out ulong midSeed, out ulong hiSeed)
    {
        loSeed = GetSeed(0);
        midSeed = GetSeed(1);
        hiSeed = GetSeed(2);
    }

    public override void GetSeeds(out ulong loSeed, out ulong loMidSeed, out ulong hiMidSeed, out ulong hiSeed)
    {
        loSeed = GetSeed(0);
        loMidSeed = GetSeed(1);
        hiMidSeed = GetSeed(2);
        hiSeed = GetSeed(3);
    }

    public override string ToString() => TextBuilder.New
        .Append("Seeds:[")
        .EnumerateAndDelimit(_seeds,
            static (tb, seed) => tb.Append(seed, "X"),
            static tb => tb.Append(','))
        .Append(']')
        .ToStringAndDispose();
}