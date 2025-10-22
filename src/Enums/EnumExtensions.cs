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
    extension(Enum)
    {
        public static bool TryParse(Type enumType, object value, [NotNullWhen(true)] out Enum? @enum)
        {
            if (value is string str)
            {
#if NETSTANDARD2_0
                if (Enum.TryParse(enumType, str, out var obj))
#else
                if (Enum.TryParse(enumType, str, true, out var obj))
#endif
                {
                    return obj.Is(out @enum);
                }
                else
                {
                    @enum = null;
                    return false;
                }
            }

            ulong u64;

            if (value is ulong)
            {
                u64 = (ulong)value;
            }
            else if (value is long i64)
            {
                u64 = (ulong)i64;
            }
            else if (value is uint u32)
            {
                u64 = u32;
            }
            else if (value is int i32)
            {
                u64 = (ulong)i32;
            }
            else if (value is ushort u16)
            {
                u64 = u16;
            }
            else if (value is short i16)
            {
                u64 = (ulong)i16;
            }
            else if (value is byte u8)
            {
                u64 = u8;
            }
            else if (value is sbyte i8)
            {
                u64 = (ulong)i8;
            }
            else
            {
                throw Ex.NotImplemented();
            }

            var underType = Enum.GetUnderlyingType(enumType).ThrowIfNull();

            if (underType == typeof(int))
            {
                if (u64 > int.MaxValue)
                    throw Ex.Invalid();
                if (Enum.IsDefined(enumType, (int)u64))
                {
                    @enum = Enum.ToObject(enumType, (int)u64).ThrowIfNot<Enum>();
                    return true;
                }
                else
                {
                    @enum = null;
                    return false;
                }
            }
            else
            {
                throw Ex.NotImplemented();
            }
        }

        /// <summary>
        /// Returns the underlying <see cref="Type"/> for the generic <see cref="Enum"/> type <typeparamref name="E"/>
        /// </summary>
        public static Type UnderlyingType<E>()
            where E : Enum
            => typeof(E).GetEnumUnderlyingType();
    }

    extension(Enum @enum)
    {
        public ulong ToUInt64()
        {
            return ((IConvertible)@enum).ToUInt64(null);
        }

        public long ToInt64()
        {
            return ((IConvertible)@enum).ToInt64(null);
        }
    }

    extension<E>(E @enum)
        where E : struct, Enum
    {
        public bool IsDefault()
        {
            Emit.Ldarg_0();
            Emit.Ldc_I4_0();
            Emit.Ceq();
            return Return<bool>();
        }
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
    public static bool IsEqual<E>([AllowNull] this E @enum, [AllowNull] E other)
        where E : Enum
    {
        Emit.Ldarg(nameof(@enum));
        Emit.Ldarg(nameof(other));
        Emit.Ceq();
        return Return<bool>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool NotEqual<E>([AllowNull] this E @enum, [AllowNull] E other)
        where E : Enum => !IsEqual<E>(@enum, other);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool LessThan<E>(this E @enum, E other)
        where E : Enum
        => Comparer<E>.Default.Compare(@enum, other) < 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool LessThanOrEqual<E>(this E @enum, E other)
        where E : Enum
        => Comparer<E>.Default.Compare(@enum, other) <= 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool GreaterThan<E>(this E @enum, E other)
        where E : Enum
        => Comparer<E>.Default.Compare(@enum, other) > 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool GreaterThanOrEqual<E>(this E @enum, E other)
        where E : Enum
        => Comparer<E>.Default.Compare(@enum, other) >= 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CompareTo<E>(this E @enum, E other)
        where E : Enum
        => Comparer<E>.Default.Compare(@enum, other);

    public static string AsString<E>(this E @enum)
        where E : Enum
        => @enum.ToString();
}