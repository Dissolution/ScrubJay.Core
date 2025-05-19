using System.Security.Cryptography;
using System.Text;

namespace ScrubJay.Utilities;

public interface IRandomNumberGenerator
{
    ulong NextU64();
}

/// <summary>
///
/// </summary>
/// <see href="https://www.romu-random.org/"/>
public abstract class RomuPrng : IRandomNumberGenerator
{
    // #define ROTL(d,lrot) ((d<<(lrot)) | (d>>(8*sizeof(d)-(lrot))))
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static ulong RotateLeft(ulong value, int offset)
    {
#if NETFRAMEWORK || NETSTANDARD2_0
        return (value << offset) | (value >> (64 - offset));
#else
        return BitOperations.RotateLeft(value, offset);
#endif
    }

    protected static ReadOnlySpan<ulong> GetRandomSeeds(int count)
    {
        Debug.Assert(count > 0);
        byte[] buffer = new byte[count * sizeof(ulong)];
        using var rng = RandomNumberGenerator.Create();

        Span<ulong> seeds;
        int i;
        bool foundZero;

        LOOP:

        rng.GetBytes(buffer);
        seeds = MemoryMarshal.Cast<byte, ulong>(buffer);
        i = 0;
        foundZero = false;
        do
        {
            // no seed is ever allowed to be zero
            if (seeds[i] == 0UL)
            {
                if (foundZero)
                    goto LOOP;
                foundZero = true;
            }
            i++;
        } while (i < count);

        return seeds;
    }

    protected static ReadOnlySpan<ulong> GetSeeds(int count, string seed)
    {
        Debug.Assert(count > 0);

        int totalByteCount = count * sizeof(ulong);
        int seedByteCount = seed.Length * 4; // utf32

        if (seedByteCount < totalByteCount)
            throw new ArgumentException($"Seed phrase must be at least {(totalByteCount/4)} characters long", nameof(seed));

        byte[] buffer = new byte[totalByteCount];
        byte[] seedBytes = Encoding.UTF32.GetBytes(seed);
        Debug.Assert(seedByteCount == seedBytes.Length);
        Debug.Assert(buffer.Length <= seedBytes.Length);

        int i;

        i = 0;
        do
        {
            buffer[i % totalByteCount] = seedBytes[i];
            i++;
        } while (i < seedByteCount);

        var seeds = MemoryMarshal.Cast<byte, ulong>(buffer);

        i = 0;
        bool foundZero = false;
        do
        {
            // no seed is ever allowed to be zero
            if (seeds[i] == 0UL)
            {
                if (foundZero)
                    throw new ArgumentException("The provided seed cannot be used", nameof(seed));
                foundZero = true;
            }
            i++;
        } while (i < count);

        return seeds;
    }

    public abstract ulong NextU64();


    /// <summary>
    /// The fastest generator using 64-bit arithmetic, but not suited for huge jobs.<br/>
    /// Est. capacity = 2^51 bytes.<br/>
    /// Register pressure = 4.<br/>
    /// State size = 128 bits.<br/>
    /// </summary>
    public sealed class RomuDuoJrPrng : RomuPrng
    {
        /*
        uint64_t xState, yState;  // set to nonzero seed

        uint64_t romuDuoJr_random () {
            uint64_t xp = xState;
            xState = 15241094284759029579u * yState;
            yState = yState - xp;  yState = ROTL(yState,27);
            return xp;
        }
        */

        private ulong _xState;
        private ulong _yState;

        public RomuDuoJrPrng()
        {
            var seeds = GetRandomSeeds(2);
            _xState = seeds[0];
            _yState = seeds[1];
        }

        public RomuDuoJrPrng(ulong seed)
        {
            Throw.IfZero(seed);
            _xState = seed;
            _yState = seed;
        }

        public RomuDuoJrPrng(ulong lowerSeed, ulong upperSeed)
        {
            if (lowerSeed == 0UL && upperSeed == 0UL)
                throw new InvalidOperationException("At least one seed value must be non-zero");

            _xState = lowerSeed;
            _yState = upperSeed;
        }

        public RomuDuoJrPrng(string seed)
        {
            var seeds = GetSeeds(2, seed);
            _xState = seeds[0];
            _yState = seeds[1];
        }

        public override ulong NextU64()
        {
            ulong ystate = _yState;
            ulong xp = _xState;
            _xState = 0xD3833E804F4C574Bu * ystate;
            ystate -= xp;
            _yState = RotateLeft(ystate, 27);
            return xp;
        }
    }

    /// <summary>
    /// Might be faster than RomuTrio due to using fewer registers, but might struggle with massive jobs.<br/>
    /// Est. capacity = 2^61 bytes.<br/>
    /// Register pressure = 5.<br/>
    /// State size = 128 bits.<br/>
    /// </summary>
    public sealed class RomuDuo : RomuPrng
    {
        /*
        uint64_t xState, yState;  // set to nonzero seed

        uint64_t romuDuo_random () {
           uint64_t xp = xState;
           xState = 15241094284759029579u * yState;
           yState = ROTL(yState,36) + ROTL(yState,15) - xp;
           return xp;
        */

        private ulong _xState;
        private ulong _yState;

        public RomuDuo()
        {
            var seeds = GetRandomSeeds(2);
            _xState = seeds[0];
            _yState = seeds[1];
        }

        public RomuDuo(ulong seed)
        {
            Throw.IfZero(seed);
            _xState = seed;
            _yState = seed;
        }

        public RomuDuo(ulong lowerSeed, ulong upperSeed)
        {
            if (lowerSeed == 0UL && upperSeed == 0UL)
                throw new InvalidOperationException("At least one seed value must be non-zero");

            _xState = lowerSeed;
            _yState = upperSeed;
        }

        public RomuDuo(string seed)
        {
            var seeds = GetSeeds(2, seed);
            _xState = seeds[0];
            _yState = seeds[1];
        }

        public override ulong NextU64()
        {
            ulong ystate = _yState;
            ulong xp = _xState;
            _xState = 0xD3833E804F4C574Bu * ystate;
            _yState = RotateLeft(ystate, 36) + RotateLeft(ystate, 15) - xp;
            return xp;
        }
    }


    /// <summary>
    /// Great for general purpose work, including huge jobs.<br/>
    /// Est. capacity = 2^75 bytes.<br/>
    /// Register pressure = 6.<br/>
    /// State size = 192 bits.<br/>
    /// </summary>
    public sealed class RomuTrio : RomuPrng
    {
        /*
    uint64_t xState, yState, zState;  // set to nonzero seed

    uint64_t romuTrio_random () {
       uint64_t xp = xState, yp = yState, zp = zState;
       xState = 15241094284759029579u * zp;
       yState = yp - xp;  yState = ROTL(yState,12);
       zState = zp - yp;  zState = ROTL(zState,44);
       return xp;
        */

        private ulong _xState;
        private ulong _yState;
        private ulong _zState;

        public RomuTrio()
        {
            var seeds = GetRandomSeeds(3);
            _xState = seeds[0];
            _yState = seeds[1];
            _zState = seeds[2];
        }

        public RomuTrio(ulong seed)
        {
            Throw.IfZero(seed);
            _xState = seed;
            _yState = seed;
            _zState = seed;
        }

        public RomuTrio(ulong lowerSeed, ulong midSeed, ulong upperSeed)
        {
            if (lowerSeed == 0UL && midSeed == 0UL && upperSeed == 0UL)
                throw new InvalidOperationException("At least one seed value must be non-zero");

            _xState = lowerSeed;
            _yState = midSeed;
            _zState = upperSeed;
        }

        public RomuTrio(string seed)
        {
            var seeds = GetSeeds(3, seed);
            _xState = seeds[0];
            _yState = seeds[1];
            _zState = seeds[2];
        }

        public override ulong NextU64()
        {
            ulong xp = _xState;
            ulong yp = _yState;
            ulong zp = _zState;

            _xState = 15241094284759029579u * zp;
            _yState = RotateLeft(yp - xp, 12);
            _zState = RotateLeft(zp - yp, 44);

            return xp;
        }
    }

    /// <summary>
    /// More robust than anyone could need, but uses more registers than RomuTrio.<br/>
    /// Est. capacity = 2^90 bytes.<br/>
    /// Register pressure = 8 (high).<br/>
    /// State size = 256 bits.<br/>
    /// </summary>
    public sealed class RomuQuad : RomuPrng
    {
        /*
    uint64_t wState, xState, yState, zState;  // set to nonzero seed

    uint64_t romuQuad_random () {
       uint64_t wp = wState, xp = xState, yp = yState, zp = zState;
       wState = 15241094284759029579u * zp; // a-mult
       xState = zp + ROTL(wp,52);           // b-rotl, c-add
       yState = yp - xp;                    // d-sub
       zState = yp + wp;                    // e-add
       zState = ROTL(zState,19);            // f-rotl
       return xp;
        */

        private ulong _wState;
        private ulong _xState;
        private ulong _yState;
        private ulong _zState;

        public RomuQuad()
        {
            var seeds = GetRandomSeeds(4);
            _wState = seeds[0];
            _xState = seeds[1];
            _yState = seeds[2];
            _zState = seeds[3];
        }

        public RomuQuad(ulong seed)
        {
            Throw.IfZero(seed);
            _wState = seed;
            _xState = seed;
            _yState = seed;
            _zState = seed;
        }

        public RomuQuad(ulong loSeed, ulong loMidSeed, ulong hiMidSeed, ulong hiSeed)
        {
            if (loSeed == 0UL && loMidSeed == 0UL && hiMidSeed == 0UL && hiSeed == 0)
                throw new InvalidOperationException("At least one seed value must be non-zero");

            _wState = loSeed;
            _xState = ;;
            _yState = midSeed;
            _zState = upperSeed;
        }

        public RomuQuad(string seed)
        {
            var seeds = GetSeeds(3, seed);
            _xState = seeds[0];
            _yState = seeds[1];
            _zState = seeds[2];
        }

        public override ulong NextU64()
        {
            ulong xp = _xState;
            ulong yp = _yState;
            ulong zp = _zState;

            _xState = 15241094284759029579u * zp;
            _yState = RotateLeft(yp - xp, 12);
            _zState = RotateLeft(zp - yp, 44);

            return xp;
        }
    }

}