/*using System.Security.Cryptography;

namespace ScrubJay.Utilities;

/// <summary>
///
/// </summary>
/// <remarks>
/// Heavily inspired from JSF
/// </remarks>
/// <seealso href="https://burtleburtle.net/bob/rand/smallprng.html"/>
[PublicAPI]
public sealed class SmallPrng
{
    /* 64-bit C code:
     *
     * typedef unsigned long long u8;
     * typedef struct ranctx { u8 a; u8 b; u8 c; u8 d; } ranctx;
     *
     * #define rot(x,k) (((x)<<(k))|((x)>>(64-(k))))
     * u8 ranval( ranctx *x ) {
     *     u8 e = x->a - rot(x->b, 7);
     *     x->a = x->b ^ rot(x->c, 13);
     *     x->b = x->c + rot(x->d, 37);
     *     x->c = x->d + e;
     *     x->d = e + x->a;
     *     return x->d;
     * }
     *
     * void raninit( ranctx *x, u8 seed ) {
     *     u8 i;
     *     x->a = 0xf1ea5eed, x->b = x->c = x->d = seed;
     *     for (i=0; i<20; ++i) {
     *         (void)ranval(x);
     *     }
     * }

     #1#

    public static ulong CreateSeed()
    {
#if NETFRAMEWORK || NETSTANDARD2_0
        using var rng = RandomNumberGenerator.Create();
        byte[] bytes = new byte[sizeof(ulong)];
        rng.GetBytes(bytes);
        return BitConverter.ToUInt64(bytes, 0);
#else
        Span<byte> bytes = stackalloc byte[sizeof(ulong)];
        RandomNumberGenerator.Fill(bytes);
        return Unsafe.ReadUnaligned<ulong>(ref MemoryMarshal.GetReference(bytes));
#endif
    }


    private ulong _stateA;
    private ulong _stateB;
    private ulong _stateC;
    private ulong _stateD;

    public ulong Seed { get; }

    /// <summary>
    /// Creates a new <see cref="SmallPrng"/> with a random seed
    /// </summary>
    public SmallPrng() : this(CreateSeed()) { }

    /// <summary>
    /// Creates a new <see cref="SmallPrng"/> with the specified <paramref name="seed"/>
    /// </summary>
    /// <param name="seed"></param>
    public SmallPrng(ulong seed)
    {
        _stateA = 0xF1EA5EED;
        _stateB = seed;
        _stateC = seed;
        _stateD = seed;

        for (var i = 0; i < 20; ++i)
        {
            NextRandom();
        }

        this.Seed = seed;
    }

    public SmallPrng(ulong? seed)
        : this(seed.TryGetValue(out var s) ? s : GetSeed())
    {

    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void InitRandom()
    {
        ulong e = _stateA - BitOperations.RotateLeft(_stateB, 7);
        _stateA = _stateB ^ BitOperations.RotateLeft(_stateC, 13);
        _stateB = _stateC + BitOperations.RotateLeft(_stateD, 37);
        _stateC = _stateD + e;
        _stateD = e + _stateA;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ulong NextRandom()
    {
        ulong e = _stateA - BitOperations.RotateLeft(_stateB, 7);
        _stateA = _stateB ^ BitOperations.RotateLeft(_stateC, 13);
        _stateB = _stateC + BitOperations.RotateLeft(_stateD, 37);
        _stateC = _stateD + e;
        _stateD = e + _stateA;
        return _stateD;
    }

    public void Fill(Span<byte> buffer)
    {
        int bytesWritten = 0;
        int length = buffer.Length;

        ref byte destRef = ref buffer.GetPinnableReference();
        ulong r;

        // Process 8 bytes at a time for efficiency
        while ((bytesWritten + 8) <= length)
        {
            r = NextRandom();
            Unsafe.WriteUnaligned(ref destRef, r);
            Unsafe.Add(ref destRef, 8);
            bytesWritten += 8;
        }

        // Handle remaining bytes (less than 8)
        if (bytesWritten < length)
        {
            r = NextRandom();

            Span<byte> bytes = stackalloc byte[sizeof(ulong)];
            Unsafe.As<byte, ulong>(ref bytes.GetPinnableReference()) = r;

            for (int i = 0; i < length - bytesWritten; i++)
            {
                buffer[bytesWritten + i] = bytes[i];
            }
        }
    }

    public N Next<N>()
        where N : unmanaged
    {
        Span<byte> buffer = stackalloc byte[Notsafe.SizeOf<N>()];
        Fill(buffer);
        return Unsafe.ReadUnaligned<N>(ref buffer.GetPinnableReference());
    }

    public byte NextU8()
    {
        return (byte)(NextRandom() >> (sizeof(ulong) - sizeof(byte)));
    }

    public sbyte NextI8()
    {
        return (sbyte)(NextRandom() >> (sizeof(ulong) - sizeof(sbyte)));
    }

    public short NextI16()
    {
        return (short)(NextRandom() >> (sizeof(ulong) - sizeof(short)));
    }

    public ushort NextU16()
    {
        return (ushort)(NextRandom() >> (sizeof(ulong) - sizeof(ushort)));
    }

    public int NextI32()
    {
        return (int)(NextRandom() >> (sizeof(ulong) - sizeof(int)));
    }

    public uint NextU32()
    {
        return (uint)(NextRandom() >> (sizeof(ulong) - sizeof(uint)));
    }

    public long NextI64()
    {
        return (long)(NextRandom());
    }

    public ulong NextU64()
    {
        return NextRandom();
    }

    public float NextF32Percent()
    {
        return (float)NextRandom() / (float)ulong.MaxValue;
    }

    public double NextF64Percent()
    {
        return (double)NextRandom() / (double)ulong.MaxValue;
    }

    public int Between(int inclusiveMin, int exclusiveMax)
    {
        Debug.Assert(inclusiveMin < exclusiveMax);

        ulong range = ((ulong)exclusiveMax - (ulong)inclusiveMin);// + 1UL;
        return inclusiveMin + (int)(NextRandom() % range);
    }

    public void Shuffle<T>(T[] array)
    {
        int n = array.Length;
        if (n < 2)
            return;

        T temp;
        for (int i = n - 2; i >= 0; i--)
        {
            int r = Between(i, n);
            if (r != i)
            {
                temp = array[i];
                array[i] = array[r];
                array[r] = temp;
            }
        }
    }

    public void Shuffle<T>(scoped Span<T> span)
    {
        int n = span.Length;
        if (n < 2)
            return;

        T temp;
        for (int i = n - 2; i >= 0; i--)
        {
            int r = Between(i, n);
            if (r != i)
            {
                temp = span[i];
                span[i] = span[r];
                span[r] = temp;
            }
        }
    }

    public void Shuffle<T>(IList<T> list)
    {
        int n = list.Count;
        if (n < 2)
            return;

        T temp;
        for (int i = n - 2; i >= 0; i--)
        {
            int r = Between(i, n);
            if (r != i)
            {
                temp = list[i];
                list[i] = list[r];
                list[r] = temp;
            }
        }
    }
}*/