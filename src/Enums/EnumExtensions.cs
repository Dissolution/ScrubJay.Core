// ReSharper disable EntityNameCapturedOnly.Global

using static InlineIL.IL;

namespace ScrubJay.Enums;

/// <summary>
/// Generic (non-boxing) extensions upon <c>enum</c>
/// </summary>
public static class EnumExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Type UnderlyingType<TEnum>()
        where TEnum : struct, Enum
        => typeof(TEnum).GetEnumUnderlyingType();
    
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
        return Comparer<TEnum>.Default.Compare(@enum, other) < 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool LessThanOrEqual<TEnum>(this TEnum @enum, TEnum other)
        where TEnum : struct, Enum
    {
        return Comparer<TEnum>.Default.Compare(@enum, other) <= 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool GreaterThan<TEnum>(this TEnum @enum, TEnum other)
        where TEnum : struct, Enum
    {
        return Comparer<TEnum>.Default.Compare(@enum, other) > 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool GreaterThanOrEqual<TEnum>(this TEnum @enum, TEnum other)
        where TEnum : struct, Enum
    {
        return Comparer<TEnum>.Default.Compare(@enum, other) >= 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CompareTo<TEnum>(this TEnum @enum, TEnum other)
        where TEnum : struct, Enum
    {
        return Comparer<TEnum>.Default.Compare(@enum, other);
    }
}