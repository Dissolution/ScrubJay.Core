using ScrubJay.Maths;
using ScrubJay.Randomization.Seeding;
using ScrubJay.Text.Rendering;

// ReSharper disable InconsistentNaming

namespace ScrubJay.Randomization;

[PublicAPI]
public abstract class PrngBase : IPrng
{
    public RandSeed Seed { get; }

    protected PrngBase(RandSeed seed)
    {
        Seed = seed;
    }

    public virtual bool NextBoolean() => (NextU64() >> 63) == 1UL;

    public virtual sbyte NextI8() => (sbyte)(NextU64() >> (sizeof(ulong) - sizeof(sbyte)));

    public virtual byte NextU8() => (byte)(NextU64() >> (sizeof(ulong) - sizeof(byte)));

    public virtual short NextI16() => (short)(NextU64() >> (sizeof(ulong) - sizeof(short)));

    public virtual ushort NextU16() => (ushort)(NextU64() >> (sizeof(ulong) - sizeof(ushort)));

    public virtual int NextI32() => (int)(NextU64() >> (sizeof(ulong) - sizeof(int)));

    public virtual uint NextU32() => (uint)(NextU64() >> (sizeof(ulong) - sizeof(uint)));

    public virtual (int Lo, int Hi) NextI32s()
    {
        ulong u64 = NextU64();
        const ulong lo_mask = 0x00000000FFFFFFFFUL;
        const ulong hi_mask = 0xFFFFFFFF00000000UL;

        var lo = ((int)(u64 & lo_mask));
        var hi = ((int)((u64 & hi_mask) >> 32));
        return (lo, hi);
    }

    public virtual (uint Lo, uint Hi) NextU32s()
    {
        ulong u64 = NextU64();
        const ulong lo_mask = 0x00000000FFFFFFFFUL;
        const ulong hi_mask = 0xFFFFFFFF00000000UL;

        var lo = ((uint)(u64 & lo_mask));
        var hi = ((uint)((u64 & hi_mask) >> 32));
        return (lo, hi);
    }

    public abstract ulong NextU64();

    public virtual long NextI64() => (long)NextU64();

    public virtual decimal NextDecimal(bool nonUniform = true)
    {
        if (nonUniform)
        {
            // https://stackoverflow.com/a/609529
            return new decimal(
                NextI32(),
                NextI32(),
                NextI32(),
                NextBoolean(),
                (byte)ZeroTo(29));
        }
        else
        {
            // https://stackoverflow.com/a/610228
            return new decimal(
                NextI32(),
                NextI32(),
                NextI32(),
                NextBoolean(),
                GetDecimalScale());
        }


        byte GetDecimalScale()
        {
            for (int i = 0; i <= 28; i++)
            {
                if (PercentF64() >= 0.1d)
                    return (byte)i;
            }
            return 0;
        }
    }

    public decimal PercentDec()
    {
        // https://stackoverflow.com/a/610228
        // https://stackoverflow.com/a/28860710

        return new decimal(
            lo: NextI32(),
            mid: NextI32(),
            hi: ZeroTo(0x204FCE5F),
            isNegative: false,
            scale: 28);
    }

    // [0.0 .. 1.0)
    public float PercentF32()
    {
        return (NextU64() >> 40) * (1.0f / (1U << 24));
    }

    // [0.0 .. 1.0)
    public double PercentF64()
    {
        return (NextU64() >> 11) * (1.0d / (1UL << 53));
    }

    public virtual void Fill(Span<byte> span)
    {
        while (span.Length >= sizeof(ulong))
        {
            BitHelper.WriteTo(span, NextU64());
            span = span[sizeof(ulong)..];
        }

        if (!span.IsEmpty)
        {
            ulong next = NextU64();
            var remaining = BitHelper.AsBytes(in next);
            for (var i = 0; i < span.Length; i++)
            {
                span[i] = remaining[i];
            }
        }
    }

    public void Fill<U>(Span<U> span)
        where U : unmanaged
    {
        Fill(MemoryMarshal.Cast<U, byte>(span));
    }

    public U Create<U>()
        where U : unmanaged
    {
        unsafe
        {
            Span<byte> buffer = stackalloc byte[sizeof(U)];
            Fill(buffer);
            return BitHelper.Read<U>(buffer);
        }
    }

    public T GetItem<T>(params ReadOnlySpan<T> items)
    {
        int count = items.Length;
        if (count == 0)
            throw Ex.Arg<Unit>(default, "You must pass at least one item", nameof(items));
        if (count == 1)
            return items[0];
        int r = ZeroTo(count);
        return items[r];
    }

    public T[] GetItems<T>(int count, params ReadOnlySpan<T> items)
    {
        Throw.IfLessThan(count, 0);
        if (count == 0) return [];

        int choiceCount = items.Length;

        T[] dest = new T[count];

        // elide bounds checks
        for (int i = dest.Length - 1; i >= 0; i--)
        {
            dest[i] = items[ZeroTo(choiceCount)];
        }

        return dest;
    }

    public void FillItems<T>(ReadOnlySpan<T> choices, Span<T> destination)
    {
        Throw.IfEmpty(choices);

        int choiceCount = choices.Length;

        // elide bounds checks
        for (int i = destination.Length - 1; i >= 0; i--)
        {
            destination[i] = choices[ZeroTo(choiceCount)];
        }
    }

    public string GetAsciiString(int length)
    {
        Throw.IfLessThan(length, 0);
        if (length == 0)
            return string.Empty;
        Span<char> buffer = stackalloc char[length];
        // valid ascii characters are in range [32..127)
        for (var i = 0; i < length; i++)
        {
            buffer[i] = (char)InRange(32, 127);
        }
        return buffer.AsString();
    }




    /// <summary>
    ///
    /// </summary>
    /// <param name="maxValue"><b>exclusive</b> upper bound</param>
    /// <returns></returns>
    public uint ZeroTo(uint maxValue)
    {
        ulong randomProduct = (ulong)maxValue * NextU32();
        uint lowPart = (uint)randomProduct;

        if (lowPart < maxValue)
        {
            uint remainder = unchecked((0U - maxValue) % maxValue);

            while (lowPart < remainder)
            {
                randomProduct = (ulong)maxValue * NextU32();
                lowPart = (uint)randomProduct;
            }
        }

        return (uint)(randomProduct >> 32);
    }

    public int ZeroTo(int maxValue) => (int)ZeroTo((uint)maxValue);

    /// <summary>
    ///
    /// </summary>
    /// <param name="maxValue"><b>exclusive</b> upper bound</param>
    /// <returns></returns>
    public ulong ZeroTo(ulong maxValue)
    {
        ulong randomProduct = MathHelper.BigMul(maxValue, NextU64(), out ulong lowPart);

        if (lowPart < maxValue)
        {
            ulong remainder = unchecked((0UL - maxValue) % maxValue);

            while (lowPart < remainder)
            {
                randomProduct = MathHelper.BigMul(maxValue, NextU64(), out lowPart);
            }
        }

        return randomProduct;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="minValue"><b>inclusive</b> minimum value</param>
    /// <param name="maxValue"><b>exclusive</b> maximum value</param>
    /// <returns></returns>
    public int InRange(int minValue, int maxValue)
    {
        Throw.IfLessThan(maxValue, minValue);

        return ZeroTo((maxValue - minValue)) + minValue;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="minValue"><b>inclusive</b> minimum value</param>
    /// <param name="maxValue"><b>exclusive</b> maximum value</param>
    /// <returns></returns>
    public long InRange(long minValue, long maxValue)
    {
        Throw.IfLessThan(maxValue, minValue);

        return (long)ZeroTo((ulong)(maxValue - minValue)) + minValue;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="span"></param>
    /// <typeparam name="T"></typeparam>
    /// <seealso href="https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle"/>
    public void Shuffle<T>(Span<T> span)
    {
        // fisher-yates
        int n = span.Length;

        int r;
        T temp;

        for (int i = (n - 1); i >= 1; i--)
        {
            r = (int)ZeroTo((uint)i + 1U);
            temp = span[r];
            span[r] = span[i];
            span[i] = temp;
        }
    }

    public void Shuffle<T>(T[]? array)
    {
        if (array is null) return;

        // fisher-yates
        int n = array.Length;

        int r;
        T temp;

        for (int i = (n - 1); i >= 1; i--)
        {
            r = (int)ZeroTo((uint)i + 1U);
            temp = array[r];
            array[r] = array[i];
            array[i] = temp;
        }
    }

    public void Shuffle<T>(IList<T>? list)
    {
        if (list is null) return;

        // fisher-yates
        int n = list.Count;

        int r;
        T temp;

        for (int i = (n - 1); i >= 1; i--)
        {
            r = (int)ZeroTo((uint)i + 1U);
            temp = list[r];
            list[r] = list[i];
            list[i] = temp;
        }
    }

    public override string ToString()
    {
        if (Seed.IsStable)
        {
            return $"{GetType().Render()} w/Seed {Seed}";
        }
        else
        {
            return GetType().Render();
        }
    }
}