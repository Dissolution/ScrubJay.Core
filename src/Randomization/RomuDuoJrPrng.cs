using ScrubJay.Maths;
using ScrubJay.Randomization.Seeding;

namespace ScrubJay.Randomization;

/// <summary>
/// <b>RomuTrio</b>: the fastest generator using 64-bit arithmetic, but not suited for huge jobs.<br/>
/// Est. capacity       = 2^51 bytes<br/>
/// Register pressure   = 4<br/>
/// State size          = 128 bits<br/>
/// </summary>
[PublicAPI]
public sealed class RomuDuoJrPrng : RomuPrng
{
    private ulong _xState;
    private ulong _yState;

    public RomuDuoJrPrng(RandSeed seed) : base(seed)
    {
        // We require two seeds
        Seed.GetSeeds(out _xState, out _yState);
        if (_xState == 0UL && _yState == 0UL)
            throw new InvalidOperationException("At least one seed value must be non-zero");
    }

    public override ulong NextU64()
    {
        /* uint64_t romuDuoJr_random () {
         *     uint64_t xp = xState;
         *     xState = 15241094284759029579u * yState;
         *     yState = yState - xp;  yState = ROTL(yState,27);
         *     return xp;
         * }
         */

        ulong xp = _xState;
        _xState = 15241094284759029579UL * _yState;
        _yState = _yState - xp;
        _yState = MathHelper.RotateLeft(_yState, 27);
        return xp;
    }
}