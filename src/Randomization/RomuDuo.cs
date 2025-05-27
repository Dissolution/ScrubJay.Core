using ScrubJay.Maths;
using ScrubJay.Randomization.Seeding;

namespace ScrubJay.Randomization;

/// <summary>
/// <b>RomuDuo</b>: might be faster than <see cref="RomuTrio"/> due to using fewer registers, but might struggle with massive jobs.<br/>
/// Est. capacity       = 2^61 bytes<br/>
/// Register pressure   = 5<br/>
/// State size          = 128 bits<br/>
/// </summary>
[PublicAPI]
public sealed class RomuDuo : RomuPrng
{
    private ulong _xState;
    private ulong _yState;

    public RomuDuo(RandSeed seed) : base(seed)
    {
        // We require two seeds
        this.Seed.GetSeeds(out _xState, out _yState);
        if (_xState == 0UL && _yState == 0UL)
            throw new InvalidOperationException("At least one seed value must be non-zero");
    }


    public override ulong NextU64()
    {
        /* uint64_t romuDuo_random () {
         *     uint64_t xp = xState;
         *     xState = 15241094284759029579u * yState;
         *     yState = ROTL(yState,36) + ROTL(yState,15) - xp;
         *     return xp;
         * }
         */

        ulong xp = _xState;
        _xState = 15241094284759029579UL * _yState;
        _yState = MathHelper.RotateLeft(_yState, 36) + MathHelper.RotateLeft(_yState, 15) - xp;
        return xp;
    }
}