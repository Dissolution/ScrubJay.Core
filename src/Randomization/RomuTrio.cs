using ScrubJay.Maths;
using ScrubJay.Randomization.Seeding;

namespace ScrubJay.Randomization;

/// <summary>
/// <b>RomuTrio</b>: great for general purpose work, including huge jobs.<br/>
/// Est. capacity       = 2^75 bytes<br/>
/// Register pressure   = 6<br/>
/// State size          = 192 bits<br/>
/// </summary>
public sealed class RomuTrio : RomuPrng
{
    private ulong _xState;
    private ulong _yState;
    private ulong _zState;

    public RomuTrio(RandSeed seed) : base(seed)
    {
        // We require three seeds
        Seed.GetSeeds(out _xState, out _yState, out _zState);
        if (_xState == 0UL && _yState == 0UL && _zState == 0UL)
            throw new InvalidOperationException("At least one seed value must be non-zero");
    }

    public override ulong NextU64()
    {
        /* uint64_t romuTrio_random () {
         *     uint64_t xp = xState, yp = yState, zp = zState;
         *     xState = 15241094284759029579u * zp;
         *     yState = yp - xp;  yState = ROTL(yState,12);
         *     zState = zp - yp;  zState = ROTL(zState,44);
         *     return xp;
         * }
         */

        ulong xp = _xState, yp = _yState, zp = _zState;
        _xState = 15241094284759029579UL * zp;
        _yState = yp - xp; _yState = MathHelper.RotateLeft(_yState, 12);
        _zState = zp - yp; _zState = MathHelper.RotateLeft(_zState, 44);
        return xp;
    }
}