#pragma warning disable IDE0004, CA2225, IDE0002, IDE0090, IDE0018, IDE0047
// ReSharper disable RedundantOverflowCheckingContext
// ReSharper disable ArrangeThisQualifier

using System.Globalization;
using ScrubJay.Parsing;



// https://github.com/danm-de/Fractions

/* The order when referencing other types (equality, comparison, etc):
 * Rational
 * decimal
 * double
 * float (only for helpful rounding circumstances)
 * BigInteger
 * long
 */

namespace ScrubJay.Maths;

/// <summary>
/// A Rational Number
/// </summary>
/// <seealso href="https://en.wikipedia.org/wiki/Rational_number"/>
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
    ITrySpanParsable<Rational>,
    IAlgebraic<Rational, Rational>,
    IAlgebraic<Rational, decimal>,
    IAlgebraic<Rational, double>,
    IAlgebraic<Rational, BigInteger>,
    IAlgebraic<Rational, long>,
#if NET6_0_OR_GREATER
    ISpanFormattable,
#endif
    IFormattable,
    IRenderable
{
#region Operators

    // implicit operations will never lose precision nor fail

    public static implicit operator Rational(decimal dec) => FromDecimal(dec);
    public static implicit operator Rational(double f64) => FromDouble(f64);
    public static implicit operator Rational(BigInteger bigInt) => new(bigInt, BigInteger.One);
    public static implicit operator Rational(long i64) => new(new BigInteger(i64), BigInteger.One);

    // explicit operations can lose precision and can fail

    public static explicit operator decimal(Rational rational) => rational.ToDecimal();
    public static explicit operator double(Rational rational) => rational.ToDouble();
    public static explicit operator BigInteger(Rational rational) => rational.ToBigInteger();
    public static explicit operator long(Rational rational) => rational.ToInt64();

    public static Rational operator -(Rational value) => new(-value.Numerator, value.Denominator);
    public static Rational operator +(Rational value) => value; // no-op

    public static Rational operator --(Rational value) => value.Subtract(One);
    public static Rational operator ++(Rational value) => value.Add(One);

    public static bool operator ==(Rational left, Rational right) => left.Equals(right);
    public static bool operator ==(Rational left, decimal right) => left.Equals(right);
    public static bool operator ==(Rational left, double right) => left.Equals(right);
    public static bool operator ==(Rational left, BigInteger right) => left.Equals(right);
    public static bool operator ==(Rational left, long right) => left.Equals(right);

    public static bool operator !=(Rational left, Rational right) => !left.Equals(right);
    public static bool operator !=(Rational left, decimal right) => !left.Equals(right);
    public static bool operator !=(Rational left, double right) => !left.Equals(right);
    public static bool operator !=(Rational left, BigInteger right) => !left.Equals(right);
    public static bool operator !=(Rational left, long right) => !left.Equals(right);

    public static bool operator >(Rational left, Rational right) => left.CompareTo(right) > 0;
    public static bool operator >(Rational left, decimal right) => left.CompareTo(right) > 0;
    public static bool operator >(Rational left, double right) => left.CompareTo(right) > 0;
    public static bool operator >(Rational left, BigInteger right) => left.CompareTo(right) > 0;
    public static bool operator >(Rational left, long right) => left.CompareTo(right) > 0;

    public static bool operator >=(Rational left, Rational right) => left.CompareTo(right) >= 0;
    public static bool operator >=(Rational left, decimal right) => left.CompareTo(right) >= 0;
    public static bool operator >=(Rational left, double right) => left.CompareTo(right) >= 0;
    public static bool operator >=(Rational left, BigInteger right) => left.CompareTo(right) >= 0;
    public static bool operator >=(Rational left, long right) => left.CompareTo(right) >= 0;

    public static bool operator <(Rational left, Rational right) => left.CompareTo(right) < 0;
    public static bool operator <(Rational left, decimal right) => left.CompareTo(right) < 0;
    public static bool operator <(Rational left, double right) => left.CompareTo(right) < 0;
    public static bool operator <(Rational left, BigInteger right) => left.CompareTo(right) < 0;
    public static bool operator <(Rational left, long right) => left.CompareTo(right) < 0;

    public static bool operator <=(Rational left, Rational right) => left.CompareTo(right) <= 0;
    public static bool operator <=(Rational left, decimal right) => left.CompareTo(right) <= 0;
    public static bool operator <=(Rational left, double right) => left.CompareTo(right) <= 0;
    public static bool operator <=(Rational left, BigInteger right) => left.CompareTo(right) <= 0;
    public static bool operator <=(Rational left, long right) => left.CompareTo(right) <= 0;

    public static Rational operator +(Rational left, Rational right) => left.Add(right);
    public static Rational operator +(Rational left, decimal right) => left.Add(FromDecimal(right));
    public static Rational operator +(Rational left, double right) => left.Add(FromDouble(right));
    public static Rational operator +(Rational left, BigInteger right) => left.Add(right);
    public static Rational operator +(Rational left, long right) => left.Add(right);

    public static Rational operator -(Rational left, Rational right) => left.Subtract(right);
    public static Rational operator -(Rational left, decimal right) => left.Subtract(FromDecimal(right));
    public static Rational operator -(Rational left, double right) => left.Subtract(FromDouble(right));
    public static Rational operator -(Rational left, BigInteger right) => left.Subtract(right);
    public static Rational operator -(Rational left, long right) => left.Subtract(right);

    public static Rational operator *(Rational left, Rational right) => left.Multiply(right);
    public static Rational operator *(Rational left, decimal right) => left.Multiply(FromDecimal(right));
    public static Rational operator *(Rational left, double right) => left.Multiply(FromDouble(right));
    public static Rational operator *(Rational left, BigInteger right) => left.Multiply(right);
    public static Rational operator *(Rational left, long right) => left.Multiply(right);

    public static Rational operator /(Rational left, Rational right) => left.Divide(right);
    public static Rational operator /(Rational left, decimal right) => left.Divide(FromDecimal(right));
    public static Rational operator /(Rational left, double right) => left.Divide(FromDouble(right));
    public static Rational operator /(Rational left, BigInteger right) => left.Divide(right);
    public static Rational operator /(Rational left, long right) => left.Divide(right);

    public static Rational operator %(Rational left, Rational right) => left.Mod(right);
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

    /// <summary>
    /// A Rational is stored in Base 10
    /// </summary>
    public static int Radix => 10;

#endregion

#region Static Methods

    private static bool? MaybeEqual(bool leftResult, bool rightResult)
    {
        if (leftResult != rightResult)
            return false; // different
        if (leftResult)
            return true; // both are true
        return null; // both are false
    }

    private static bool? MaybeEqual<I>(Func<I, bool> predicate, I leftInstance, I rightInstance)
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

    private static int? ComparePredicateResults<I>(Func<I, bool> predicate, I leftInstance,
        I rightInstance)
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


    public static Rational FromDouble(double value,
        double? minTolerance = null,
        long maxDenominator = 1_000_000_000_000)
    {
        // special cases
        if (double.IsNaN(value))
            return NaN;
        if (double.IsPositiveInfinity(value))
            return PositiveInfinity;
        if (double.IsNegativeInfinity(value))
            return NegativeInfinity;
        Debug.Assert(!double.IsInfinity(value));
        if (value == 0.0d)
            return Zero;

        // deal with negative values
        bool isNegative = value < 0;
        if (isNegative)
            value = -value;

        // resolve tolerance with more wiggle room for larger values
        double tolerance = minTolerance ?? Math.Max(double.Epsilon * value, float.Epsilon);

        // anything below tolerance is zero
        if (value <= tolerance)
            return Zero;

        // close enough to a number?
        double integerPart = Math.Truncate(value);
        double fractionalPart = value - integerPart;
        if (value - integerPart <= tolerance)
            return new((BigInteger)(isNegative ? -integerPart : integerPart), BigInteger.One);


        // continued fractions algorithm  (https://en.wikipedia.org/wiki/Continued_fraction)
        // n(-1)/d(-1) = 1/0    (but shift left one before starting)
        BigInteger numMinus2 = BigInteger.One;
        BigInteger denomMinus2 = BigInteger.Zero;
        // n(0)/d(0) = intpart/1    (shift left)
        BigInteger numMinus1 = (BigInteger)value;
        BigInteger denomMinus1 = BigInteger.One;


        // only need fractional part for algo
        value = fractionalPart;

        // first convergent = just integer part
        Rational best = new Rational(isNegative ? -numMinus1 : numMinus1, denomMinus1);

        // continue while fractional part is significant
        while (value > tolerance)
        {
            // reciprocal
            value = 1.0d / value;

            // next int part
            BigInteger intPart = (BigInteger)value;

            // next convergent
            // n(0) = intPart * n(-1) + (n-2)
            // d(0) = intPart * d(-1) + (d-2)
            BigInteger num = (intPart * numMinus1) + numMinus2;
            BigInteger denom = (intPart * denomMinus1) + denomMinus2;

            // have we exceeded our max?
            if (denom > maxDenominator)
                break; // our best stands

            // this is the new best rational
            best = new Rational(isNegative ? -num : num, denom);

            // accurate enough?
            if (Math.Abs(best.ToDouble() - value) <= tolerance)
                return best;

            // update to fractional part
            value -= Math.Truncate(value);

            // shift convergents
            // n(-2) <- n(-1) <- n(0)
            // p(-2) <- p(-1) <- p(0)
            numMinus2 = numMinus1;
            numMinus1 = num;
            denomMinus2 = denomMinus1;
            denomMinus1 = denom;
        }

        // found the best approximation we can
        return best;
    }

    public static Rational FromF32(float f32, float minTolerance = float.Epsilon)
        => FromDouble(f32, minTolerance);

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

        Rational rational = new(numerator, denominator);
        return rational;
    }

    public static Rational FromDecimal(decimal dec)
    {
        int digitCount = dec.TrailingDigitCount();
        if (digitCount <= 18)
        {
            long denominator = (long)Math.Pow(10d, digitCount);
            var numerator = new BigInteger(dec * denominator);
            Rational rational = new(numerator, denominator);
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
        { '0', Zero },
        { '1', One },
        { '2', new(2, 1) },
        { '3', new(3, 1) },
        { '4', new(4, 1) },
        { '5', new(5, 1) },
        { '6', new(6, 1) },
        { '7', new(7, 1) },
        { '8', new(8, 1) },
        { '9', new(9, 1) },
        { '¼', new(1, 4) },
        { '½', new(1, 2) },
        { '¾', new(3, 4) },
        { '⅐', new(1, 7) },
        { '⅑', new(1, 9) },
        { '⅒', new(1, 10) },
        { '⅓', new(1, 3) },
        { '⅔', new(2, 3) },
        { '⅕', new(1, 5) },
        { '⅖', new(2, 5) },
        { '⅗', new(3, 5) },
        { '⅘', new(4, 5) },
        { '⅙', new(1, 6) },
        { '⅚', new(5, 6) },
        { '⅛', new(1, 8) },
        { '⅜', new(3, 8) },
        { '⅝', new(5, 8) },
        { '⅞', new(7, 8) },
        { '↉', new(0, 3) },
    };

    public static Result<Rational> TryParse(scoped text text,
        NumberStyles numberStyle = NumberStyles.Integer,
        IFormatProvider? provider = null
    )
    {
        // we can directly parse a few single characters
        if (text.Length == 1)
        {
            char ch = text[0];
            if (_fastUnicodeRationals.TryGetValue(ch, out var rational))
                return Ok(rational);
            // will not parse
            return Ex.Parse<Rational>(text);
        }

        // expect {numerator}/{denominator}
        var reader = new SpanReader<char>(text);

        // skip all leading whitespace
        reader.SkipWhile(char.IsWhiteSpace);
        if (reader.IsCompleted)
            return Ex.Parse<Rational>(text);

        // everything until / or whitespace is the numerator
        var numText = reader.TakeUntil(ch => char.IsWhiteSpace(ch) || ch == '/');
        if (reader.IsCompleted)
            return Ex.Parse<Rational>(text);

        // must be able to convert
#if NET7_0_OR_GREATER
        if (!BigInteger.TryParse(numText, numberStyle, provider, out var numerator))
            return Ex.Parse<Rational>(text, "Invalid numerator");
#else
        if (!BigInteger.TryParse(numText.AsString(), numberStyle, provider, out var numerator))
            return Ex.Parse<Rational>(text, "Invalid numerator");
#endif

        // skip whitespace
        reader.SkipWhile(char.IsWhiteSpace);
        if (reader.IsCompleted)
            return Ex.Parse<Rational>(text);

        // must be a slash
        if (reader.Take() != '/')
            return Ex.Parse<Rational>(text);

        // skip whitespace
        reader.SkipWhile(char.IsWhiteSpace);
        if (reader.IsCompleted)
            return Ex.Parse<Rational>(text);

        // everything until whitespace or end is the denominator
        var denomText = reader.TakeUntil(char.IsWhiteSpace);
        if (!reader.IsCompleted)
        {
            // skip whitespace
            reader.SkipWhile(char.IsWhiteSpace);
            if (!reader.IsCompleted)
                return Ex.Parse<Rational>(text);
        }

        // must be able to convert
#if NET7_0_OR_GREATER
        if (!BigInteger.TryParse(denomText, numberStyle, provider, out var denominator))
            return Ex.Parse<Rational>(text, "Invalid denominator");
#else
        if (!BigInteger.TryParse(denomText.AsString(), numberStyle, provider, out var denominator))
            return Ex.Parse<Rational>(text, "Invalid denominator");
#endif
        return Ok<Rational>(new(numerator, denominator));
    }

#if NET7_0_OR_GREATER
    static Rational IParsable<Rational>.Parse(string str, IFormatProvider? provider)
    {
        Throw.IfNull(str);
        return TryParse(str.AsSpan(), provider: provider)
            .OkOrThrow();
    }

    static Rational INumberBase<Rational>.Parse(string str, NumberStyles style, IFormatProvider? provider)
    {
        Throw.IfNull(str);
        return TryParse(str.AsSpan(), style, provider)
            .OkOrThrow();
    }

    static Rational ISpanParsable<Rational>.Parse(scoped text text, IFormatProvider? provider)
    {
        return TryParse(text, provider: provider).OkOrThrow();
    }

    static Rational INumberBase<Rational>.Parse(scoped text text, NumberStyles style, IFormatProvider? provider)
    {
        return TryParse(text, style, provider).OkOrThrow();
    }


    static bool IParsable<Rational>.TryParse([NotNullWhen(true)] string? str, IFormatProvider? provider, out Rational rational)
    {
        return TryParse(str.AsSpan(), provider: provider)
            .IsOk(out rational);
    }

    static bool INumberBase<Rational>.TryParse([NotNullWhen(true)] string? str, NumberStyles style, IFormatProvider? provider,
        out Rational rational)
    {
        return TryParse(str.AsSpan(), style, provider)
            .IsOk(out rational);
    }

    static bool ISpanParsable<Rational>.TryParse(scoped text text, IFormatProvider? provider, out Rational rational)
    {
        return TryParse(text, provider: provider)
            .IsOk(out rational);
    }

    static bool INumberBase<Rational>.TryParse(scoped text text, NumberStyles style, IFormatProvider? provider,
        out Rational rational)
    {
        return TryParse(text, style, provider)
            .IsOk(out rational);
    }

#endif

    public static Result<Rational> TryParse(text text, IFormatProvider? provider)
    {
        return TryParse(text, NumberStyles.Integer, provider);
    }

#endregion

#region IsXYZ

    public static bool IsCanonical(Rational value) => IsInteger(value);

    public static bool IsComplexNumber(Rational _) => false;

    public static bool IsEvenInteger(Rational value) => IsInteger(value) && ((value.Numerator % 2) == 0);

    public static bool IsFinite(Rational value) => value.Denominator != BigInteger.Zero;

    public static bool IsImaginaryNumber(Rational _) => false;

    public static bool IsInfinity(Rational value) => value.Denominator == BigInteger.Zero;

    public static bool IsInteger(Rational value) => value.Denominator == BigInteger.One;

    public static bool IsNaN(Rational value) => (value.Denominator == BigInteger.Zero) && (value.Numerator == BigInteger.Zero);

    public static bool IsNegative(Rational value) => value.Numerator < BigInteger.Zero;

    public static bool IsNegativeInfinity(Rational value) =>
        (value.Denominator == BigInteger.Zero) && (value.Numerator < BigInteger.Zero);

    public static bool IsNormal(Rational value) => value != Zero;

    public static bool IsOddInteger(Rational value) => IsInteger(value) && ((value.Numerator % 2) != 0);

    public static bool IsPositive(Rational value) => value.Numerator >= BigInteger.Zero;

    public static bool IsPositiveInfinity(Rational value) =>
        (value.Denominator == BigInteger.Zero) && (value.Numerator > BigInteger.Zero);

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
    static bool INumberBase<Rational>.TryConvertFromChecked<T>(T value, out Rational rational) =>
        throw new NotImplementedException();

    static bool INumberBase<Rational>.TryConvertFromSaturating<T>(T value, out Rational rational) =>
        TryConvertFrom<T>(value, out rational);

    static bool INumberBase<Rational>.TryConvertFromTruncating<T>(T value, out Rational rational) =>
        TryConvertFrom<T>(value, out rational);
#endif

    public static bool TryConvertFrom<T>(T value, out Rational rational)
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
                rational = FromF32(f32);
                return true;
            case double f64:
                rational = FromDouble(f64);
                return true;
            case decimal dec:
                rational = FromDecimal(dec);
                return true;
            case string str:
                return TryParse(str.AsSpan()).IsOk(out rational);
            case Rational fraction:
                rational = fraction;
                return true;
            default:
                rational = default;
                return false;
        }
    }

#if NET7_0_OR_GREATER
    static bool INumberBase<Rational>.TryConvertToChecked<T>(Rational value, [MaybeNullWhen(false)] out T result) =>
        TryConvertTo<T>(value, out result, true);

    static bool INumberBase<Rational>.TryConvertToSaturating<T>(Rational value, [MaybeNullWhen(false)] out T result) =>
        TryConvertTo<T>(value, out result);

    static bool INumberBase<Rational>.TryConvertToTruncating<T>(Rational value, [MaybeNullWhen(false)] out T result) =>
        TryConvertTo<T>(value, out result);
#endif

#pragma warning disable CA1502
    public static bool TryConvertTo<T>(Rational rational, [MaybeNullWhen(false)] out T other,
        bool isChecked = false)
    {
        var otherType = typeof(T);

        if (otherType == typeof(float))
        {
            return rational
                .TryConvertToFloat()
                .Select(Notsafe.As<float, T>)
                .IsOk(out other);
        }
        else if (otherType == typeof(double))
        {
            return rational
                .TryConvertToDouble()
                .Select(Notsafe.As<double, T>)
                .IsOk(out other);
        }
        else if (otherType == typeof(decimal))
        {
            return rational
                .TryConvertToDecimal()
                .Select(Notsafe.As<decimal, T>)
                .IsOk(out other);
        }
        else if (otherType == typeof(string))
        {
            string str = rational.ToString();
            other = Notsafe.As<string, T>(str);
            return true;
        }
        else if (otherType == typeof(Rational))
        {
            other = Notsafe.As<Rational, T>(rational);
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

                other = Notsafe.As<byte, T>(uint8);
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

                other = Notsafe.As<sbyte, T>(int8);
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

                other = Notsafe.As<short, T>(int16);
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

                other = Notsafe.As<ushort, T>(uint16);
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

                other = Notsafe.As<int, T>(int32);
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

                other = Notsafe.As<uint, T>(uint32);
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

                other = Notsafe.As<long, T>(int64);
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

                other = Notsafe.As<ulong, T>(uint64);
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

                other = Notsafe.As<nint, T>(nint);
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

                other = Notsafe.As<nuint, T>(nuint);
                return true;
            }
            else if (otherType == typeof(BigInteger))
            {
                other = Notsafe.As<BigInteger, T>(rational.Numerator);
                return true;
            }
        }

        // We do not know how to cast to this type
        Debugger.Break();
        other = default;
        return false;
    }

#endregion

#endregion

    /// <summary>
    /// This <see cref="Rational"/>'s Numerator
    /// </summary>
    public readonly BigInteger Numerator;

    /// <summary>
    /// This <see cref="Rational"/>'s Denominator
    /// </summary>
    public readonly BigInteger Denominator;


    public Rational(BigInteger numerator, BigInteger denominator)
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

    public Rational Add(Rational other) => new(
        (Numerator * other.Denominator) + (Denominator * other.Numerator),
        Denominator * other.Denominator);

    public Rational Add(BigInteger bigInt) => new(
        Numerator + (Denominator * bigInt),
        Denominator);

    public Rational Subtract(Rational other) => new(
        (Numerator * other.Denominator) - (Denominator * other.Numerator),
        Denominator * other.Denominator);

    public Rational Subtract(BigInteger bigInt) => new(
        Numerator - (Denominator * bigInt),
        Denominator);

    public Rational Multiply(Rational other) => new(
        Numerator * other.Numerator,
        Denominator * other.Denominator);

    public Rational Multiply(BigInteger bigInt) => new(
        Numerator * bigInt,
        Denominator);

    public Rational Divide(Rational other) => new(
        Numerator * other.Denominator,
        Denominator * other.Numerator);

    public Rational Divide(BigInteger bigInt) => new(
        Numerator,
        Denominator * bigInt);

    public Rational Mod(Rational other)
    {
        BigInteger num = Numerator * other.Denominator;
        BigInteger den = Denominator * other.Numerator;
        BigInteger mod = num % den;
        return new(mod, den);
    }

    public Rational Mod(BigInteger bigInt)
    {
        BigInteger den = Denominator * bigInt;
        BigInteger mod = Numerator % den;
        return new(mod, den);
    }

#endregion

    /// <summary>
    /// Get the Reciprocal of this <see cref="Rational"/>
    /// </summary>
    /// <seealso href="https://en.wikipedia.org/wiki/Multiplicative_inverse"/>
    public Rational Reciprocal() => new(Denominator, Numerator);


    /// <summary>
    /// Get the simplified version of this <see cref="Rational"/>
    /// </summary>
    /// <returns></returns>
    public Rational Simplify()
    {
        var (num, denom) = this;

        // x/0 == (+∞ | -∞ | NaN)
        if (denom == BigInteger.Zero)
        {
            if (num > BigInteger.Zero)
                return PositiveInfinity;
            if (num < BigInteger.Zero)
                return NegativeInfinity;
            // 0/0 == Not A Number
            return NaN;
        }

        // 0/x == 0
        if (num == BigInteger.Zero)
            return Zero;

        // x/x == 1
        if (num == denom)
            return One;

        // if the denominator is negative, flip both signs
        // x/-y -> -x/y
        // -x/-y -> x/y
        if (denom < BigInteger.Zero)
        {
            num = -num;
            denom = -denom;
        }

        Debug.Assert(denom > BigInteger.Zero);

        // If the numerator or denominator are already 1/-1, cannot simplify further
        if (num == BigInteger.One || num == BigInteger.MinusOne || denom == BigInteger.One)
            return new(num, denom);

        var gcd = BigInteger.GreatestCommonDivisor(num, denom);
        // if gcd is 1 or -1, cannot simplify
        if (gcd == BigInteger.One || gcd == BigInteger.MinusOne)
        {
            if (gcd == BigInteger.MinusOne)
                Debugger.Break();
            return new(num, denom);
        }

        num /= gcd;
        denom /= gcd;
        return new(num, denom);
    }


    /// <summary>
    /// Convert this <see cref="Rational"/> to a <see cref="decimal"/>
    /// </summary>
    /// <exception cref="DivideByZeroException">Thrown if <see cref="Denominator"/> is <c>BigInteger.Zero</c></exception>
    public decimal ToDecimal() => TryConvertToDecimal().OkOrThrow();

    public Result<decimal> TryConvertToDecimal()
    {
        if (Denominator == BigInteger.Zero)
            return new DivideByZeroException("The denominator of this Rational is Zero");
        if ((Numerator < MathHelper.BigInt.DecimalMinValue) ||
            (Numerator > MathHelper.BigInt.DecimalMaxValue))
        {
            return new InvalidOperationException($"Numerator '{Numerator}' is outside the bounds of a decimal");
        }

        if ((Denominator < MathHelper.BigInt.DecimalMinValue) ||
            (Denominator > MathHelper.BigInt.DecimalMaxValue))
        {
            return new InvalidOperationException($"Denominator '{Denominator}' is outside the bounds of a decimal");
        }

        return Ok((decimal)Numerator / (decimal)Denominator);
    }


    /// <summary>
    /// Convert this <see cref="Rational"/> to a <see cref="double"/>
    /// </summary>
    public double ToDouble() => TryConvertToDouble().OkOrThrow();

    public Result<double> TryConvertToDouble()
    {
        if (Denominator == BigInteger.Zero)
        {
            if (Numerator > BigInteger.Zero)
                return Ok(double.PositiveInfinity);
            if (Numerator < BigInteger.Zero)
                return Ok(double.NegativeInfinity);
            return Ok(double.NaN);
        }

        if ((Numerator < MathHelper.BigInt.DoubleMinValue) ||
            (Numerator > MathHelper.BigInt.DoubleMaxValue))
        {
            return new InvalidOperationException(
                $"Numerator '{Numerator}' is outside the bounds of a double");
        }

        if ((Denominator > MathHelper.BigInt.DoubleMaxValue) ||
            (Denominator < MathHelper.BigInt.DoubleMinValue))
        {
            return new InvalidOperationException(
                $"Denominator '{Denominator}' is outside the bounds of a double");
        }

        return Ok((double)Numerator / (double)Denominator);
    }

    /// <summary>
    /// Convert this <see cref="Rational"/> to a <see cref="double"/>
    /// </summary>
    public float ToSingle() => TryConvertToFloat().OkOrThrow();

    public Result<float> TryConvertToFloat()
    {
        if (Denominator == BigInteger.Zero)
        {
            if (Numerator > BigInteger.Zero)
                return Ok(float.PositiveInfinity);
            if (Numerator < BigInteger.Zero)
                return Ok(float.NegativeInfinity);
            return Ok(float.NaN);
        }

        if ((Numerator < MathHelper.BigInt.FloatMinValue) ||
            (Numerator > MathHelper.BigInt.FloatMaxValue))
        {
            return new InvalidOperationException(
                $"Numerator '{Numerator}' is outside the bounds of a float");
        }

        if ((Denominator > MathHelper.BigInt.FloatMaxValue) ||
            (Denominator < MathHelper.BigInt.FloatMinValue))
        {
            return new InvalidOperationException(
                $"Denominator '{Denominator}' is outside the bounds of a float");
        }

        return Ok((float)Numerator / (float)Denominator);
    }

    public BigInteger ToBigInteger() => TryConvertToBigInteger().OkOrThrow();

    public Result<BigInteger> TryConvertToBigInteger()
    {
        if (Denominator == BigInteger.Zero)
            return new DivideByZeroException("The denominator of this Rational is Zero");
        return Ok((BigInteger)Numerator / (BigInteger)Denominator);
    }

    public long ToInt64() => TryConvertToInt64().OkOrThrow();

    public Result<long> TryConvertToInt64()
    {
        if (Denominator == BigInteger.Zero)
            return new DivideByZeroException("The denominator of this Rational is Zero");
        if ((Numerator < MathHelper.BigInt.LongMinValue) ||
            (Numerator > MathHelper.BigInt.LongMaxValue))
        {
            return new InvalidOperationException(
                $"Numerator '{Numerator}' is outside the bounds of a long");
        }

        if ((Denominator > MathHelper.BigInt.LongMaxValue) ||
            (Denominator < MathHelper.BigInt.LongMinValue))
        {
            return new InvalidOperationException(
                $"Denominator '{Denominator}' is outside the bounds of a long");
        }

        return Ok((long)Numerator / (long)Denominator);
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

    public int CompareTo(decimal dec) => CompareTo(FromDecimal(dec));

    public int CompareTo(double f64) => CompareTo(FromDouble(f64));

    public int CompareTo(float f32) => CompareTo(FromF32(f32));

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
            decimal dec => CompareTo(dec),
            double f64 => CompareTo(f64),
            float f32 => CompareTo(f32),
            BigInteger bigInt => CompareTo(bigInt),
            long i64 => CompareTo(i64),
            // We cannot compare to obj, sort it before
            _ => 1,
        };
    }

    public bool Equals(Rational other)
    {
        // cross multiplication solved non-simplified compares
        BigInteger left = Numerator * other.Denominator;
        BigInteger right = Denominator * other.Numerator;
        return left.Equals(right);
    }

    public bool Equals(decimal m) => TryConvertToDecimal().Equals(m);

    public bool Equals(double other) => Equals(other, double.Epsilon);

    public bool Equals(double other, double accuracy)
    {
        if (TryConvertToDouble().IsOk(out double f64))
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
            decimal m => Equals(m),
            double d => Equals(d),
            BigInteger iBig => Equals(iBig),
            long i64 => Equals(i64),
            _ => false,
        };

    public override int GetHashCode() => Hasher.HashMany(Numerator, Denominator);

#region Formatting + Stringification

    private void FormatTo(TextBuilder builder, string? format, IFormatProvider? provider)
    {
        if (string.IsNullOrEmpty(format))
        {
            RenderTo(builder);
            return;
        }

        int firstChar = format![0];
        text fmt = format.AsSpan(1);
        if (firstChar is ('g' or 'G'))
        {
            builder.Append(Numerator, fmt, provider)
                .Append('/')
                .Append(Denominator, fmt, provider);
        }
        else if (firstChar == 'M') // Mixed number
        {
            BigInteger numerator = Numerator;
            BigInteger denominator = Denominator;
            if ((BigInteger.Abs(numerator) < BigInteger.Abs(denominator)) || (denominator == BigInteger.Zero))
            {
                RenderTo(builder);
                return;
            }

            BigInteger integralPart = numerator / denominator;
            var remainder = this - integralPart;
            builder.Append(integralPart, fmt, provider);
            if (remainder != Zero)
            {
                builder.Append(' ').Invoke(remainder.RenderTo);
            }
        }
        else if (firstChar == 'N') // Numerator only
        {
            builder.Append(Numerator, fmt, provider);
        }
        else if (firstChar == 'D') // Denominator only
        {
            builder.Append(Denominator, fmt, provider);
        }
        else if (firstChar == 'd')
        {
            builder.Append(ToDouble(), fmt, provider);
        }
        else if (firstChar == 'm')
        {
            builder.Append(ToDecimal(), fmt, provider);
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(format));
        }
    }

    public void RenderTo(TextBuilder builder)
    {
        builder.Append(Numerator).Append('/').Append(Denominator);
    }

    public bool TryFormat(
        Span<char> destination,
        out int charsWritten,
        text format = default,
        IFormatProvider? provider = null)
    {
        using var builder = new TextBuilder();
        FormatTo(builder, format.AsString(), provider);
        if (builder.Length > destination.Length)
        {
            charsWritten = 0;
            return false;
        }

        charsWritten = builder.Length;
        TextHelper.Notsafe.CopyBlock(builder.AsText(), destination, charsWritten);
        return true;
    }

    public string ToString(string? format, IFormatProvider? provider = null)
    {
        using var builder = new TextBuilder();
        FormatTo(builder, format, provider);
        return builder.ToString();
    }

    public override string ToString() => this.Render();

#endregion
}