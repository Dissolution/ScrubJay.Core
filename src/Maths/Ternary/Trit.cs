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
    IEquatable<Trit>

//ITrySpanParsable<Trit>,
// IEqualityOperators<Trit, Trit, bool>,
// IEqualityOperators<Trit, bool, bool>,
// IComparisonOperators<Trit, Trit, Trit>,
// IComparisonOperators<Trit, bool, Trit>,

// IEquatable<Trit>,
// IComparable<Trit>
{
    //public static implicit operator Trit(bool boolean) => boolean ? True : False;

    public static bool operator true(Trit trit)
    {
        return trit == True;
    }

    public static bool operator false(Trit trit)
    {
        return trit == False;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Trit operator !(Trit trit)
    {
        Emit.Ldarg(nameof(trit));
        Emit.Neg();
        Emit.Conv_I1();
        return Return<Trit>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Trit operator &(Trit left, Trit right)
    {
        Emit.Ldarg(nameof(left));
        Emit.Ldarg(nameof(right));
        Emit.Ble_S("left_le_right");
        Emit.Ldarg(nameof(right));
        Emit.Ret();
        MarkLabel("left_le_right");
        Emit.Ldarg(nameof(left));
        Emit.Ret();
        throw Unreachable();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Trit operator |(Trit left, Trit right)
    {
        Emit.Ldarg(nameof(left));
        Emit.Ldarg(nameof(right));
        Emit.Bge_S("left_ge_right");
        Emit.Ldarg(nameof(right));
        Emit.Ret();
        MarkLabel("left_ge_right");
        Emit.Ldarg(nameof(left));
        Emit.Ret();
        throw Unreachable();
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Trit left, Trit right)
    {
        return left._value == right._value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Trit left, Trit right)
    {
        return left._value != right._value;
    }

    public static Trit operator ^(Trit left, Trit right)
    {
        return (left & !right) | (!left & right);
    }

    public static Trit operator ~(Trit value)
    {
        throw Ex.NotSupported();
    }

    internal const sbyte TRUE_VALUE = +1;
    internal const sbyte UNKNOWN_VALUE = 0;
    internal const sbyte FALSE_VALUE = -1;

    public static readonly Trit True = new(TRUE_VALUE);
    public static readonly Trit Unknown = new(UNKNOWN_VALUE);
    public static readonly Trit False = new(FALSE_VALUE);


    [FieldOffset(0)] private readonly sbyte _value;

    private Trit(sbyte value)
    {
        _value = value;
    }

    public Trit And(Trit other)
    {
        return this & other;
    }

    public Trit Or(Trit other)
    {
        return this | other;
    }

    public Trit Not()
    {
        return !this;
    }

    public Trit Xor(Trit other)
    {
        return this ^ other;
    }


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
        return _value.Equals(trit._value);
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

    public override int GetHashCode()
    {
        return _value;
    }

    public override string ToString()
        => _value switch
        {
            TRUE_VALUE => "True",
            FALSE_VALUE => "False",
            _ => "Unknown"
        };
}