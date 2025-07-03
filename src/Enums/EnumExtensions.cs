// We're using these parameters
#pragma warning disable IDE0060

// ReSharper disable EntityNameCapturedOnly.Global

using static InlineIL.IL;

namespace ScrubJay.Enums;

/// <summary>
/// Generic (non-boxing) extensions upon <c>enum</c>
/// </summary>
[PublicAPI]
public static class EnumExtensions
{
    /// <summary>
    /// Returns the underlying <see cref="Type"/> for the generic enumeration type <typeparamref name="E"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Type UnderlyingType<E>()
        where E : struct, Enum
        => typeof(E).GetEnumUnderlyingType();

    /// <summary>
    /// Is <c>this</c> <paramref name="enum"/> the <c>default</c> value for its <see cref="Type"/>?
    /// </summary>
    /// <param name="enum">
    /// <c>this</c> <see cref="Enum"/> to check
    /// </param>
    /// <typeparam name="E">
    /// The <see cref="Type"/> of the <see cref="Enum"/>
    /// </typeparam>
    /// <returns>
    /// <c>true</c> if <paramref name="enum"/> is <c>== default(E)</c><br/>
    /// <c>false</c> if it is not
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDefault<E>(this E @enum)
        where E : struct, Enum
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
    /// <typeparam name="E">
    /// The <see cref="Type"/> of the <see cref="Enum">enums</see>
    /// </typeparam>
    /// <returns>
    /// <c>true</c> if the two <see cref="Enum">Enums</see> have the same value<br/>
    /// <c>false</c> if they do not
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEqual<E>(this E @enum, E other)
        where E : struct, Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Ldarg(nameof(other));
        Emit.Ceq();
        return Return<bool>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool NotEqual<E>(this E @enum, E other)
        where E : struct, Enum => !IsEqual<E>(@enum, other);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool LessThan<E>(this E @enum, E other)
        where E : struct, Enum
        => Comparer<E>.Default.Compare(@enum, other) < 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool LessThanOrEqual<E>(this E @enum, E other)
        where E : struct, Enum
        => Comparer<E>.Default.Compare(@enum, other) <= 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool GreaterThan<E>(this E @enum, E other)
        where E : struct, Enum
        => Comparer<E>.Default.Compare(@enum, other) > 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool GreaterThanOrEqual<E>(this E @enum, E other)
        where E : struct, Enum
        => Comparer<E>.Default.Compare(@enum, other) >= 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CompareTo<E>(this E @enum, E other)
        where E : struct, Enum
        => Comparer<E>.Default.Compare(@enum, other);

    public static string AsString<E>(this E @enum)
        where E : struct, Enum
        => @enum.ToString();
}
