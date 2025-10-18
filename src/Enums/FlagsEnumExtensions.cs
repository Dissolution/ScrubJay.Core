#pragma warning disable IDE0060, CA1045
// ReSharper disable EntityNameCapturedOnly.Global

#if NET6_0_OR_GREATER
using InlineIL;
#endif

using static InlineIL.IL;

namespace ScrubJay.Enums;

/// <summary>
/// Generic (non-boxing) extensions upon <c>[Flags] enum</c>
/// </summary>
/// <remarks>
/// There is no way to constrict these extensions to flagged enums,
/// so beware of odd behavior for non-flagged enums
/// </remarks>
[PublicAPI]
public static class FlagsEnumExtensions
{
    extension(Enum @enum)
    {
        private ulong MaxMask()
        {
            var t = Enum.GetUnderlyingType(@enum.GetType());
            if (t == typeof(int))
                return int.MaxValue;
            if (t == typeof(long))
                return long.MaxValue;
            if (t == typeof(short))
                return (ulong)short.MaxValue;
            if (t == typeof(sbyte))
                return (ulong)sbyte.MaxValue;
            if (t == typeof(uint))
                return uint.MaxValue;
            if (t == typeof(ulong))
                return ulong.MaxValue;
            if (t == typeof(ushort))
                return ushort.MaxValue;
            if (t == typeof(byte))
                return byte.MaxValue;
            throw Ex.Arg(t);
        }

        public IEnumerable<Enum> EnumerateFlags()
        {
            ulong value = EnumExtensions.ToUInt64(@enum);
            ulong max = MaxMask(@enum);
            for (ulong mask = 1UL; mask <= max; mask <<= 1)
            {
                if ((value & mask) == mask)
                {
                    if (Enum.TryParse(@enum.GetType(), mask, out var flag))
                    {
                        yield return flag;
                    }
                }
            }
        }
    }




    /// <summary>
    /// Returns the <see cref="ulong"/> representation of <c>this</c> <see cref="FlagsAttribute">flagged</see> <see cref="Enum">enum</see>
    /// </summary>
    /// <param name="enum"></param>
    /// <typeparam name="E"></typeparam>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong ToUInt64<E>(this E @enum)
        where E : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Conv_U8();
        return Return<ulong>();
    }

    /// <summary>
    /// Returns a <see cref="long"/> representation of this <see cref="Enum"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ToInt64<E>(this E @enum)
        where E : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Conv_I8();
        return Return<long>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static E FromUInt64<E>(ulong value)
        where E : struct, Enum
    {
        Emit.Ldarg(nameof(value));
        return Return<E>();
    }

    /// <summary>
    /// Returns a bitwise NOT (<c>~</c>) of this <paramref name="enum"/>
    /// </summary>
    /// <remarks>
    /// Also known as the inverse or bitwise complement
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static E BitwiseComplement<E>(this E @enum)
        where E : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Not();
        return Return<E>();
    }

    /// <summary>
    /// Returns a bitwise AND (<c>&amp;</c>) of the <paramref name="enum"/> and the <paramref name="flag"/>
    /// </summary>
    /// <typeparam name="E">The <see cref="Type"/> of <see langword="enum"/> being AND'd</typeparam>
    /// <param name="enum">The first <typeparamref name="E"/> to AND</param>
    /// <param name="flag">The second <typeparamref name="E"/> to AND</param>
    /// <returns>The two <see langword="enum"/>s AND'd together</returns>
    /// <remarks>
    /// <c>return (<paramref name="enum"/> &amp; <paramref name="flag"/>);</c>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static E And<E>(this E @enum, E flag)
        where E : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Ldarg(nameof(flag));
        Emit.And();
        return Return<E>();
    }

    /// <summary>
    /// Returns a bitwise OR (<c>|</c>) of the <paramref name="enum"/> and the <paramref name="flag"/>
    /// </summary>
    /// <typeparam name="E">The <see cref="Type"/> of <see langword="enum"/> being OR'd</typeparam>
    /// <param name="enum">The first <typeparamref name="E"/> to OR</param>
    /// <param name="flag">The second <typeparamref name="E"/> to OR</param>
    /// <returns>The two <see langword="enum"/>s OR'd together</returns>
    /// <remarks>
    /// <c>return (<paramref name="enum"/> | <paramref name="flag"/>);</c>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static E Or<E>(this E @enum, E flag)
        where E : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Ldarg(nameof(flag));
        Emit.Or();
        return Return<E>();
    }

    /// <summary>
    /// Returns a bitwise XOR (<c>^</c>) of the <paramref name="enum"/> and the <paramref name="flag"/>
    /// </summary>
    /// <typeparam name="E">The <see cref="Type"/> of <see langword="enum"/> being XOR'd</typeparam>
    /// <param name="enum">The first <typeparamref name="E"/> to XOR</param>
    /// <param name="flag">The second <typeparamref name="E"/> to XOR</param>
    /// <returns>The two <see langword="enum"/>s XOR'd together</returns>
    /// <remarks>
    /// <c>return (<paramref name="enum"/> ^ <paramref name="flag"/>);</c>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static E Xor<E>(this E @enum, E flag)
        where E : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Ldarg(nameof(flag));
        Emit.Xor();
        return Return<E>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasOneFlag<E>(this E @enum)
        where E : struct, Enum
    {
        // is power of 2
        // return (x & (x - 1)) == 0

        Emit.Ldarg(nameof(@enum));
        Emit.Ldarg(nameof(@enum));
        Emit.Ldc_I4_1();
        Emit.Sub();
        Emit.And();
        Emit.Ldc_I4_0();
        Emit.Ceq();
        return Return<bool>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasMultipleFlags<E>(this E @enum)
        where E : struct, Enum
        => !HasOneFlag(@enum) && !@enum.IsDefault();


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int FlagCount<E>(this E @enum)
        where E : struct, Enum
    {
#if NET6_0_OR_GREATER
        Emit.Ldarg(nameof(@enum));
        Emit.Conv_U8();
        Emit.Call(MethodRef.Method(typeof(BitOperations), nameof(BitOperations.PopCount), typeof(ulong)));
        return Return<int>();
#else
        const ulong MASK1 = 0b1010101010101010101010101010101_01010101010101010101010101010101UL;
        const ulong MASK2 = 0b0110011001100110011001100110011_00110011001100110011001100110011UL;
        const ulong MASK3 = 0b0001111000011110000111100001111_00001111000011110000111100001111UL;
        const ulong MASK4 = 0b0000001000000010000000100000001_00000001000000010000000100000001UL;

        ulong value = ToUInt64<E>(@enum);
        value -= (value >> 1) & MASK1;
        value = (value & MASK2) + ((value >> 2) & MASK2);
        value = (((value + (value >> 4)) & MASK3) * MASK4) >> 56;
        return (int)value;
#endif
    }

    public static E[] GetFlags<E>(this E @enum)
        where E : struct, Enum
    {
        int flagCount = FlagCount(@enum);
        var flags = new E[flagCount];
        int f = 0;
        int maxBits = Unsafe.SizeOf<E>() * 8;
        ulong enumValue = ToUInt64(@enum);
        for (int shift = 0; shift < maxBits; shift++)
        {
            ulong mask = 1UL << shift;
            if ((enumValue & mask) != 0UL)
            {
                var flag = FromUInt64<E>(mask);
                flags[f++] = flag;
            }
        }
        Debug.Assert(f == flagCount);
        return flags;
    }



    /// <summary>
    /// Adds the <paramref name="flag"/> to this <paramref name="enum"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddFlag<E>(this ref E @enum, E flag)
        where E : struct, Enum
        => @enum = Or(@enum, flag);

    /// <summary>
    /// Returns this <paramref name="enum"/> combined with the <paramref name="flag"/>
    /// </summary>
    /// <remarks>
    /// <c>return</c> <paramref name="enum"/> <c>|</c> <paramref name="flag"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static E WithFlag<E>(this E @enum, E flag)
        where E : struct, Enum
        => Or(@enum, flag);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RemoveFlag<E>(this ref E @enum, E flag)
        where E : struct, Enum
        => @enum = And(@enum, BitwiseComplement(flag));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static E WithoutFlag<E>(this E @enum, E flag)
        where E : struct, Enum
        => And(@enum, BitwiseComplement(flag));


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasFlag<E>(this E @enum, E flag)
        where E : struct, Enum
    {
        return @enum.And(flag)
            .NotEqual<E>(default);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasFlags<E>(this E @enum, E flag)
        where E : struct, Enum
    {
        return @enum.And(flag)
            .NotEqual<E>(default);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAnyFlags<E>(this E @enum, E flag)
        where E : struct, Enum
    {
        return @enum.And(flag)
            .NotEqual<E>(default);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAnyFlags<E>(this E @enum, E flag1, E flag2)
        where E : struct, Enum
    {
        return @enum.And(flag1.Or(flag2))
            .NotEqual<E>(default);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAnyFlags<E>(this E @enum, E flag1, E flag2, E flag3)
        where E : struct, Enum
        => @enum.And(flag1.Or(flag2).Or(flag3)).NotEqual<E>(default);

    public static bool HasAnyFlags<E>(this E @enum, params E[] flags)
        where E : struct, Enum
    {
        E flag = default;
        for (int i = 0; i < flags.Length; i++)
        {
            flag.AddFlag(flags[i]);
        }

        return @enum.And(flag)
            .NotEqual(default);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAllFlags<E>(this E @enum, E flag)
        where E : struct, Enum
    {
        return @enum.And(flag)
            .IsEqual(flag);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAllFlags<E>(this E @enum, E flag1, E flag2)
        where E : struct, Enum
    {
        E flag = flag1.Or(flag2);
        return @enum.And(flag)
            .IsEqual(flag);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAllFlags<E>(this E @enum, E flag1, E flag2, E flag3)
        where E : struct, Enum
    {
        E flag = flag1.Or(flag2)
            .Or(flag3);
        return @enum.And(flag)
            .IsEqual(flag);
    }

    public static bool HasAllFlags<E>(this E @enum, params E[] flags)
        where E : struct, Enum
    {
        E flag = default;
        for (int i = 0; i < flags.Length; i++)
        {
            flag.AddFlag(flags[i]);
        }

        return @enum.And(flag)
            .IsEqual(flag);
    }
}
