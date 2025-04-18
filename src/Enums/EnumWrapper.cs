namespace ScrubJay.Enums;

[PublicAPI]
public readonly struct EnumWrapper<TEnum> :
#if NET7_0_OR_GREATER
    IEqualityOperators<EnumWrapper<TEnum>, TEnum, bool>,
    IComparisonOperators<EnumWrapper<TEnum>, TEnum, bool>,
    IBitwiseOperators<EnumWrapper<TEnum>, TEnum, TEnum>,
    ISpanParsable<EnumWrapper<TEnum>>,
    IParsable<EnumWrapper<TEnum>>,
#endif
#if !NETSTANDARD2_1
    ISpanFormattable,
#endif
    IEquatable<EnumWrapper<TEnum>>,
    IEquatable<TEnum>,
    IComparable<EnumWrapper<TEnum>>,
    IComparable<TEnum>,
    IFormattable
    where TEnum : struct, Enum
{
    public static implicit operator EnumWrapper<TEnum>(TEnum @enum) => new(@enum);
    public static implicit operator TEnum(EnumWrapper<TEnum> wrapper) => wrapper.Enum;

    public static bool operator ==(EnumWrapper<TEnum> left, TEnum right) => left.Equals(right);
    public static bool operator !=(EnumWrapper<TEnum> left, TEnum right) => !left.Equals(right);

    public static bool operator >(EnumWrapper<TEnum> left, TEnum right) => left.CompareTo(right) > 0;
    public static bool operator >=(EnumWrapper<TEnum> left, TEnum right) => left.CompareTo(right) >= 0;
    public static bool operator <(EnumWrapper<TEnum> left, TEnum right) => left.CompareTo(right) < 0;
    public static bool operator <=(EnumWrapper<TEnum> left, TEnum right) => left.CompareTo(right) <= 0;

    public static TEnum operator &(EnumWrapper<TEnum> left, TEnum right) => left.Enum.And(right);
    public static TEnum operator |(EnumWrapper<TEnum> left, TEnum right) => left.Enum.Or(right);
    public static TEnum operator ^(EnumWrapper<TEnum> left, TEnum right) => left.Enum.Xor(right);
    public static TEnum operator ~(EnumWrapper<TEnum> wrapper) => wrapper.Enum.BitwiseComplement();

#region Parse

    public static EnumWrapper<TEnum> Parse(string str, IFormatProvider? _ = default)
    {
#if NETFRAMEWORK || NETSTANDARD2_0
        object obj = System.Enum.Parse(typeof(TEnum), str, true);
        TEnum e = obj.ThrowIfNot<TEnum>();
        return new(e);
#else
        return System.Enum.Parse<TEnum>(str, true);
#endif
    }

    public static EnumWrapper<TEnum> Parse(text text, IFormatProvider? _ = default)
        => Parse(text.ToString());

    public static bool TryParse([NotNullWhen(true)] string? str, IFormatProvider? _, out EnumWrapper<TEnum> result)
    {
        if (!System.Enum.TryParse(str, true, out result))
        {
            result = default;
            return false;
        }

        return true;
    }

    public static bool TryParse(text text, IFormatProvider? _, out EnumWrapper<TEnum> result)
        => TryParse(text.ToString(), _, out result);

#endregion


    public readonly TEnum Enum;

    public EnumWrapper(TEnum @enum)
    {
        Enum = @enum;
    }

    public int CompareTo(TEnum other) => Comparer<TEnum>.Default.Compare(Enum, other);

    public int CompareTo(EnumWrapper<TEnum> other) => Comparer<TEnum>.Default.Compare(Enum, other.Enum);

    public bool Equals(TEnum other) => Enum.IsEqual(other);

    public bool Equals(EnumWrapper<TEnum> other) => Enum.IsEqual(other.Enum);

    public bool Equals(long value) => Enum.ToInt64() == value;

    public bool Equals(ulong value) => Enum.ToUInt64() == value;

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj switch
        {
            TEnum @enum => Equals(@enum),
            EnumWrapper<TEnum> wrapper => Equals(wrapper),
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
