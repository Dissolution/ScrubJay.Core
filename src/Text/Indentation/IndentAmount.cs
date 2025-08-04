namespace ScrubJay.Text.Indentation;

[PublicAPI]
[StructLayout(LayoutKind.Explicit, Size = 1)]
public readonly struct IndentAmount :
#if NET7_0_OR_GREATER
    IEqualityOperators<IndentAmount, IndentAmount, bool>,
#endif
    IEquatable<IndentAmount>
{
    public static implicit operator IndentAmount(bool indent) => indent ? Full : None;
    public static implicit operator bool(IndentAmount amount) => amount._value > 0;

    public static bool operator ==(IndentAmount left, IndentAmount right) => left.Equals(right);
    public static bool operator !=(IndentAmount left, IndentAmount right) => !left.Equals(right);

    public static readonly IndentAmount None = new(0);
    public static readonly IndentAmount Half = new(1);
    public static readonly IndentAmount Full = new(2);

    [FieldOffset(0)]
    private readonly byte _value;

    private IndentAmount(byte value)
    {
        _value = value;
    }

    public bool Equals(IndentAmount other)
    {
        return other._value == _value;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj switch
        {
            IndentAmount amount => Equals(amount),
            bool indent => indent == _value > 0,
            _ => false,
        };
    }

    public override int GetHashCode() => _value;

    public override string ToString()
    {
        return _value switch
        {
            0 => "None",
            1 => "Half",
            2 => "Full",
            _ => throw new UnreachableException(),
        };
    }
}