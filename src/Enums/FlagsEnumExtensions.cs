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
    /// <summary>
    /// Returns the <see cref="ulong"/> representation of <c>this</c> <see cref="FlagsAttribute">flagged</see> <see cref="Enum">enum</see>
    /// </summary>
    /// <param name="enum"></param>
    /// <typeparam name="TEnum"></typeparam>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong ToUInt64<TEnum>(this TEnum @enum)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Conv_U8();
        return Return<ulong>();
    }
    
    /// <summary>
    /// Returns a <see cref="long"/> representation of this <see cref="Enum"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ToInt64<TEnum>(this TEnum @enum)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Conv_I8();
        return Return<long>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum FromUInt64<TEnum>(ulong value)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(value));
        return Return<TEnum>();
    }

    /// <summary>
    /// Returns a bitwise NOT (<c>~</c>) of this <paramref name="enum"/>
    /// </summary>
    /// <remarks>
    /// Also known as the inverse or bitwise complement
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum BitwiseComplement<TEnum>(this TEnum @enum)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Not();
        return Return<TEnum>();
    }

    /// <summary>
    /// Returns a bitwise AND (<c>&amp;</c>) of the <paramref name="enum"/> and the <paramref name="flag"/>
    /// </summary>
    /// <typeparam name="TEnum">The <see cref="Type"/> of <see langword="enum"/> being AND'd</typeparam>
    /// <param name="enum">The first <typeparamref name="TEnum"/> to AND</param>
    /// <param name="flag">The second <typeparamref name="TEnum"/> to AND</param>
    /// <returns>The two <see langword="enum"/>s AND'd together</returns>
    /// <remarks>
    /// <c>return (<paramref name="enum"/> &amp; <paramref name="flag"/>);</c>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum And<TEnum>(this TEnum @enum, TEnum flag)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Ldarg(nameof(flag));
        Emit.And();
        return Return<TEnum>();
    }

    /// <summary>
    /// Returns a bitwise OR (<c>|</c>) of the <paramref name="enum"/> and the <paramref name="flag"/>
    /// </summary>
    /// <typeparam name="TEnum">The <see cref="Type"/> of <see langword="enum"/> being OR'd</typeparam>
    /// <param name="enum">The first <typeparamref name="TEnum"/> to OR</param>
    /// <param name="flag">The second <typeparamref name="TEnum"/> to OR</param>
    /// <returns>The two <see langword="enum"/>s OR'd together</returns>
    /// <remarks>
    /// <c>return (<paramref name="enum"/> | <paramref name="flag"/>);</c>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum Or<TEnum>(this TEnum @enum, TEnum flag)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Ldarg(nameof(flag));
        Emit.Or();
        return Return<TEnum>();
    }

    /// <summary>
    /// Returns a bitwise XOR (<c>^</c>) of the <paramref name="enum"/> and the <paramref name="flag"/>
    /// </summary>
    /// <typeparam name="TEnum">The <see cref="Type"/> of <see langword="enum"/> being XOR'd</typeparam>
    /// <param name="enum">The first <typeparamref name="TEnum"/> to XOR</param>
    /// <param name="flag">The second <typeparamref name="TEnum"/> to XOR</param>
    /// <returns>The two <see langword="enum"/>s XOR'd together</returns>
    /// <remarks>
    /// <c>return (<paramref name="enum"/> ^ <paramref name="flag"/>);</c>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum Xor<TEnum>(this TEnum @enum, TEnum flag)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Ldarg(nameof(flag));
        Emit.Xor();
        return Return<TEnum>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int FlagCount<TEnum>(this TEnum @enum)
        where TEnum : struct, Enum
    {
#if NET6_0_OR_GREATER
        Emit.Ldarg(nameof(@enum));
        Emit.Conv_U8();
        Emit.Call(MethodRef.Method(typeof(BitOperations), nameof(BitOperations.PopCount), typeof(ulong)));
        return Return<int>();
#else
        const ulong MASK_1 = 0b1010101010101010101010101010101_01010101010101010101010101010101UL;
        const ulong MASK_2 = 0b0110011001100110011001100110011_00110011001100110011001100110011UL;
        const ulong MASK_3 = 0b0001111000011110000111100001111_00001111000011110000111100001111UL;
        const ulong MASK_4 = 0b0000001000000010000000100000001_00000001000000010000000100000001UL;
        
        ulong value = ToUInt64<TEnum>(@enum);
        value -= (value >> 1) & MASK_1;
        value = (value & MASK_2) + ((value >> 2) & MASK_2);
        value = (((value + (value >> 4)) & MASK_3) * MASK_4) >> 56;
        return (int)value;
#endif
    }

    public static TEnum[] GetFlags<TEnum>(this TEnum @enum)
        where TEnum : struct, Enum
    {
        int flagCount = FlagCount(@enum);
        var flags = new TEnum[flagCount];
        int f = 0;
        int maxBits = Unsafe.SizeOf<TEnum>() * 8;
        ulong enumValue = ToUInt64(@enum);
        for (var shift = 0; shift < maxBits; shift++)
        {
            ulong mask = 1UL << shift;
            if ((enumValue & mask) != 0UL)
            {
                var flag = FromUInt64<TEnum>(mask);
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
    public static void AddFlag<TEnum>(this ref TEnum @enum, TEnum flag)
        where TEnum : struct, Enum
    {
        @enum = Or(@enum, flag);
    }

    /// <summary>
    /// Returns this <paramref name="enum"/> combined with the <paramref name="flag"/>
    /// </summary>
    /// <remarks>
    /// <c>return</c> <paramref name="enum"/> <c>|</c> <paramref name="flag"/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum WithFlag<TEnum>(this TEnum @enum, TEnum flag)
        where TEnum : struct, Enum
    {
        return Or(@enum, flag);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RemoveFlag<TEnum>(this ref TEnum @enum, TEnum flag)
        where TEnum : struct, Enum
    {
        @enum = And(@enum, BitwiseComplement(flag));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum WithoutFlag<TEnum>(this TEnum @enum, TEnum flag)
        where TEnum : struct, Enum
    {
        return And(@enum, BitwiseComplement(flag));
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasFlag<TEnum>(this TEnum @enum, TEnum flag)
        where TEnum : struct, Enum
    {
        return @enum.And(flag)
            .NotEqual(default);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasFlags<TEnum>(this TEnum @enum, TEnum flag)
        where TEnum : struct, Enum
    {
        return @enum.And(flag)
            .NotEqual(default);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAnyFlags<TEnum>(this TEnum @enum, TEnum flag)
        where TEnum : struct, Enum
    {
        return @enum.And(flag)
            .NotEqual(default);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAnyFlags<TEnum>(this TEnum @enum, TEnum flag1, TEnum flag2)
        where TEnum : struct, Enum
    {
        return @enum.And(flag1.Or(flag2))
            .NotEqual(default);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAnyFlags<TEnum>(this TEnum @enum, TEnum flag1, TEnum flag2, TEnum flag3)
        where TEnum : struct, Enum
    {
        return @enum.And(flag1.Or(flag2).Or(flag3)).NotEqual(default);
    }

    public static bool HasAnyFlags<TEnum>(this TEnum @enum, params TEnum[] flags)
        where TEnum : struct, Enum
    {
        TEnum flag = default;
        for (var i = 0; i < flags.Length; i++)
        {
            flag.AddFlag(flags[i]);
        }

        return @enum.And(flag)
            .NotEqual(default);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAllFlags<TEnum>(this TEnum @enum, TEnum flag)
        where TEnum : struct, Enum
    {
        return @enum.And(flag)
            .IsEqual(flag);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAllFlags<TEnum>(this TEnum @enum, TEnum flag1, TEnum flag2)
        where TEnum : struct, Enum
    {
        TEnum flag = flag1.Or(flag2);
        return @enum.And(flag)
            .IsEqual(flag);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAllFlags<TEnum>(this TEnum @enum, TEnum flag1, TEnum flag2, TEnum flag3)
        where TEnum : struct, Enum
    {
        TEnum flag = flag1.Or(flag2)
            .Or(flag3);
        return @enum.And(flag)
            .IsEqual(flag);
    }

    public static bool HasAllFlags<TEnum>(this TEnum @enum, params TEnum[] flags)
        where TEnum : struct, Enum
    {
        TEnum flag = default;
        for (var i = 0; i < flags.Length; i++)
        {
            flag.AddFlag(flags[i]);
        }

        return @enum.And(flag)
            .IsEqual(flag);
    }
}