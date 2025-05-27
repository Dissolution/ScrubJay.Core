using ScrubJay.Maths;
using ScrubJay.Randomization.Seeding;

namespace ScrubJay.Randomization;

/// <summary>
///
/// </summary>
/// <remarks>
/// Heavily inspired from JSF
/// </remarks>
/// <seealso href="https://burtleburtle.net/bob/rand/smallprng.html"/>
[PublicAPI]
public sealed class SmallPrng : PrngBase
{
    private ulong _stateA;
    private ulong _stateB;
    private ulong _stateC;
    private ulong _stateD;

    /// <summary>
    /// Creates a new <see cref="SmallPrng"/> with the specified <seealso cref="RandSeed"/>
    /// </summary>
    /// <param name="randSeed"></param>
    public SmallPrng(RandSeed randSeed) : base(randSeed)
    {
        /* void raninit( ranctx *x, u8 seed ) {
         *     u8 i;
         *     x->a = 0xf1ea5eed, x->b = x->c = x->d = seed;
         *     for (i=0; i<20; ++i) {
         *         (void)ranval(x);
         *     }
         * }
         */

        randSeed.GetSeed(out var seed);

        _stateA = 0xF1EA5EED;
        _stateB = seed;
        _stateC = seed;
        _stateD = seed;

        for (var i = 0; i < 20; ++i)
        {
            InitRandom();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void InitRandom()
    {
        ulong e = _stateA - MathHelper.RotateLeft(_stateB, 7);
        _stateA = _stateB ^ MathHelper.RotateLeft(_stateC, 13);
        _stateB = _stateC + MathHelper.RotateLeft(_stateD, 37);
        _stateC = _stateD + e;
        _stateD = e + _stateA;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ulong NextU64()
    {
        /* u8 ranval( ranctx *x ) {
         *     u8 e = x->a - rot(x->b, 7);
         *     x->a = x->b ^ rot(x->c, 13);
         *     x->b = x->c + rot(x->d, 37);
         *     x->c = x->d + e;
         *     x->d = e + x->a;
         *     return x->d;
         * }
         */

        ulong e = _stateA - MathHelper.RotateLeft(_stateB, 7);
        _stateA = _stateB ^ MathHelper.RotateLeft(_stateC, 13);
        _stateB = _stateC + MathHelper.RotateLeft(_stateD, 37);
        _stateC = _stateD + e;
        _stateD = e + _stateA;
        return _stateD;
    }
}