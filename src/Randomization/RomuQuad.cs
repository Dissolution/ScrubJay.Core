using ScrubJay.Maths;
using ScrubJay.Randomization.Seeding;

namespace ScrubJay.Randomization;

/// <summary>
/// <b>RomuQuad</b>: more robust than anyone could need, also uses more registers than <see cref="RomuTrio"/>.<br/>
/// Est. capacity       = 2^90 bytes<br/>
/// Register pressure   = 8 (high)<br/>
/// State size          = 256 bits<br/>
/// </summary>
public sealed class RomuQuad : RomuPrng, ICloneable<RomuQuad>
{
    private ulong _wState;
    private ulong _xState;
    private ulong _yState;
    private ulong _zState;

    public RomuQuad(ulong wState, ulong xState, ulong yState, ulong zState, RandSeed seed) : base(seed)
    {
        _wState = wState;
        _xState = xState;
        _yState = yState;
        _zState = zState;
    }


    public RomuQuad(RandSeed seed) : base(seed)
    {
        // We require four seeds
        Seed.GetSeeds(out _wState, out _xState, out _yState, out _zState);
        if (_wState == 0UL && _xState == 0UL && _yState == 0UL && _zState == 0UL)
            throw Ex.Invalid("At least one seed value must be non-zero");
    }

    public override ulong NextU64()
    {
        /* uint64_t romuQuad_random () {
         *     uint64_t wp = wState, xp = xState, yp = yState, zp = zState;
         *     wState = 15241094284759029579u * zp; // a-mult
         *     xState = zp + ROTL(wp,52);           // b-rotl, c-add
         *     yState = yp - xp;                    // d-sub
         *     zState = yp + wp;                    // e-add
         *     zState = ROTL(zState,19);            // f-rotl
         *     return xp;
         * }
         */

        ulong wp = _wState, xp = _xState, yp = _yState, zp = _zState;
        _wState = 15241094284759029579UL * zp; // a-mult
        _xState = zp + MathHelper.RotateLeft(wp, 52); // b-rotl, c-add
        _yState = yp - xp; // d-sub
        _zState = yp + wp; // e-add
        _zState = MathHelper.RotateLeft(_zState, 19); // f-rotl
        return xp;
    }

    public RomuQuad Clone()
    {
        return new RomuQuad(_wState, _xState, _yState, _zState, Seed);
    }
}