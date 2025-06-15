namespace ScrubJay.Enums;

[PublicAPI]
public readonly struct EnumWrapper<E> :
#if NET7_0_OR_GREATER
    IEqualityOperators<EnumWrapper<E>, E, bool>,
    IComparisonOperators<EnumWrapper<E>, E, bool>,
    IBitwiseOperators<EnumWrapper<E>, E, E>,
    ISpanParsable<EnumWrapper<E>>,
    IParsable<EnumWrapper<E>>,
#endif
#if NET6_0_OR_GREATER
    ISpanFormattable,
#endif
    IEquatable<EnumWrapper<E>>,
    IEquatable<E>,
    IComparable<EnumWrapper<E>>,
    IComparable<E>,
    IFormattable
    where E : struct, Enum
{
    public static implicit operator EnumWrapper<E>(E @enum) => new(@enum);
    public static implicit operator E(EnumWrapper<E> wrapper) => wrapper.Enum;

    public static bool operator ==(EnumWrapper<E> left, E right) => left.Equals(right);
    public static bool operator !=(EnumWrapper<E> left, E right) => !left.Equals(right);

    public static bool operator >(EnumWrapper<E> left, E right) => left.CompareTo(right) > 0;
    public static bool operator >=(EnumWrapper<E> left, E right) => left.CompareTo(right) >= 0;
    public static bool operator <(EnumWrapper<E> left, E right) => left.CompareTo(right) < 0;
    public static bool operator <=(EnumWrapper<E> left, E right) => left.CompareTo(right) <= 0;

    public static E operator &(EnumWrapper<E> left, E right) => left.Enum.And(right);
    public static E operator |(EnumWrapper<E> left, E right) => left.Enum.Or(right);
    public static E operator ^(EnumWrapper<E> left, E right) => left.Enum.Xor(right);
    public static E operator ~(EnumWrapper<E> wrapper) => wrapper.Enum.BitwiseComplement();

#region Parse

    public static EnumWrapper<E> Parse(string str, IFormatProvider? _ = default)
    {
#if NETFRAMEWORK || NETSTANDARD2_0
        object obj = System.Enum.Parse(typeof(E), str, true);
        E e = obj.ThrowIfNot<E>();
        return new(e);
#else
        return System.Enum.Parse<E>(str, true);
#endif
    }

    public static EnumWrapper<E> Parse(text text, IFormatProvider? _ = default)
        => Parse(text.ToString());

    public static bool TryParse([NotNullWhen(true)] string? str, IFormatProvider? _, out EnumWrapper<E> result)
    {
        if (!System.Enum.TryParse(str, true, out result))
        {
            result = default;
            return false;
        }

        return true;
    }

    public static bool TryParse(text text, IFormatProvider? _, out EnumWrapper<E> result)
        => TryParse(text.ToString(), _, out result);

#endregion


    public readonly E Enum;

    public EnumWrapper(E @enum)
    {
        Enum = @enum;
    }

    public int CompareTo(E other) => Comparer<E>.Default.Compare(Enum, other);

    public int CompareTo(EnumWrapper<E> other) => Comparer<E>.Default.Compare(Enum, other.Enum);

    public bool Equals(E other) => Enum.IsEqual(other);

    public bool Equals(EnumWrapper<E> other) => Enum.IsEqual(other.Enum);

    public bool Equals(long value) => Enum.ToInt64() == value;

    public bool Equals(ulong value) => Enum.ToUInt64() == value;

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj switch
        {
            E @enum => Equals(@enum),
            EnumWrapper<E> wrapper => Equals(wrapper),
            ulong uint64 => Equals(uint64),
            long int64 => Equals(int64),
            _ => false,
        };

        // Should we also support string?
    }

    public override int GetHashCode() => Enum.GetHashCode();

    public bool TryFormat(
        Span<char> destination,
        out int charsWritten,
        text format = default,
        IFormatProvider? provider = default)
    {
        return new TryFormatWriter(destination)
        {
            Enum,
        }.GetResult(out charsWritten);
    }

    public string ToString(
        [StringSyntax(StringSyntaxAttribute.EnumFormat)]
        string? format,
        IFormatProvider? formatProvider = default) => Enum.ToString(format);

    public override string ToString() => Enum.AsString();
}
