#pragma warning disable CA1034

using ScrubJay.Randomization.Seeding;

namespace ScrubJay.Randomization;

/// <summary>
///
/// </summary>
/// <see href="https://www.romu-random.org/"/><br/>
/// <see href="https://www.romu-random.org/code.c"/><br/>
[PublicAPI]
public abstract class RomuPrng : PrngBase
{
    protected RomuPrng(RandSeed seed) : base(seed) { }

    public override string ToString() => GetType().NameOf();
}