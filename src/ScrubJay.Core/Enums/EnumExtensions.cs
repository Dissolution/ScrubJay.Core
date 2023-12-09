// ReSharper disable EntityNameCapturedOnly.Global

using static InlineIL.IL;

namespace ScrubJay.Enums;

/// <summary>
/// Generic (non-boxing) extensions upon <c>enum</c>
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Is <c>this</c> <paramref name="enum"/> the <c>default</c> value for its <see cref="Type"/>?
    /// </summary>
    /// <param name="enum">
    /// <c>this</c> <see cref="Enum"/> to check
    /// </param>
    /// <typeparam name="TEnum">
    /// The <see cref="Type"/> of the <see cref="Enum"/>
    /// </typeparam>
    /// <returns>
    /// <c>true</c> if <paramref name="enum"/> is <c>== default(TEnum)</c><br/>
    /// <c>false</c> if it is not
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDefault<TEnum>(this TEnum @enum)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Ldc_I4_0();
        Emit.Ceq();
        return Return<bool>();
    }

    /// <summary>
    /// Does <c>this</c> <paramref name="enum"/> equal the <paramref name="other"/>?
    /// </summary>
    /// <param name="enum">
    /// <c>this</c> <see cref="Enum"/> to equate
    /// </param>
    /// <param name="other">
    /// The other <see cref="Enum"/> to equate
    /// </param>
    /// <typeparam name="TEnum">
    /// The <see cref="Type"/> of the <see cref="Enum">enums</see>
    /// </typeparam>
    /// <returns>
    /// <c>true</c> if the two <see cref="Enum">Enums</see> have the same value<br/>
    /// <c>false</c> if they do not
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equal<TEnum>(this TEnum @enum, TEnum other)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Ldarg(nameof(other));
        Emit.Ceq();
        return Return<bool>();
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool NotEqual<TEnum>(this TEnum @enum, TEnum other)
        where TEnum : struct, Enum => !Equal<TEnum>(@enum, other);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool LessThan<TEnum>(this TEnum @enum, TEnum other)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Ldarg(nameof(other));
        Emit.Clt();
        return Return<bool>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool LessThanOrEqual<TEnum>(this TEnum @enum, TEnum other)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Ldarg(nameof(other));
        Emit.Cgt();
        Emit.Ldc_I4_0();
        Emit.Ceq();
        return Return<bool>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool GreaterThan<TEnum>(this TEnum @enum, TEnum other)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Ldarg(nameof(other));
        Emit.Cgt();
        return Return<bool>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool GreaterThanOrEqual<TEnum>(this TEnum @enum, TEnum other)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Ldarg(nameof(other));
        Emit.Clt();
        Emit.Ldc_I4_0();
        Emit.Ceq();
        return Return<bool>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CompareTo<TEnum>(this TEnum @enum, TEnum other)
        where TEnum : struct, Enum
    {
        if (LessThan(@enum, other))
            return -1;
        if (GreaterThan(@enum, other))
            return 1;

        return 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ToInt32<TEnum>(this TEnum @enum)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Conv_I4();
        return Return<int>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ToInt64<TEnum>(this TEnum @enum)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Conv_I8();
        return Return<long>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong ToUInt64<TEnum>(this TEnum @enum)
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Conv_U8();
        return Return<ulong>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum FromUInt64<TEnum>(ulong value)
    {
        Emit.Ldarg(nameof(value));
        return Return<TEnum>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T To<TEnum, T>(TEnum @enum)
        where TEnum : struct, Enum
        where T : unmanaged
    {
        Emit.Ldarg(nameof(@enum));
        return Return<T>();
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum From<T, TEnum>(T input)
        where T : unmanaged
        where TEnum : struct, Enum
    {
        Emit.Ldarg(nameof(input));
        return Return<TEnum>();
    }
}