﻿// ReSharper disable InvokeAsExtensionMethod

namespace ScrubJay.Enums;

/// <summary>
/// A wrapper around a <typeparamref name="TEnum"/> value that exposes interfaces and additional operations
/// </summary>
/// <typeparam name="TEnum"></typeparam>
public readonly struct EnumWrapper<TEnum> :
#if NET7_0_OR_GREATER
    IEqualityOperators<EnumWrapper<TEnum>, TEnum, bool>,
    IComparisonOperators<EnumWrapper<TEnum>, TEnum, bool>,
#endif
    IComparable<EnumWrapper<TEnum>>,
    IComparable<TEnum>,
    IEquatable<EnumWrapper<TEnum>>,
    IEquatable<TEnum>
    where TEnum : struct, Enum
{
    public static implicit operator TEnum(EnumWrapper<TEnum> enumWrapper) => enumWrapper._enum;
    public static implicit operator EnumWrapper<TEnum>(TEnum @enum) => new(@enum);

    public static bool operator ==(EnumWrapper<TEnum> left, TEnum right) => EnumExtensions.Equal(left._enum, right);
    public static bool operator !=(EnumWrapper<TEnum> left, TEnum right) => EnumExtensions.NotEqual(left._enum, right);
    public static bool operator >(EnumWrapper<TEnum> left, TEnum right) => EnumExtensions.GreaterThan(left._enum, right);
    public static bool operator >=(EnumWrapper<TEnum> left, TEnum right) => EnumExtensions.GreaterThanOrEqual(left._enum, right);
    public static bool operator <(EnumWrapper<TEnum> left, TEnum right) => EnumExtensions.LessThan(left._enum, right);
    public static bool operator <=(EnumWrapper<TEnum> left, TEnum right) => EnumExtensions.LessThanOrEqual(left._enum, right);

    private readonly TEnum _enum;

    public string Name => _enum.ToString();

    public EnumWrapper(TEnum @enum)
    {
        _enum = @enum;
    }

    public int CompareTo(EnumWrapper<TEnum> other)
    {
        return EnumExtensions.CompareTo(_enum, other._enum);
    }

    public int CompareTo(TEnum other)
    {
        return EnumExtensions.CompareTo(_enum, other);
    }

    public bool Equals(EnumWrapper<TEnum> enumWrapper)
    {
        return EnumExtensions.Equals(_enum, enumWrapper._enum);
    }

    public bool Equals(TEnum @enum)
    {
        return EnumExtensions.Equals(_enum, @enum);
    }

    public override bool Equals(object? obj)
    {
        return obj switch
        {
            TEnum @enum => Equals(@enum),
            EnumWrapper<TEnum> enumWrapper => Equals(enumWrapper),
            _ => false,
        };
    }

    public override int GetHashCode()
    {
        return _enum.GetHashCode();
    }

    public override string ToString()
    {
        return $"{typeof(TEnum).Name}.{Name}";
    }
}