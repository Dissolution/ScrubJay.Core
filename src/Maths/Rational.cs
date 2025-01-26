#pragma warning disable IDE0004, CA2225, IDE0002, IDE0090, IDE0018, IDE0047

using System.Globalization;
using ScrubJay.Text;

// ReSharper disable RedundantOverflowCheckingContext

// ReSharper disable ArrangeThisQualifier
// https://github.com/danm-de/Fractions

/* The order when referencing other types (equality, comparison, etc):
 * Rational
 * BigDecimal
 * decimal
 * double
 * float (only for helpful rounding circumstances)
 * BigInteger
 * long
 */

namespace ScrubJay.Maths;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// 
/// </remarks>
[PublicAPI]
[StructLayout(LayoutKind.Auto)]
public readonly struct Rational :
#if NET7_0_OR_GREATER
    ISignedNumber<Rational>,
    INumber<Rational>,
    IMinMaxValue<Rational>,
    ISpanParsable<Rational>,
    IParsable<Rational>,
#endif
    IAlgebraic<Rational, Rational>,
    IAlgebraic<Rational, BigDecimal>,
    IAlgebraic<Rational, decimal>,
    IAlgebraic<Rational, double>,
    IAlgebraic<Rational, BigInteger>,
    IAlgebraic<Rational, long>,
    ISpanFormattable,
    IFormattable
{
    #region Operators

    // implicit operations cannot lose precision nor fail
    // explicit operations can lose precision and can throw

    public static implicit operator Rational(BigDecimal bigDec) => FromBigDecimal(bigDec);
    public static implicit operator Rational(decimal dec) => FromDecimal(dec);
    public static implicit operator Rational(double f64) => FromDouble(f64, double.Epsilon);
    public static implicit operator Rational(float f32) => FromDouble(f32, float.Epsilon);
    public static implicit operator Rational(BigInteger bigInt) => New(bigInt, BigInteger.One);
    public static implicit operator Rational(long i64) => New(new BigInteger(i64), BigInteger.One);

    public static explicit operator BigDecimal(Rational rational) => rational.ToBigDecimal();
    public static explicit operator decimal(Rational rational) => rational.ToDecimal();
    public static explicit operator double(Rational rational) => rational.ToDouble();
    public static explicit operator BigInteger(Rational rational) => rational.ToBigInteger();
    public static explicit operator long(Rational rational) => rational.ToInt64();

    public static Rational operator -(Rational value) => new Rational(-value.Numerator, value.Denominator);
    public static Rational operator +(Rational value) => value; // no-op

    public static Rational operator --(Rational value) => value.Subtract(Rational.One);
    public static Rational operator ++(Rational value) => value.Add(Rational.One);

    public static bool operator ==(Rational left, Rational right) => left.Equals(right);
    public static bool operator ==(Rational left, BigDecimal right) => left.Equals(right);
    public static bool operator ==(Rational left, decimal right) => left.Equals(right);
    public static bool operator ==(Rational left, double right) => left.Equals(right);
    public static bool operator ==(Rational left, BigInteger right) => left.Equals(right);
    public static bool operator ==(Rational left, long right) => left.Equals(right);
    
    public static bool operator !=(Rational left, Rational right) => !left.Equals(right);
    public static bool operator !=(Rational left, BigDecimal right) => !left.Equals(right);
    public static bool operator !=(Rational left, decimal right) => !left.Equals(right);
    public static bool operator !=(Rational left, double right) => !left.Equals(right);    
    public static bool operator !=(Rational left, BigInteger right) => !left.Equals(right);
    public static bool operator !=(Rational left, long right) => !left.Equals(right);

    public static bool operator >(Rational left, Rational right) => left.CompareTo(right) > 0;
    public static bool operator >(Rational left, BigDecimal right) => left.CompareTo(right) > 0;
    public static bool operator >(Rational left, decimal right) => left.CompareTo(right) > 0;
    public static bool operator >(Rational left, double right) => left.CompareTo(right) > 0;
    public static bool operator >(Rational left, BigInteger right) => left.CompareTo(right) > 0;
    public static bool operator >(Rational left, long right) => left.CompareTo(right) > 0;
    
    public static bool operator >=(Rational left, Rational right) => left.CompareTo(right) >= 0;
    public static bool operator >=(Rational left, BigDecimal right) => left.CompareTo(right) >= 0;
    public static bool operator >=(Rational left, decimal right) => left.CompareTo(right) >= 0;
    public static bool operator >=(Rational left, double right) => left.CompareTo(right) >= 0;
    public static bool operator >=(Rational left, BigInteger right) => left.CompareTo(right) >= 0;
    public static bool operator >=(Rational left, long right) => left.CompareTo(right) >= 0;
    
    public static bool operator <(Rational left, Rational right) => left.CompareTo(right) < 0;
    public static bool operator <(Rational left, BigDecimal right) => left.CompareTo(right) < 0;
    public static bool operator <(Rational left, decimal right) => left.CompareTo(right) < 0;
    public static bool operator <(Rational left, double right) => left.CompareTo(right) < 0;
    public static bool operator <(Rational left, BigInteger right) => left.CompareTo(right) < 0;
    public static bool operator <(Rational left, long right) => left.CompareTo(right) < 0;
    
    public static bool operator <=(Rational left, Rational right) => left.CompareTo(right) <= 0;
    public static bool operator <=(Rational left, BigDecimal right) => left.CompareTo(right) <= 0;
    public static bool operator <=(Rational left, decimal right) => left.CompareTo(right) <= 0;
    public static bool operator <=(Rational left, double right) => left.CompareTo(right) <= 0;
    public static bool operator <=(Rational left, BigInteger right) => left.CompareTo(right) <= 0;
    public static bool operator <=(Rational left, long right) => left.CompareTo(right) <= 0;

    public static Rational operator +(Rational left, Rational right) => left.Add(right);
    public static Rational operator +(Rational left, BigDecimal right) => left.Add(FromBigDecimal(right));
    public static Rational operator +(Rational left, decimal right) => left.Add(FromDecimal(right));
    public static Rational operator +(Rational left, double right) => left.Add(FromDouble(right));
    public static Rational operator +(Rational left, BigInteger right) => left.Add(right);
    public static Rational operator +(Rational left, long right) => left.Add(right);

    public static Rational operator -(Rational left, Rational right) => left.Subtract(right);
    public static Rational operator -(Rational left, BigDecimal right) => left.Subtract(FromBigDecimal(right));
    public static Rational operator -(Rational left, decimal right) => left.Subtract(FromDecimal(right));
    public static Rational operator -(Rational left, double right) => left.Subtract(FromDouble(right));
    public static Rational operator -(Rational left, BigInteger right) => left.Subtract(right);
    public static Rational operator -(Rational left, long right) => left.Subtract(right);

    public static Rational operator *(Rational left, Rational right) => left.Multiply(right);
    public static Rational operator *(Rational left, BigDecimal right) => left.Multiply(FromBigDecimal(right));
    public static Rational operator *(Rational left, decimal right) => left.Multiply(FromDecimal(right));
    public static Rational operator *(Rational left, double right) => left.Multiply(FromDouble(right));
    public static Rational operator *(Rational left, BigInteger right) => left.Multiply(right);
    public static Rational operator *(Rational left, long right) => left.Multiply(right);

    public static Rational operator /(Rational left, Rational right) => left.Divide(right);
    public static Rational operator /(Rational left, BigDecimal right) => left.Divide(FromBigDecimal(right));
    public static Rational operator /(Rational left, decimal right) => left.Divide(FromDecimal(right));
    public static Rational operator /(Rational left, double right) => left.Divide(FromDouble(right));
    public static Rational operator /(Rational left, BigInteger right) => left.Divide(right);
    public static Rational operator /(Rational left, long right) => left.Divide(right);

    public static Rational operator %(Rational left, Rational right) => left.Mod(right);
    public static Rational operator %(Rational left, BigDecimal right) => left.Mod(FromBigDecimal(right));
    public static Rational operator %(Rational left, decimal right) => left.Mod(FromDecimal(right));
    public static Rational operator %(Rational left, double right) => left.Mod(FromDouble(right));
    public static Rational operator %(Rational left, BigInteger right) => left.Mod(right);
    public static Rational operator %(Rational left, long right) => left.Mod(right);

    #endregion

    #region Static Properties

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

    /// <summary>
    /// Negative Infinity
    /// </summary>
    /// <remarks>
    /// <c>-x/0 == -∞</c>
    /// </remarks>
    public static Rational NegativeInfinity { get; } = new(BigInteger.MinusOne, BigInteger.Zero);
    
    /// <summary>
    /// Positive Infinity
    /// </summary>
    /// <remarks>
    /// <c>+x/0 == +∞</c>
    /// </remarks>
    public static Rational PositiveInfinity { get; } = new(BigInteger.One, BigInteger.Zero);

    /// <summary>
    /// Not A Number
    /// </summary>
    /// <remarks>
    /// <c>0/0 == NaN</c>
    /// </remarks>
    public static Rational NaN { get; } = new(BigInteger.Zero, BigInteger.Zero);

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

    #endregion

    private static bool? MaybeEqual(bool leftResult, bool rightResult)
    {
        if (leftResult != rightResult)
            return false; // different
        if (leftResult)
            return true; // both are true
        return null; // both are false
    }
    
    private static bool? MaybeEqual<TInstance>(Func<TInstance, bool> predicate, TInstance leftInstance, TInstance rightInstance)
    {
        if (predicate(leftInstance))
        {
            if (predicate(rightInstance))
            {
                return true;
            }

            return false;
        }
        else if (predicate(rightInstance))
        {
            return false;
        }
        else
        {
            return null; // neither matched
        }
    }
    
    private static int? ComparePredicateResults<TInstance>(Func<TInstance, bool> predicate, TInstance leftInstance, TInstance rightInstance)
    {
        if (predicate(leftInstance))
        {
            if (predicate(rightInstance))
            {
                return 0; // both have the same result
            }

            // left is, right is not
            return 1; // true > false
        }
        else if (predicate(rightInstance))
        {
            return -1; // false < true
        }
        else
        {
            return null; // neither matched
        }
    }
    
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
            if (numerator > BigInteger.Zero)
                return PositiveInfinity;
            if (numerator < BigInteger.Zero)
                return NegativeInfinity;
            return NaN;
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
            var greatestCommonDivisor = BigInteger.GreatestCommonDivisor(numerator, denominator);
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
        if (double.IsNaN(value))
            return Rational.NaN;
        if (double.IsPositiveInfinity(value))
            return Rational.PositiveInfinity;
        if (double.IsNegativeInfinity(value))
            return Rational.NegativeInfinity;
        
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
#if NETFRAMEWORK || NETSTANDARD || NETCOREAPP
        int[] bits = decimal.GetBits(dec);
#else
        Span<int> bits = stackalloc int[4];
        decimal.GetBits(dec, bits);
#endif
        // Convert the lowest three integers into a 96-bit int

        var low = new BigInteger((uint)bits[0]);
        var mid = new BigInteger((uint)bits[1]) << 32;
        var high = new BigInteger((uint)bits[2]) << 64;

        BigInteger bigInt = unchecked(high | mid | low);

        // High integer massaging
        int massaged = (bits[3] >> 30) & 0b00000010;
        int exponent = (bits[3] >> 16) & 0b11111111;

        BigInteger numerator = (1 - massaged) * bigInt;
        var denominator = BigInteger.Pow(MathHelper.BigInt.Ten, exponent);

        var rational = New(numerator, denominator);
        return rational;
    }

    public static Rational FromBigDecimal(BigDecimal bigDec) => throw new NotImplementedException();

    public static Rational FromDecimal(decimal dec)
    {
        int digitCount = dec.TrailingDigitCount();
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
        { '½', new Rational(1, 2) },
        { '¾', new Rational(3, 4) },
        { '⅐', new Rational(1, 7) },
        { '⅑', new Rational(1, 9) },
        { '⅒', new Rational(1, 10) },
        { '⅓', new Rational(1, 3) },
        { '⅔', new Rational(2, 3) },
        { '⅕', new Rational(1, 5) },
        { '⅖', new Rational(2, 5) },
        { '⅗', new Rational(3, 5) },
        { '⅘', new Rational(4, 5) },
        { '⅙', new Rational(1, 6) },
        { '⅚', new Rational(5, 6) },
        { '⅛', new Rational(1, 8) },
        { '⅜', new Rational(3, 8) },
        { '⅝', new Rational(5, 8) },
        { '⅞', new Rational(7, 8) },
        { '↉', new Rational(0, 3) },
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

    public static bool TryParse(ReadOnlySpan<char> text, IFormatProvider? provider, out Rational rational) =>
        TryParse(text, NumberStyles.Integer, provider, out rational);

    public static bool TryParse(ReadOnlySpan<char> text, NumberStyles style, IFormatProvider? provider,
        out Rational rational)
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

    public static bool TryParse([NotNullWhen(true)] string? str, NumberStyles style, IFormatProvider? provider,
        out Rational rational)
    {
        if (str is null)
        {
            rational = default;
            return false;
        }

        return TryParse(str.AsSpan(), style, provider, out rational);
    }

    #endregion

    #region IsXYZ

    public static bool IsCanonical(Rational value) => IsInteger(value);

    public static bool IsComplexNumber(Rational _) => false;

    public static bool IsEvenInteger(Rational value) => IsInteger(value) && ((value.Numerator % 2) == 0);

    public static bool IsFinite(Rational value) => value.Denominator != BigInteger.Zero;

    public static bool IsImaginaryNumber(Rational _) => false;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInfinity(Rational value) => value.Denominator == BigInteger.Zero;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInteger(Rational value) => value.Denominator == BigInteger.One;

    public static bool IsNaN(Rational value) => value.Denominator == BigInteger.Zero && value.Numerator == BigInteger.Zero;

    public static bool IsNegative(Rational value) => value.Numerator < BigInteger.Zero;

    public static bool IsNegativeInfinity(Rational value) => value.Denominator == BigInteger.Zero && value.Numerator < BigInteger.Zero;

    public static bool IsNormal(Rational value) => value != Zero;

    public static bool IsOddInteger(Rational value) => IsInteger(value) && ((value.Numerator % 2) != 0);

    public static bool IsPositive(Rational value) => value.Numerator >= BigInteger.Zero;

    public static bool IsPositiveInfinity(Rational value) => value.Denominator == BigInteger.Zero && value.Numerator > BigInteger.Zero;

    public static bool IsRealNumber(Rational _) => true;

    public static bool IsSubnormal(Rational _) =>
        // we are not floating-point
        false;

    public static bool IsZero(Rational value) => value == Zero;

    #endregion

    #region Math

    public static Rational Abs(Rational value)
    {
        if (value.Numerator < BigInteger.Zero)
            return -value;
        return value;
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

    #endregion

    #region TryConvert

#if NET7_0_OR_GREATER
    static bool INumberBase<Rational>.TryConvertFromChecked<TOther>(TOther value, out Rational rational) => throw new NotImplementedException();

    static bool INumberBase<Rational>.TryConvertFromSaturating<TOther>(TOther value, out Rational rational) => TryConvertFrom<TOther>(value, out rational);

    static bool INumberBase<Rational>.TryConvertFromTruncating<TOther>(TOther value, out Rational rational) => TryConvertFrom<TOther>(value, out rational);
#endif

    public static bool TryConvertFrom<TOther>(TOther value, out Rational rational)
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
                rational = new(new(nint), BigInteger.One);
                return true;
            case nuint nuint:
                rational = new(new(nuint), BigInteger.One);
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
    static bool INumberBase<Rational>.TryConvertToChecked<TOther>(Rational value, [MaybeNullWhen(false)] out TOther result) => TryConvertTo<TOther>(value, out result, true);

    static bool INumberBase<Rational>.TryConvertToSaturating<TOther>(Rational value, [MaybeNullWhen(false)] out TOther result) => TryConvertTo<TOther>(value, out result);

    static bool INumberBase<Rational>.TryConvertToTruncating<TOther>(Rational value, [MaybeNullWhen(false)] out TOther result) => TryConvertTo<TOther>(value, out result);
#endif

    public static bool TryConvertTo<TOther>(Rational rational, [MaybeNullWhen(false)] out TOther other,
        bool isChecked = false)
    {
        var otherType = typeof(TOther);

        if (otherType == typeof(float))
        {
            return rational
                .TryConvertToFloat()
                .Select(Notsafe.As<float, TOther>)
                .HasOk(out other);
        }
        else if (otherType == typeof(double))
        {
            return rational
                .TryConvertToDouble()
                .Select(Notsafe.As<double, TOther>)
                .HasOk(out other);
        }
        else if (otherType == typeof(decimal))
        {
            return rational
                .TryConvertToDecimal()
                .Select(Notsafe.As<decimal, TOther>)
                .HasOk(out other);
        }
        else if (otherType == typeof(BigDecimal))
        {
            return rational
                .TryConvertToBigDecimal()
                .Select(Notsafe.As<BigDecimal, TOther>)
                .HasOk(out other);
        }
        else if (otherType == typeof(string))
        {
            string str = rational.ToString();
            other = Notsafe.As<string, TOther>(str);
            return true;
        }
        else if (otherType == typeof(Rational))
        {
            other = Notsafe.As<Rational, TOther>(rational);
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

                other = Notsafe.As<byte, TOther>(uint8);
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

                other = Notsafe.As<sbyte, TOther>(int8);
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

                other = Notsafe.As<short, TOther>(int16);
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

                other = Notsafe.As<ushort, TOther>(uint16);
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

                other = Notsafe.As<int, TOther>(int32);
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

                other = Notsafe.As<uint, TOther>(uint32);
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

                other = Notsafe.As<long, TOther>(int64);
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

                other = Notsafe.As<ulong, TOther>(uint64);
                return true;
            }
            else if (otherType == typeof(nint))
            {
                nint nint;
                if (isChecked)
                {
                    try
                    {
                        nint = checked((nint)(long)rational.Numerator);
                    }
                    catch (OverflowException)
                    {
                        other = default;
                        return false;
                    }
                }
                else
                {
                    nint = unchecked((nint)(long)rational.Numerator);
                }

                other = Notsafe.As<nint, TOther>(nint);
                return true;
            }
            else if (otherType == typeof(nuint))
            {
                nuint nuint;
                if (isChecked)
                {
                    try
                    {
                        nuint = checked((nuint)(ulong)rational.Numerator);
                    }
                    catch (OverflowException)
                    {
                        other = default;
                        return false;
                    }
                }
                else
                {
                    nuint = unchecked((nuint)(ulong)rational.Numerator);
                }

                other = Notsafe.As<nuint, TOther>(nuint);
                return true;
            }
            else if (otherType == typeof(BigInteger))
            {
                other = Notsafe.As<BigInteger, TOther>(rational.Numerator);
                return true;
            }
        }

        // We do not know how to cast to this type
        Debugger.Break();
        other = default;
        return false;
    }

    #endregion

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
        Numerator = numerator;
        Denominator = denominator;
    }

    public void Deconstruct(out BigInteger numerator, out BigInteger denominator)
    {
        numerator = Numerator;
        denominator = Denominator;
    }

    #region Math

    public Rational Add(Rational other) => New(
            (Numerator * other.Denominator) + (Denominator * other.Numerator),
            Denominator * other.Denominator);

    public Rational Add(BigInteger bigInt) => New(
            Numerator + (Denominator * bigInt),
            Denominator);

    public Rational Subtract(Rational other) => New(
            (Numerator * other.Denominator) - (Denominator * other.Numerator),
            Denominator * other.Denominator);

    public Rational Subtract(BigInteger bigInt) => New(
            Numerator - (Denominator * bigInt),
            Denominator);

    public Rational Multiply(Rational other) => New(
        Numerator * other.Numerator,
        Denominator * other.Denominator);

    public Rational Multiply(BigInteger bigInt) => New(
            Numerator * bigInt,
            Denominator);
    
    public Rational Divide(Rational other) => New(
        Numerator * other.Denominator,
        Denominator * other.Numerator);

    public Rational Divide(BigInteger bigInt) => New(
        Numerator,
        Denominator * bigInt);
    
    public Rational Mod(Rational other)
    {
        BigInteger num = Numerator * other.Denominator;
        BigInteger den = Denominator * other.Numerator;
        BigInteger mod = num % den;
        return New(mod, den);
    }
    
    public Rational Mod(BigInteger bigInt)
    {
        BigInteger den = Denominator * bigInt;
        BigInteger mod = Numerator % den;
        return New(mod, den);
    }
    
    #endregion

    /// <summary>
    /// Get the Reciprocal of this <see cref="Rational"/>
    /// </summary>
    /// <seealso href="https://en.wikipedia.org/wiki/Multiplicative_inverse"/>
    public Rational Reciprocal() => new(Denominator, Numerator);
    
    public BigDecimal ToBigDecimal() => TryConvertToBigDecimal().OkOrThrow();

    public Result<BigDecimal, Exception> TryConvertToBigDecimal()
    {
        if (Denominator == BigInteger.Zero)
            return new DivideByZeroException("The denominator of this Rational is Zero");
        return (BigDecimal)Numerator / (BigDecimal)Denominator;
    }

    /// <summary>
    /// Convert this <see cref="Rational"/> to a <see cref="decimal"/>
    /// </summary>
    /// <exception cref="DivideByZeroException">Thrown if <see cref="Denominator"/> is <c>BigInteger.Zero</c></exception>
    public decimal ToDecimal() => TryConvertToDecimal().OkOrThrow();

    public Result<decimal, Exception> TryConvertToDecimal()
    {
        if (Denominator == BigInteger.Zero)
            return new DivideByZeroException("The denominator of this Rational is Zero");
        if (Numerator < MathHelper.BigInt.DecimalMinValue ||
            Numerator > MathHelper.BigInt.DecimalMaxValue)
        {
            return new InvalidOperationException($"Numerator '{Numerator}' is larger than a decimal");
        }
        if (Denominator > MathHelper.BigInt.DecimalMaxValue)
            return new InvalidOperationException($"Denominator '{Denominator}' is larger than decimal.MaxValue '{decimal.MaxValue}'");
        return (decimal)Numerator / (decimal)Denominator;
    }
    
    
    /// <summary>
    /// Convert this <see cref="Rational"/> to a <see cref="double"/>
    /// </summary>
    public double ToDouble() => TryConvertToDouble().OkOrThrow();
    
    public Result<double, Exception> TryConvertToDouble()
    {
        if (Denominator == BigInteger.Zero)
        {
            if (Numerator > BigInteger.Zero)
                return double.PositiveInfinity;
            if (Numerator < BigInteger.Zero)
                return double.NegativeInfinity;
            return double.NaN;
        }
        if (Numerator < MathHelper.BigInt.DoubleMinValue ||
            Numerator > MathHelper.BigInt.DoubleMaxValue)
        {
            return new InvalidOperationException($"Numerator '{Numerator}' is larger than a double");
        }
        if (Denominator > MathHelper.BigInt.DoubleMaxValue)
            return new InvalidOperationException($"Denominator '{Denominator}' is larger than double.MaxValue '{double.MaxValue}'");
        return (double)Numerator / (double)Denominator;
    }

    /// <summary>
    /// Convert this <see cref="Rational"/> to a <see cref="double"/>
    /// </summary>
    public float ToSingle() => TryConvertToFloat().OkOrThrow();
    
    public Result<float, Exception> TryConvertToFloat()
    {
        if (Denominator == BigInteger.Zero)
        {
            if (Numerator > BigInteger.Zero)
                return float.PositiveInfinity;
            if (Numerator < BigInteger.Zero)
                return float.NegativeInfinity;
            return float.NaN;
        }
        if (Numerator < MathHelper.BigInt.FloatMinValue ||
            Numerator > MathHelper.BigInt.FloatMaxValue)
        {
            return new InvalidOperationException($"Numerator '{Numerator}' is larger than a float");
        }
        if (Denominator > MathHelper.BigInt.FloatMaxValue)
            return new InvalidOperationException($"Denominator '{Denominator}' is larger than float.MaxValue '{float.MaxValue}'");
        return (float)Numerator / (float)Denominator;
    }

    public BigInteger ToBigInteger() => TryConvertToBigInteger().OkOrThrow();

    public Result<BigInteger, Exception> TryConvertToBigInteger()
    {
        if (Denominator == BigInteger.Zero)
            return new DivideByZeroException("The denominator of this Rational is Zero");
        return (BigInteger)Numerator / (BigInteger)Denominator;
    }

    public long ToInt64() => TryConvertToInt64().OkOrThrow();
    
    public Result<long, Exception> TryConvertToInt64()
    {
        if (Denominator == BigInteger.Zero)
            return new DivideByZeroException("The denominator of this Rational is Zero");
        if (Numerator < MathHelper.BigInt.LongMinValue ||
            Numerator > MathHelper.BigInt.LongMaxValue)
        {
            return new InvalidOperationException($"Numerator '{Numerator}' is larger than a long");
        }
        if (Denominator > MathHelper.BigInt.LongMaxValue)
            return new InvalidOperationException($"Denominator '{Denominator}' is larger than long.MaxValue '{long.MaxValue}'");
        return (long)Numerator / (long)Denominator;
    }
    
    public int CompareTo(Rational other)
    {
        // NaN sorts first
        int? fc = ComparePredicateResults(IsNaN, this, other);
        if (fc.TryGetValue(out int compare))
            return -compare; // negate, match sorts first

        // then Negative Infinity
        fc = ComparePredicateResults(IsNegativeInfinity, this, other);
        if (fc.TryGetValue(out compare))
            return -compare; // negate, match sorts first

        // positive infinity is always last
        fc = ComparePredicateResults(IsPositiveInfinity, this, other);
        if (fc.TryGetValue(out compare))
            return compare; // same, match sorts last

        // cross multiplication compare
        BigInteger left = Numerator * other.Denominator;
        BigInteger right = Denominator * other.Numerator;
        return left.CompareTo(right);
    }

    public int CompareTo(BigDecimal bigDec)
    {
        // Cannot be NaN or Infinity
        Rational other = FromBigDecimal(bigDec);
        // cross multiplication compare
        BigInteger left = Numerator * other.Denominator;
        BigInteger right = Denominator * other.Numerator;
        return left.CompareTo(right);
    }

    public int CompareTo(decimal dec)
    {
        // Cannot be NaN or Infinity
        Rational other = FromDecimal(dec);
        // cross multiplication compare
        BigInteger left = Numerator * other.Denominator;
        BigInteger right = Denominator * other.Numerator;
        return left.CompareTo(right);
    }

    public int CompareTo(double f64) => CompareTo(FromDouble(f64));

    public int CompareTo(BigInteger bigInt)
    {
        if (Denominator == BigInteger.Zero)
        {
            return Numerator >= 0 ? 1 : -1;
        }

        // cross multiplication
        return Numerator.CompareTo(Denominator * bigInt);
    }
    
    public int CompareTo(long i64)
    {
        if (Denominator == BigInteger.Zero)
        {
            return Numerator >= 0 ? 1 : -1;
        }

        // cross multiplication
        return Numerator.CompareTo(Denominator * i64);
    }
    

    public int CompareTo(object? obj)
    {
        return obj switch
        {
            Rational rational => CompareTo(rational),
            BigDecimal bigDec => CompareTo(bigDec),
            decimal dec => CompareTo(dec),
            double f64 => CompareTo(f64),
            BigInteger bigInt => CompareTo(bigInt),
            long i64 => CompareTo(i64),
            // We cannot compare to obj, sort it before
            _ => 1,
        };
    }

    public bool Equals(Rational other)
    {
        // cross multiplication
        BigInteger left = Numerator * other.Denominator;
        BigInteger right = Denominator * other.Numerator;
        return left.Equals(right);
    }

    public bool Equals(BigDecimal bigDec) => TryConvertToBigDecimal().Equals(bigDec);

    public bool Equals(decimal m) => TryConvertToDecimal().Equals(m);

    public bool Equals(double other) => Equals(other, double.Epsilon);
    
    public bool Equals(double other, double accuracy)
    {
        if (TryConvertToDouble().HasOk(out double f64))
        {
            bool? maybeEqual;
            bool equal;

            maybeEqual = MaybeEqual(double.IsNaN(f64), double.IsNaN(other));
            if (maybeEqual.TryGetValue(out equal))
                return equal;

            maybeEqual = MaybeEqual(double.IsNegativeInfinity(f64), double.IsNegativeInfinity(other));
            if (maybeEqual.TryGetValue(out equal))
                return equal;
        
            maybeEqual = MaybeEqual(double.IsPositiveInfinity(f64), double.IsPositiveInfinity(other));
            if (maybeEqual.TryGetValue(out equal))
                return equal;
        
            return Math.Abs(f64 - other) <= accuracy;
        }

        return false;
    }

    public bool Equals(float other, float accuracy = float.Epsilon)
        => Equals((double)other, (double)accuracy);

    public bool Equals(BigInteger other)
    {
        return IsFinite(this) &&
               // cross multiplication
               Numerator.Equals(Denominator * other);
    }

    public bool Equals(long other)
    {
        return IsFinite(this) &&
               // cross multiplication
               Numerator.Equals(Denominator * other);
    }


    public override bool Equals(object? obj)
        => obj switch
        {
            Rational rational => Equals(rational),
            BigDecimal bigDec => Equals(bigDec),
            decimal m => Equals(m),
            double d => Equals(d),
            BigInteger iBig => Equals(iBig),
            long i64 => Equals(i64),
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

    public override string ToString() => $"{Numerator}/{Denominator}";
}
