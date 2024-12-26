#pragma warning disable IDE0004, CA2225

using System.Globalization;
using ScrubJay.Memory;
// ReSharper disable RedundantOverflowCheckingContext

// ReSharper disable ArrangeThisQualifier
// https://github.com/danm-de/Fractions

namespace ScrubJay.Maths;

[PublicAPI]
[StructLayout(LayoutKind.Auto)]
public readonly struct Rational :
#if NET7_0_OR_GREATER
    INumberBase<Rational>,
    INumber<Rational>,
    ISignedNumber<Rational>,
    IEqualityOperators<Rational, Rational, bool>,
    IComparisonOperators<Rational, Rational, bool>,
    IMultiplyOperators<Rational, Rational, Rational>,
    IDivisionOperators<Rational, Rational, Rational>,
    IAdditionOperators<Rational, Rational, Rational>,
    ISubtractionOperators<Rational, Rational, Rational>,
    IUnaryNegationOperators<Rational, Rational>,
    IUnaryPlusOperators<Rational, Rational>,
    IEqualityOperators<Rational, BigInteger, bool>,
    IComparisonOperators<Rational, BigInteger, bool>,
    IMultiplyOperators<Rational, BigInteger, Rational>,
    IDivisionOperators<Rational, BigInteger, Rational>,
    IAdditionOperators<Rational, BigInteger, Rational>,
    ISubtractionOperators<Rational, BigInteger, Rational>,
    IEqualityOperators<Rational, double, bool>,
    IComparisonOperators<Rational, double, bool>,
    IMultiplyOperators<Rational, double, Rational>,
    IDivisionOperators<Rational, double, Rational>,
    IAdditionOperators<Rational, double, Rational>,
    ISubtractionOperators<Rational, double, Rational>,
    IEqualityOperators<Rational, decimal, bool>,
    IComparisonOperators<Rational, decimal, bool>,
    IMultiplyOperators<Rational, decimal, Rational>,
    IDivisionOperators<Rational, decimal, Rational>,
    IAdditionOperators<Rational, decimal, Rational>,
    ISubtractionOperators<Rational, decimal, Rational>,
    IMinMaxValue<Rational>,
    ISpanParsable<Rational>,
    IParsable<Rational>,
#endif
    IEquatable<Rational>,
    IEquatable<BigInteger>,
    IEquatable<double>,
    IEquatable<decimal>,
    IComparable<Rational>,
    IComparable<BigInteger>,
    IComparable<double>,
    IComparable<decimal>,
    ISpanFormattable,
    IFormattable
{
    public static explicit operator Rational(decimal dec) => FromDecimal(dec);
    public static explicit operator Rational(double f64) => FromDouble(f64, double.Epsilon);
    public static explicit operator Rational(float f32) => FromDouble(f32, float.Epsilon);
    public static explicit operator Rational(BigInteger iBig) => New(iBig, BigInteger.One);

    public static Rational operator -(Rational value) => value * BigInteger.MinusOne;
    public static Rational operator +(Rational value) => value; // no-op

    public static Rational operator --(Rational value) => value - BigInteger.One;
    public static Rational operator ++(Rational value) => value + BigInteger.One;

    public static bool operator ==(Rational left, Rational right) => left.Equals(right);
    public static bool operator !=(Rational left, Rational right) => !left.Equals(right);
    public static bool operator >(Rational left, Rational right) => left.CompareTo(right) > 0;
    public static bool operator >=(Rational left, Rational right) => left.CompareTo(right) >= 0;
    public static bool operator <(Rational left, Rational right) => left.CompareTo(right) < 0;
    public static bool operator <=(Rational left, Rational right) => left.CompareTo(right) <= 0;

    public static bool operator ==(Rational left, BigInteger right) => left.Equals(right);
    public static bool operator !=(Rational left, BigInteger right) => !left.Equals(right);
    public static bool operator >(Rational left, BigInteger right) => left.CompareTo(right) > 0;
    public static bool operator >=(Rational left, BigInteger right) => left.CompareTo(right) >= 0;
    public static bool operator <(Rational left, BigInteger right) => left.CompareTo(right) < 0;
    public static bool operator <=(Rational left, BigInteger right) => left.CompareTo(right) <= 0;

    public static bool operator ==(Rational left, double right) => left.Equals(right);
    public static bool operator !=(Rational left, double right) => !left.Equals(right);
    public static bool operator >(Rational left, double right) => left.CompareTo(right) > 0;
    public static bool operator >=(Rational left, double right) => left.CompareTo(right) >= 0;
    public static bool operator <(Rational left, double right) => left.CompareTo(right) < 0;
    public static bool operator <=(Rational left, double right) => left.CompareTo(right) <= 0;

    public static bool operator ==(Rational left, decimal right) => left.Equals(right);
    public static bool operator !=(Rational left, decimal right) => !left.Equals(right);
    public static bool operator >(Rational left, decimal right) => left.CompareTo(right) > 0;
    public static bool operator >=(Rational left, decimal right) => left.CompareTo(right) >= 0;
    public static bool operator <(Rational left, decimal right) => left.CompareTo(right) < 0;
    public static bool operator <=(Rational left, decimal right) => left.CompareTo(right) <= 0;

    public static Rational operator +(Rational left, Rational right)
        => New((left.Numerator * right.Denominator) + (left.Denominator * right.Numerator),
        left.Denominator * right.Denominator);

    public static Rational operator +(Rational left, BigInteger right)
        => New(left.Numerator + (left.Denominator * right),
            left.Denominator);

    public static Rational operator +(Rational left, decimal right)
        => left + FromDecimal(right);

    public static Rational operator +(Rational left, double right)
        => left + FromDouble(right);


    public static Rational operator -(Rational left, Rational right)
        => New(
            (left.Numerator * right.Denominator) - (left.Denominator * right.Numerator),
            left.Denominator * right.Denominator);

    public static Rational operator -(Rational left, BigInteger right)
        => New(
            left.Numerator - (left.Denominator * right),
            left.Denominator);

    public static Rational operator -(Rational left, decimal right)
        => left - FromDecimal(right);

    public static Rational operator -(Rational left, double right)
        => left - FromDouble(right);


    public static Rational operator *(Rational left, Rational right)
        => New(
            left.Numerator * right.Numerator,
            left.Denominator * right.Denominator);

    public static Rational operator *(Rational left, BigInteger right)
        => New(
            left.Numerator * right,
            left.Denominator);

    public static Rational operator *(Rational left, decimal right)
        => left * FromDecimal(right);
    public static Rational operator *(Rational left, double right)
        => left * FromDouble(right);


    public static Rational operator /(Rational left, Rational right)
        => New(
            left.Numerator * right.Denominator,
            left.Denominator * right.Numerator);

    public static Rational operator /(Rational left, BigInteger right)
        => New(
            left.Numerator,
            left.Denominator * right);

    public static Rational operator /(Rational left, decimal right)
        => left / FromDecimal(right);

    public static Rational operator /(Rational left, double right)
        => left / FromDouble(right);


    public static Rational operator %(Rational left, Rational right)
    {
        BigInteger num = left.Numerator * right.Denominator;
        BigInteger den = left.Denominator * right.Numerator;
        BigInteger mod = num % den;
        return New(mod, den);
    }


    /// <summary>
    /// The minimum possible <see cref="Rational"/>
    /// </summary>
    /// <remarks><c>BigInteger.MinValue / BigInteger.One</c></remarks>
    public static Rational MinValue => NegativeInfinity;

    /// <summary>
    /// The maximum possible <see cref="Rational"/>
    /// </summary>
    /// <remarks><c>BigInteger.MaxValue / BigInteger.One</c></remarks>
    public static Rational MaxValue => PositiveInfinity;

    public static Rational NegativeInfinity { get; } = new(-BigInteger.One, BigInteger.Zero);
    public static Rational PositiveInfinity { get; } = new(BigInteger.One, BigInteger.Zero);

    /// <summary>
    /// The zero value for a <see cref="Rational"/>
    /// </summary>
    /// <remarks><c>BigInteger.Zero / BigInteger.One</c></remarks>
    public static Rational Zero { get; } = new(BigInteger.Zero, BigInteger.One);

    /// <summary>
    /// The one value for a <see cref="Rational"/>
    /// </summary>
    /// <remarks><c>BigInteger.One / BigInteger.One</c></remarks>
    public static Rational One { get; } = new(BigInteger.One, BigInteger.One);

    /// <summary>
    /// The negative one value for a <see cref="Rational"/>
    /// </summary>
    /// <remarks><c>-BigInteger.One / BigInteger.One</c></remarks>
    public static Rational NegativeOne { get; } = new(BigInteger.MinusOne, BigInteger.One);

    public static Rational AdditiveIdentity => Zero;
    public static Rational MultiplicativeIdentity => One;
    public static int Radix => 10;

    /// <summary>
    /// Create a new <see cref="Rational"/> from a <paramref name="numerator"/> and <paramref name="denominator"/>
    /// </summary>
    /// <param name="numerator">A <see cref="BigInteger"/> numerator for the fraction</param>
    /// <param name="denominator">A <see cref="BigInteger"/> denominator for the fraction</param>
    /// <returns>
    /// A simplified <see cref="Rational"/> representation of <c>numerator / denominator</c>
    /// </returns>
    public static Rational New(BigInteger numerator, BigInteger denominator)
    {
        // +/- infinity
        if (denominator == BigInteger.Zero)
        {
            if (numerator >= BigInteger.Zero)
                return PositiveInfinity;
            return NegativeInfinity;
        }

        // 0/x == 0
        if (numerator == BigInteger.Zero)
            return Zero;

        // x/x == 1
        if (numerator == denominator)
            return One;

        // if the denominator is negative, we want to flip signs
        // so that `x/-y -> -x/y` and `-x/-y -> x/y`
        if (denominator < BigInteger.Zero)
        {
            numerator = -numerator;
            denominator = -denominator;
        }

        // Cannot simplify further
        if (numerator != BigInteger.One && numerator != BigInteger.MinusOne &&
            denominator != BigInteger.One && denominator != BigInteger.MinusOne)
        {
            BigInteger greatestCommonDivisor = BigInteger.GreatestCommonDivisor(numerator, denominator);
            if (greatestCommonDivisor != BigInteger.One && greatestCommonDivisor != BigInteger.MinusOne)
            {
                numerator /= greatestCommonDivisor;
                denominator /= greatestCommonDivisor;
            }
        }

        Rational rational = new(numerator, denominator);
        return rational;
    }

    public static Rational FromDouble(double value, double accuracy = double.Epsilon)
    {
        int sign = value < 0d ? -1 : 1;
        value = value < 0d ? -value : value;
        long integerPart = (long)value;
        value -= integerPart;
        double minimalValue = value - accuracy;
        if (minimalValue < 0.0d)
            return New(sign * integerPart, BigInteger.One);

        double maximalValue = value + accuracy;
        if (maximalValue > 1.0d)
            return New(sign * (integerPart + BigInteger.One), BigInteger.One);

        //int a = 0;
        long b = 1L;
        //int c = 1;
        long d = (long)(1L / maximalValue);
        double leftNumerator = minimalValue; // b * minimalValue - a
        double leftDenominator = 1.0d - (d * minimalValue); // c - d * minimalValue
        double rightNumerator = 1.0d - (d * maximalValue); // c - d * maximalValue
        double rightDenominator = maximalValue; // b * maximalValue - a
        while (true)
        {
            if (leftNumerator < leftDenominator)
                break;
            long n = (long)(leftNumerator / leftDenominator);
            //a += n * c;
            b += n * d;
            leftNumerator -= n * leftDenominator;
            rightDenominator -= n * rightNumerator;
            if (rightNumerator < rightDenominator)
                break;
            n = (long)(rightNumerator / rightDenominator);
            //c += n * a;
            d += n * b;
            leftDenominator -= n * leftNumerator;
            rightNumerator -= n * rightDenominator;
        }


        long denominator = b + d;
        long numerator = (long)((value * denominator) + 0.5d);
        return New(sign * ((integerPart * denominator) + numerator), denominator);
    }

    private static Rational FromDecimalSlow(decimal dec)
    {
#if NETFRAMEWORK || NETSTANDARD
        int[] bits = decimal.GetBits(dec);
#else
        Span<int> bits = stackalloc int[4];
        decimal.GetBits(dec, bits);
#endif
        // Convert the lowest three integers into a 96-bit int

        BigInteger low = new BigInteger((uint)bits[0]);
        BigInteger mid = new BigInteger((uint)bits[1]) << 32;
        BigInteger high = new BigInteger((uint)bits[2]) << 64;

        BigInteger bigInt = unchecked(high | mid | low);

        // High integer massaging
        int massaged = (bits[3] >> 30) & 0x02;
        int exponent = (bits[3] >> 16) & 0xFF;

        BigInteger numerator = (1 - massaged) * bigInt;
        BigInteger denominator = BigInteger.Pow(MathHelper.BigInteger_Ten, exponent);

        var rational = New(numerator, denominator);
        return rational;
    }

    public static Rational FromDecimal(decimal dec)
    {
        var digitCount = dec.TrailingDigitCount();
        if (digitCount <= 18)
        {
            long denominator = (long)Math.Pow(10d, digitCount);
            var numerator = new BigInteger(dec * denominator);
            var rational = New(numerator, denominator);
            return rational;
        }
        else
        {
            return FromDecimalSlow(dec);
        }
    }

#region Parse + TryParse

    private static readonly Dictionary<char, Rational> _fastUnicodeRationals = new()
    {
        { '¼', new Rational(1, 4) },
        { '½', new Rational(1, 2)},
        { '¾', new Rational(3, 4)},
        { '⅐', new Rational(1, 7)},
        { '⅑', new Rational(1, 9)},
        { '⅒', new Rational(1, 10)},
        { '⅓', new Rational(1, 3)},
        { '⅔', new Rational(2, 3)},
        { '⅕', new Rational(1, 5)},
        { '⅖', new Rational(2, 5)},
        { '⅗', new Rational(3, 5)},
        { '⅘', new Rational(4, 5)},
        { '⅙', new Rational(1, 6)},
        { '⅚', new Rational(5, 6)},
        { '⅛', new Rational(1, 8)},
        { '⅜', new Rational(3, 8)},
        { '⅝', new Rational(5, 8)},
        { '⅞', new Rational(7, 8)},
        { '↉', new Rational(0, 3)},
    };

    public static Rational Parse(ReadOnlySpan<char> text, IFormatProvider? provider = default)
    {
        if (TryParse(text, provider, out var fraction))
            return fraction;
        throw new ArgumentException($"Could not parse \"{text}\" into a Rational", nameof(text));
    }

    public static Rational Parse(ReadOnlySpan<char> text, NumberStyles style, IFormatProvider? provider = default)
    {
        if (TryParse(text, style, provider, out var fraction))
            return fraction;
        throw new ArgumentException($"Could not parse \"{text}\" into a Rational", nameof(text));
    }

    public static Rational Parse(string str, IFormatProvider? provider = default)
    {
        if (TryParse(str, provider, out var fraction))
            return fraction;
        throw new ArgumentException($"Could not parse \"{str}\" into a Rational", nameof(str));
    }

    public static Rational Parse(string str, NumberStyles style, IFormatProvider? provider = default)
    {
        if (TryParse(str, style, provider, out var fraction))
            return fraction;
        throw new ArgumentException($"Could not parse \"{str}\" into a Rational", nameof(str));
    }

    public static bool TryParse(ReadOnlySpan<char> text, IFormatProvider? provider, out Rational rational)
        => TryParse(text, NumberStyles.Integer, provider, out rational);

    public static bool TryParse(ReadOnlySpan<char> text, NumberStyles style, IFormatProvider? provider, out Rational rational)
    {
        if (text.Length == 1 && _fastUnicodeRationals.TryGetValue(text[0], out rational))
            return true;

        var segments = text.Splitter('/');
        if (!segments.MoveNext())
        {
            rational = default;
            return false;
        }

#if NET6_0_OR_GREATER || NETSTANDARD2_1
        if (!BigInteger.TryParse(segments.Current.Trim(), style, provider, out BigInteger numerator))
        {
            rational = default;
            return false;
        }
#else
        if (!BigInteger.TryParse(segments.Current.Trim().ToString(), style, provider, out BigInteger numerator))
        {
            rational = default;
            return false;
        }
#endif

        if (!segments.MoveNext())
        {
            // Just a numerator, cannot simplify
            rational = new(numerator, BigInteger.One);
            return true;
        }

#if NET6_0_OR_GREATER || NETSTANDARD2_1
        if (!BigInteger.TryParse(segments.Current.Trim(), style, provider, out BigInteger denominator))
        {
            rational = default;
            return false;
        }
#else
        if (!BigInteger.TryParse(segments.Current.Trim().ToString(), style, provider, out BigInteger denominator))
        {
            rational = default;
            return false;
        }
#endif

        rational = New(numerator, denominator);
        return true;
    }

    public static bool TryParse([NotNullWhen(true)] string? str, IFormatProvider? provider, out Rational rational)
    {
        if (str is null)
        {
            rational = default;
            return false;
        }

        return TryParse(str.AsSpan(), provider, out rational);
    }

    public static bool TryParse([NotNullWhen(true)] string? str, NumberStyles style, IFormatProvider? provider, out Rational rational)
    {
        if (str is null)
        {
            rational = default;
            return false;
        }

        return TryParse(str.AsSpan(), style, provider, out rational);
    }

#endregion


    public static Rational Abs(Rational value)
    {
        if (value.Numerator < BigInteger.Zero)
            return -value;
        return value;
    }

    public static bool IsCanonical(Rational value)
    {
        return IsInteger(value);
    }

    public static bool IsComplexNumber(Rational value) => false;

    public static bool IsEvenInteger(Rational value)
    {
        return IsInteger(value) && ((value.Numerator % 2) == 0);
    }

    public static bool IsFinite(Rational value)
    {
        return value.Denominator != BigInteger.Zero;
    }

    public static bool IsImaginaryNumber(Rational value) => false;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInfinity(Rational value)
    {
        return value.Denominator == BigInteger.Zero;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInteger(Rational value)
    {
        return value.Denominator == BigInteger.One;
    }

    public static bool IsNaN(Rational value)
    {
        return false;
    }

    public static bool IsNegative(Rational value)
    {
        return value.Numerator < BigInteger.Zero;
    }

    public static bool IsNegativeInfinity(Rational value)
    {
        return value.Denominator == BigInteger.Zero && value.Numerator < BigInteger.Zero;
    }

    public static bool IsNormal(Rational value)
    {
        return value != Zero;
    }

    public static bool IsOddInteger(Rational value)
    {
        return IsInteger(value) && ((value.Numerator % 2) != 0);
    }

    public static bool IsPositive(Rational value)
    {
        return value.Numerator >= BigInteger.Zero;
    }

    public static bool IsPositiveInfinity(Rational value)
    {
        return value.Denominator == BigInteger.Zero && value.Numerator >= BigInteger.Zero;
    }

    public static bool IsRealNumber(Rational value)
    {
        return true;
    }

    public static bool IsSubnormal(Rational value)
    {
        // we are not floating-point
        return false;
    }

    public static bool IsZero(Rational value)
    {
        return value == Zero;
    }

    public static Rational MaxMagnitude(Rational x, Rational y)
    {
        Rational ax = Abs(x);
        Rational ay = Abs(y);

        if (ax > ay)
        {
            return x;
        }

        if (ax == ay)
        {
            return IsNegative(x) ? y : x;
        }

        return y;
    }

    public static Rational MaxMagnitudeNumber(Rational x, Rational y) => MaxMagnitude(x, y);

    public static Rational MinMagnitude(Rational x, Rational y)
    {
        Rational ax = Abs(x);
        Rational ay = Abs(y);

        if (ax < ay)
        {
            return x;
        }

        if (ax == ay)
        {
            return IsNegative(x) ? x : y;
        }

        return y;
    }

    public static Rational MinMagnitudeNumber(Rational x, Rational y) => MinMagnitude(x, y);

#if NET7_0_OR_GREATER
    static bool INumberBase<Rational>.TryConvertFromChecked<TOther>(TOther value, out Rational rational)
        => TryConvertFrom<TOther>(value, out rational, true);

    static bool INumberBase<Rational>.TryConvertFromSaturating<TOther>(TOther value, out Rational rational)
        => TryConvertFrom<TOther>(value, out rational);

    static bool INumberBase<Rational>.TryConvertFromTruncating<TOther>(TOther value, out Rational rational)
        => TryConvertFrom<TOther>(value, out rational);
#endif

    public static bool TryConvertFrom<TOther>(TOther value, out Rational rational, bool isChecked = false)
    {
        switch (value)
        {
            case byte uint8:
                rational = new(uint8, BigInteger.One);
                return true;
            case sbyte int8:
                rational = new(int8, BigInteger.One);
                return true;
            case short int16:
                rational = new(int16, BigInteger.One);
                return true;
            case ushort uint16:
                rational = new(uint16, BigInteger.One);
                return true;
            case int int32:
                rational = new(int32, BigInteger.One);
                return true;
            case uint uint32:
                rational = new(uint32, BigInteger.One);
                return true;
            case long int64:
                rational = new(int64, BigInteger.One);
                return true;
            case ulong uint64:
                rational = new(uint64, BigInteger.One);
                return true;
            case nint nint:
                rational = new(nint, BigInteger.One);
                return true;
            case nuint nuint:
                rational = new(nuint, BigInteger.One);
                return true;
            case BigInteger iBig:
                rational = new(iBig, BigInteger.One);
                return true;
            case float f32:
                rational = FromDouble(f32, float.Epsilon);
                return true;
            case double f64:
                rational = FromDouble(f64, double.Epsilon);
                return true;
            case decimal dec:
                rational = FromDecimal(dec);
                return true;
            case string str:
                return TryParse(str, null, out rational);
            case Rational fraction:
                rational = fraction;
                return true;
            default:
                rational = default;
                return false;
        }
    }

#if NET7_0_OR_GREATER
    static bool INumberBase<Rational>.TryConvertToChecked<TOther>(Rational value, [MaybeNullWhen(false)] out TOther result)
        => TryConvertTo<TOther>(value, out result, true);

    static bool INumberBase<Rational>.TryConvertToSaturating<TOther>(Rational value, [MaybeNullWhen(false)] out TOther result)
        => TryConvertTo<TOther>(value, out result);

    static bool INumberBase<Rational>.TryConvertToTruncating<TOther>(Rational value, [MaybeNullWhen(false)] out TOther result)
        => TryConvertTo<TOther>(value, out result);
#endif

    public static bool TryConvertTo<TOther>(Rational rational, [MaybeNullWhen(false)] out TOther other, bool isChecked = false)
    {
        var otherType = typeof(TOther);

        if (otherType == typeof(float))
        {
            float f32 = rational.ToSingle();
            other = Notsafe.DirectCast<float, TOther>(f32);
            return true;
        }
        else if (otherType == typeof(double))
        {
            double f64 = rational.ToDouble();
            other = Notsafe.DirectCast<double, TOther>(f64);
            return true;
        }
        else if (otherType == typeof(decimal))
        {
            decimal f64 = rational.ToDecimal();
            other = Notsafe.DirectCast<decimal, TOther>(f64);
            return true;
        }
        else if (otherType == typeof(string))
        {
            string str = rational.ToString();
            other = Notsafe.DirectCast<string, TOther>(str);
            return true;
        }
        else if (otherType == typeof(Rational))
        {
            other = Notsafe.DirectCast<Rational, TOther>(rational);
            return true;
        }

        // Can this rational even be an Integral value?
        if (IsInteger(rational))
        {
            if (otherType == typeof(byte))
            {
                byte uint8;
                if (isChecked)
                {
                    try
                    {
                        uint8 = checked((byte)rational.Numerator);
                    }
                    catch (OverflowException)
                    {
                        other = default;
                        return false;
                    }
                }
                else
                {
                    uint8 = unchecked((byte)rational.Numerator);
                }

                other = Notsafe.DirectCast<byte, TOther>(uint8);
                return true;
            }
            else if (otherType == typeof(sbyte))
            {
                sbyte int8;
                if (isChecked)
                {
                    try
                    {
                        int8 = checked((sbyte)rational.Numerator);
                    }
                    catch (OverflowException)
                    {
                        other = default;
                        return false;
                    }
                }
                else
                {
                    int8 = unchecked((sbyte)rational.Numerator);
                }

                other = Notsafe.DirectCast<sbyte, TOther>(int8);
                return true;
            }
            else if (otherType == typeof(short))
            {
                short int16;
                if (isChecked)
                {
                    try
                    {
                        int16 = checked((short)rational.Numerator);
                    }
                    catch (OverflowException)
                    {
                        other = default;
                        return false;
                    }
                }
                else
                {
                    int16 = unchecked((short)rational.Numerator);
                }

                other = Notsafe.DirectCast<short, TOther>(int16);
                return true;
            }
            else if (otherType == typeof(ushort))
            {
                ushort uint16;
                if (isChecked)
                {
                    try
                    {
                        uint16 = checked((ushort)rational.Numerator);
                    }
                    catch (OverflowException)
                    {
                        other = default;
                        return false;
                    }
                }
                else
                {
                    uint16 = unchecked((ushort)rational.Numerator);
                }

                other = Notsafe.DirectCast<ushort, TOther>(uint16);
                return true;
            }
            else if (otherType == typeof(int))
            {
                int int32;
                if (isChecked)
                {
                    try
                    {
                        int32 = checked((int)rational.Numerator);
                    }
                    catch (OverflowException)
                    {
                        other = default;
                        return false;
                    }
                }
                else
                {
                    int32 = unchecked((int)rational.Numerator);
                }

                other = Notsafe.DirectCast<int, TOther>(int32);
                return true;
            }
            else if (otherType == typeof(uint))
            {
                uint uint32;
                if (isChecked)
                {
                    try
                    {
                        uint32 = checked((uint)rational.Numerator);
                    }
                    catch (OverflowException)
                    {
                        other = default;
                        return false;
                    }
                }
                else
                {
                    uint32 = unchecked((uint)rational.Numerator);
                }

                other = Notsafe.DirectCast<uint, TOther>(uint32);
                return true;
            }
            else if (otherType == typeof(long))
            {
                long int64;
                if (isChecked)
                {
                    try
                    {
                        int64 = checked((long)rational.Numerator);
                    }
                    catch (OverflowException)
                    {
                        other = default;
                        return false;
                    }
                }
                else
                {
                    int64 = unchecked((long)rational.Numerator);
                }

                other = Notsafe.DirectCast<long, TOther>(int64);
                return true;
            }
            else if (otherType == typeof(ulong))
            {
                ulong uint64;
                if (isChecked)
                {
                    try
                    {
                        uint64 = checked((ulong)rational.Numerator);
                    }
                    catch (OverflowException)
                    {
                        other = default;
                        return false;
                    }
                }
                else
                {
                    uint64 = unchecked((ulong)rational.Numerator);
                }

                other = Notsafe.DirectCast<ulong, TOther>(uint64);
                return true;
            }
            else if (otherType == typeof(nint))
            {
                nint nint;
                if (isChecked)
                {
                    try
                    {
                        nint = checked((nint)rational.Numerator);
                    }
                    catch (OverflowException)
                    {
                        other = default;
                        return false;
                    }
                }
                else
                {
                    nint = unchecked((nint)rational.Numerator);
                }

                other = Notsafe.DirectCast<nint, TOther>(nint);
                return true;
            }
            else if (otherType == typeof(nuint))
            {
                nuint nuint;
                if (isChecked)
                {
                    try
                    {
                        nuint = checked((nuint)rational.Numerator);
                    }
                    catch (OverflowException)
                    {
                        other = default;
                        return false;
                    }
                }
                else
                {
                    nuint = unchecked((nuint)rational.Numerator);
                }

                other = Notsafe.DirectCast<nuint, TOther>(nuint);
                return true;
            }
            else if (otherType == typeof(BigInteger))
            {
                other = Notsafe.DirectCast<BigInteger, TOther>(rational.Numerator);
                return true;
            }
        }

        // We do not know how to cast to this type
        Debugger.Break();
        other = default;
        return false;
    }


    /// <summary>
    /// This <see cref="Rational"/>'s Numerator
    /// </summary>
    public readonly BigInteger Numerator;

    /// <summary>
    /// This <see cref="Rational"/>'s Denominator
    /// </summary>
    public readonly BigInteger Denominator;

    private Rational(BigInteger numerator, BigInteger denominator)
    {
        this.Numerator = numerator;
        this.Denominator = denominator;
    }

    public void Deconstruct(out BigInteger numerator, out BigInteger denominator)
    {
        numerator = this.Numerator;
        denominator = this.Denominator;
    }

    /// <summary>
    /// Get the Reciprocal of this <see cref="Rational"/>
    /// </summary>
    /// <seealso href="https://en.wikipedia.org/wiki/Multiplicative_inverse"/>
    public Rational Reciprocal() => new(Denominator, Numerator);

    /// <summary>
    /// Convert this <see cref="Rational"/> to a <see cref="double"/>
    /// </summary>
    public float ToSingle()
    {
        if (Denominator == BigInteger.Zero)
        {
            if (Numerator >= BigInteger.Zero)
                return float.PositiveInfinity;
            return float.NegativeInfinity;
        }

        return (float)Numerator / (float)Denominator;
    }

    /// <summary>
    /// Convert this <see cref="Rational"/> to a <see cref="double"/>
    /// </summary>
    public double ToDouble()
    {
        if (Denominator == BigInteger.Zero)
        {
            if (Numerator >= BigInteger.Zero)
                return double.PositiveInfinity;
            return double.NegativeInfinity;
        }
        return (double)Numerator / (double)Denominator;
    }

    /// <summary>
    /// Convert this <see cref="Rational"/> to a <see cref="decimal"/>
    /// </summary>
    /// <exception cref="DivideByZeroException">Thrown if <see cref="Denominator"/> is <c>BigInteger.Zero</c></exception>
    public decimal ToDecimal()
    {
        if (Denominator == BigInteger.Zero)
            throw new DivideByZeroException();
        return (decimal)Numerator / (decimal)Denominator;
    }

    public int CompareTo(Rational other)
    {
        // sort infinities first
        if (IsInfinity(this))
        {
            if (IsInfinity(other))
            {
                // Negative Infinities before Positive
                return Numerator.CompareTo(other.Numerator);
            }
            return -1;
        }
        else if (IsInfinity(other))
        {
            return 1;
        }

        // cross multiplication
        BigInteger left = this.Numerator * other.Denominator;
        BigInteger right = this.Denominator * other.Numerator;
        return left.CompareTo(right);
    }

    public int CompareTo(double other)
    {
        return ToDouble().CompareTo(other);
    }

    public int CompareTo(decimal other)
    {
        return ToDecimal().CompareTo(other);
    }

    public int CompareTo(BigInteger other)
    {
        if (Denominator == BigInteger.Zero)
        {
            return Numerator >= 0 ? 1 : -1;
        }
        // cross multiplication
        return this.Numerator.CompareTo(this.Denominator * other);
    }

    public int CompareTo(object? obj)
    {
        if (TryConvertFrom(obj, out Rational rational))
            return CompareTo(rational);
        // We cannot compare to obj, sort it before
        return 1;
    }

    public bool Equals(Rational other)
    {
        if (IsInfinity(this))
        {
            return IsInfinity(other) && Numerator == other.Numerator;
        }
        else if (IsInfinity(other))
        {
            return false;
        }

        // cross multiplication
        BigInteger left = this.Numerator * other.Denominator;
        BigInteger right = this.Denominator * other.Numerator;
        return left.Equals(right);
    }

    public bool Equals(BigInteger other)
    {
        if (IsInfinity(this))
            return false;
        // cross multiplication
        return this.Numerator.Equals(this.Denominator * other);
    }

    public bool Equals(double other)
    {
        return ToDouble().Equals(other);
    }

    public bool Equals(decimal m) => decimal.Equals(ToDecimal(), m);

    public override bool Equals(object? obj)
        => obj switch
        {
            Rational rational => Equals(rational),
            double d => Equals(d),
            decimal m => Equals(m),
            BigInteger iBig => Equals(iBig),
            _ => false,
        };

    public override int GetHashCode() => Hasher.Combine(Numerator, Denominator);

    public bool TryFormat(
        Span<char> destination,
        out int charsWritten,
        ReadOnlySpan<char> format = default,
        IFormatProvider? provider = default)
    {
        var buffer = new Buffer<char>(initialSpan: destination, 0);
        try
        {
            WriteString(ref buffer, format.ToString(), provider);
            if (buffer.Count > destination.Length)
            {
                destination.Clear();
                charsWritten = 0;
                return false;
            }
            else
            {
                charsWritten = buffer.Count;
                return true;
            }
        }
        finally
        {
            buffer.Dispose();
        }
    }

    public string ToString(string? format, IFormatProvider? formatProvider = null)
    {
        Buffer<char> buffer = new();
        WriteString(ref buffer, format, formatProvider);
        return buffer.ToStringAndDispose();
    }


    private void WriteGeneral(ref Buffer<char> buffer)
    {
        buffer.Write(Numerator);
        buffer.Write('/');
        buffer.Write(Denominator);
    }


    private void WriteString(ref Buffer<char> buffer, string? format, IFormatProvider? provider)
    {
        if (string.IsNullOrEmpty(format))
        {
            WriteGeneral(ref buffer);
            return;
        }

        int firstChar = format![0];
        ReadOnlySpan<char> fmt = format.AsSpan(1);
        if (firstChar is ('g' or 'G'))
        {
            buffer.Write(Numerator, fmt, provider);
            buffer.Write('/');
            buffer.Write(Denominator, fmt, provider);
        }
        else if (firstChar == 'M') // Mixed number
        {
            BigInteger numerator = Numerator;
            BigInteger denominator = Denominator;
            if (BigInteger.Abs(numerator) < BigInteger.Abs(denominator) || denominator == BigInteger.Zero)
            {
                WriteGeneral(ref buffer);
                return;
            }

            BigInteger integralPart = numerator / denominator;
            var remainder = this - integralPart;
            buffer.Write(integralPart, fmt, provider);
            if (remainder != Rational.Zero)
            {
                buffer.Write(' ');
                remainder.WriteGeneral(ref buffer);
            }
        }
        else if (firstChar == 'N') // Numerator only
        {
            buffer.Write(Numerator, fmt, provider);
        }
        else if (firstChar == 'D') // Denominator only
        {
            buffer.Write(Denominator, fmt, provider);
        }
        else if (firstChar == 'd')
        {
            buffer.Write(ToDouble(), fmt, provider);
        }
        else if (firstChar == 'm')
        {
            buffer.Write(ToDecimal(), fmt, provider);
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(format));
        }
    }

    public override string ToString()
    {
        return $"{Numerator}/{Denominator}";
    }
}