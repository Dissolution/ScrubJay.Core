using ScrubJay.Parsing;
using static InlineIL.IL;

namespace ScrubJay.Maths.Ternary;

/// <summary>
///
/// </summary>
/// <see href="https://en.wikipedia.org/wiki/Balanced_ternary"/>
[StructLayout(LayoutKind.Explicit, Size = 1)]
public readonly struct Trit :
#if NET7_0_OR_GREATER
    IBitwiseOperators<Trit, Trit, Trit>,
#endif
    ITrySpanParsable<Trit>,
    IEquatable<Trit>
{
    public static explicit operator Trit(bool boolean) => boolean ? True : False;

    public static bool operator true(Trit trit)
    {
        return trit == True;
    }

    public static bool operator false(Trit trit)
    {
        return trit == False;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Trit operator !(Trit trit) => trit.Not();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Trit operator &(Trit left, Trit right) => left.And(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Trit operator |(Trit left, Trit right) => left.Or(right);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Trit left, Trit right) => left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Trit left, Trit right) => !left.Equals(right);

    public static Trit operator ^(Trit left, Trit right) => left.Xor(right);

    public static Trit operator ~(Trit value)
    {
        throw Ex.NotSupported();
    }

    public static Result<Trit> TryParse(text text, IFormatProvider? provider = null)
    {
        if (text.Equate(TRUE_STRING))
            return Ok(True);
        if (text.Equate(FALSE_STRING))
            return Ok(False);
        if (text.Equate(UNKNOWN_STRING))
            return Ok(Unknown);
        return Ex.Parse<Trit>(text);
    }


    internal const sbyte TRUE_VALUE = +1;
    internal const sbyte UNKNOWN_VALUE = 0;
    internal const sbyte FALSE_VALUE = -1;

    internal const string TRUE_STRING = "True";
    internal const string UNKNOWN_STRING = "Unknown";
    internal const string FALSE_STRING = "False";

    public static readonly Trit True = new(TRUE_VALUE);
    public static readonly Trit Unknown = new(UNKNOWN_VALUE);
    public static readonly Trit False = new(FALSE_VALUE);


    [FieldOffset(0)] private readonly sbyte _value;

    private Trit(sbyte value)
    {
        _value = value;
    }

    #region Logical operators

    #region Unary

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Trit Not()
    {
        Emit.Ldarg_0(); // this
        Emit.Neg(); // !this
        Emit.Conv_I1(); // (sbyte)!this
        return Return<Trit>(); // (Trit)(sbyte)!this
    }

    #endregion Unary

    #region Binary

    /// <summary>
    /// Logical OR (∨)
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    /// <remarks>
    /// In balanced ternary, an OR operation is just the maximum value
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Trit Or(Trit other)
    {
        Emit.Ldarg_0();
        Emit.Ldarg(nameof(other));
        Emit.Bge_S("this_ge_other");
        Emit.Ldarg(nameof(other));
        Emit.Ret();
        MarkLabel("this_ge_other");
        Emit.Ldarg_0();
        Emit.Ret();
        throw Unreachable();
    }

    /// <summary>
    /// Logical AND (∧)
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    /// <remarks>
    /// In balanced ternary, an AND operation is just the minimum value
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Trit And(Trit other)
    {
        Emit.Ldarg_0();
        Emit.Ldarg(nameof(other));
        Emit.Ble_S("this_le_other");
        Emit.Ldarg(nameof(other));
        Emit.Ret();
        MarkLabel("this_le_other");
        Emit.Ldarg_0();
        Emit.Ret();
        throw Unreachable();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Trit Nor(Trit other)
    {
        return this.Or(other).Not();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Trit Xor(Trit other)
    {
        return (this & !other) | (!this & other);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Trit Xnor(Trit other)
    {
        return (Trit)Equals(other);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Trit Nand(Trit other)
    {
        return this.And(other).Not();
    }

    #endregion Binary

    #endregion Logical operators


    public void Match(
        Action? onTrue,
        Action? onUnknown,
        Action? onFalse)
    {
        if (_value == TRUE_VALUE)
            onTrue?.Invoke();
        else if (_value == FALSE_VALUE)
            onFalse?.Invoke();
        else
            onUnknown?.Invoke();
    }

    public R Match<R>(
        Func<R> onTrue,
        Func<R> onUnknown,
        Func<R> onFalse)
#if NET9_0_OR_GREATER
        where R : allows ref struct
#endif
        => _value switch
        {
            TRUE_VALUE => onTrue.Invoke(),
            FALSE_VALUE => onFalse.Invoke(),
            _ => onUnknown.Invoke()
        };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Trit trit)
    {
        Emit.Ldarg_0();
        Emit.Ldarg(nameof(trit));
        Emit.Ceq();
        return Return<bool>();
    }

    public bool Equals(bool boolean)
    {
        return boolean ? this == True : this == False;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
            return this == Unknown;
        if (obj is Trit trit)
            return Equals(trit);
        if (obj is bool boolean)
            return Equals(boolean);
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode()
    {
        return _value;
    }

    public override string ToString()
        => _value switch
        {
            TRUE_VALUE => TRUE_STRING,
            FALSE_VALUE => FALSE_STRING,
            _ => UNKNOWN_STRING,
        };
}