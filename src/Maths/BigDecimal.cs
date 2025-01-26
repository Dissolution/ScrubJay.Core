using ScrubJay.Parsing;
using ScrubJay.Text;
using System.Globalization;

// https://gist.github.com/JcBernack/0b4eef59ca97ee931a2f45542b9ff06d

namespace ScrubJay.Maths;

public readonly struct BigDecimal :
#if NET7_0_OR_GREATER
    IFloatingPoint<BigDecimal>,
    IParsable<BigDecimal>, 
    ISpanParsable<BigDecimal>,
#endif
    IAlgebraic<BigDecimal, BigDecimal>,
    IEquatable<BigDecimal>,
    IComparable<BigDecimal>,
    IFormattable, ISpanFormattable
{
    #region Static
    #region Operators
    public static implicit operator BigDecimal(decimal dec) => TryConvertFrom<decimal>(dec).OkOrThrow();
    public static implicit operator BigDecimal(double f64) => TryConvertFrom<double>(f64).OkOrThrow();
    public static implicit operator BigDecimal(BigInteger bigInt) => TryConvertFrom<BigInteger>(bigInt).OkOrThrow();
    public static implicit operator BigDecimal(long i64) => TryConvertFrom<long>(i64).OkOrThrow();

    public static BigDecimal operator -(BigDecimal value) => value.Negate();
    public static BigDecimal operator +(BigDecimal value) => value.Plus();
    public static BigDecimal operator ++(BigDecimal value) => value.Increment();
    public static BigDecimal operator --(BigDecimal value) => value.Decrement();

    public static bool operator ==(BigDecimal left, BigDecimal right) => left.Equals(right);
    public static bool operator ==(BigDecimal left, Rational right) => left.Equals(right);
    public static bool operator ==(BigDecimal left, decimal right) => left.Equals(right);
    public static bool operator ==(BigDecimal left, double right) => left.Equals(right);
    public static bool operator ==(BigDecimal left, BigInteger right) => left.Equals(right);
    public static bool operator ==(BigDecimal left, long right) => left.Equals(right);

    public static bool operator !=(BigDecimal left, BigDecimal right) => !left.Equals(right);
    public static bool operator !=(BigDecimal left, Rational right) => !left.Equals(right);
    public static bool operator !=(BigDecimal left, decimal right) => !left.Equals(right);
    public static bool operator !=(BigDecimal left, double right) => !left.Equals(right);
    public static bool operator !=(BigDecimal left, BigInteger right) => !left.Equals(right);
    public static bool operator !=(BigDecimal left, long right) => !left.Equals(right);

    public static bool operator <(BigDecimal left, BigDecimal right) => left.CompareTo(right) < 0;
    public static bool operator <(BigDecimal left, Rational right) => left.CompareTo(right) < 0;
    public static bool operator <(BigDecimal left, decimal right) => left.CompareTo(right) < 0;
    public static bool operator <(BigDecimal left, double right) => left.CompareTo(right) < 0;
    public static bool operator <(BigDecimal left, BigInteger right) => left.CompareTo(right) < 0;
    public static bool operator <(BigDecimal left, long right) => left.CompareTo(right) < 0;

    public static bool operator <=(BigDecimal left, BigDecimal right) => left.CompareTo(right) <= 0;
    public static bool operator <=(BigDecimal left, Rational right) => left.CompareTo(right) <= 0;
    public static bool operator <=(BigDecimal left, decimal right) => left.CompareTo(right) <= 0;
    public static bool operator <=(BigDecimal left, double right) => left.CompareTo(right) <= 0;
    public static bool operator <=(BigDecimal left, BigInteger right) => left.CompareTo(right) <= 0;
    public static bool operator <=(BigDecimal left, long right) => left.CompareTo(right) <= 0;

    public static bool operator >(BigDecimal left, BigDecimal right) => left.CompareTo(right) > 0;
    public static bool operator >(BigDecimal left, Rational right) => left.CompareTo(right) > 0;
    public static bool operator >(BigDecimal left, decimal right) => left.CompareTo(right) > 0;
    public static bool operator >(BigDecimal left, double right) => left.CompareTo(right) > 0;
    public static bool operator >(BigDecimal left, BigInteger right) => left.CompareTo(right) > 0;
    public static bool operator >(BigDecimal left, long right) => left.CompareTo(right) > 0;

    public static bool operator >=(BigDecimal left, BigDecimal right) => left.CompareTo(right) >= 0;
    public static bool operator >=(BigDecimal left, Rational right) => left.CompareTo(right) >= 0;
    public static bool operator >=(BigDecimal left, decimal right) => left.CompareTo(right) >= 0;
    public static bool operator >=(BigDecimal left, double right) => left.CompareTo(right) >= 0;
    public static bool operator >=(BigDecimal left, BigInteger right) => left.CompareTo(right) >= 0;
    public static bool operator >=(BigDecimal left, long right) => left.CompareTo(right) >= 0;

    public static BigDecimal operator +(BigDecimal left, BigDecimal right) => left.Add(right);
    public static BigDecimal operator +(BigDecimal left, Rational right) => left.Add(right);
    public static BigDecimal operator +(BigDecimal left, decimal right) => left.Add(right);
    public static BigDecimal operator +(BigDecimal left, double right) => left.Add(right);
    public static BigDecimal operator +(BigDecimal left, BigInteger right) => left.Add(right);
    public static BigDecimal operator +(BigDecimal left, long right) => left.Add(right);

    public static BigDecimal operator -(BigDecimal left, BigDecimal right) => left.Subtract(right);
    public static BigDecimal operator -(BigDecimal left, Rational right) => left.Subtract(right);
    public static BigDecimal operator -(BigDecimal left, decimal right) => left.Subtract(right);
    public static BigDecimal operator -(BigDecimal left, double right) => left.Subtract(right);
    public static BigDecimal operator -(BigDecimal left, BigInteger right) => left.Subtract(right);
    public static BigDecimal operator -(BigDecimal left, long right) => left.Subtract(right);

    public static BigDecimal operator *(BigDecimal left, BigDecimal right) => left.Multiply(right);
    public static BigDecimal operator *(BigDecimal left, Rational right) => left.Multiply(right);
    public static BigDecimal operator *(BigDecimal left, decimal right) => left.Multiply(right);
    public static BigDecimal operator *(BigDecimal left, double right) => left.Multiply(right);
    public static BigDecimal operator *(BigDecimal left, BigInteger right) => left.Multiply(right);
    public static BigDecimal operator *(BigDecimal left, long right) => left.Multiply(right);

    public static BigDecimal operator /(BigDecimal left, BigDecimal right) => left.Divide(right);
    public static BigDecimal operator /(BigDecimal left, Rational right) => left.Divide(right);
    public static BigDecimal operator /(BigDecimal left, decimal right) => left.Divide(right);
    public static BigDecimal operator /(BigDecimal left, double right) => left.Divide(right);
    public static BigDecimal operator /(BigDecimal left, BigInteger right) => left.Divide(right);
    public static BigDecimal operator /(BigDecimal left, long right) => left.Divide(right);

    public static BigDecimal operator %(BigDecimal left, BigDecimal right) => left.Mod(right);
    public static BigDecimal operator %(BigDecimal left, Rational right) => left.Mod(right);
    public static BigDecimal operator %(BigDecimal left, decimal right) => left.Mod(right);
    public static BigDecimal operator %(BigDecimal left, double right) => left.Mod(right);
    public static BigDecimal operator %(BigDecimal left, BigInteger right) => left.Mod(right);
    public static BigDecimal operator %(BigDecimal left, long right) => left.Mod(right);

    #endregion
    #region Properties

    public static BigDecimal NegativeOne { get; } = new BigDecimal(BigInteger.MinusOne, 0);
    public static BigDecimal Zero { get; } = new BigDecimal(BigInteger.Zero, 0);
    public static BigDecimal One { get; } = new BigDecimal(BigInteger.One, 0);

    public static BigDecimal E { get; }
    public static BigDecimal Pi { get; }
    public static BigDecimal Tau { get; }

    public static int Radix { get; } = 10;
    public static BigDecimal AdditiveIdentity => Zero;
    public static BigDecimal MultiplicativeIdentity => One;
    #endregion

    #region Math
    public static BigDecimal Normalized(BigInteger mantissa, int exponent)
    {
        if (mantissa.IsZero)
        {
            return new BigDecimal(mantissa, 0);
        }

        BigInteger remainder = BigInteger.Zero;
        while (remainder == BigInteger.Zero)
        {
            var shortened = BigInteger.DivRem(mantissa, MathHelper.BigInt.Ten, out remainder);
            if (remainder == BigInteger.Zero)
            {
                mantissa = shortened;
                exponent++;
            }
        }

        return new BigDecimal(mantissa, exponent);
    }

    public static BigDecimal Exp(double exponent)
    {
        BigDecimal tmp = One;
        while (Math.Abs(exponent) > 100d)
        {
            int diff = exponent > 0 ? 100 : -100;
            tmp *= (BigDecimal)Math.Exp(diff);
            exponent -= diff;
        }

        return tmp * (BigDecimal)Math.Exp(exponent);
    }

    public static BigDecimal Pow(double basis, double exponent)
    {
        var tmp = (BigDecimal)1;
        while (Math.Abs(exponent) > 100)
        {
            int diff = exponent > 0 ? 100 : -100;
            tmp *= (BigDecimal)Math.Pow(basis, diff);
            exponent -= diff;
        }

        return tmp * (BigDecimal)Math.Pow(basis, exponent);
    }

    public static BigDecimal Abs(BigDecimal value)
    {
        if (IsNegative(value))
            return value.Negate();
        return value;
    }

    public static BigDecimal Truncate(BigDecimal x, int digits)
    {
        var (mantissa, exponent) = x;
        while (mantissa.NumberOfDigits() > digits)
        {
            mantissa /= MathHelper.BigInt.Ten;
            exponent++;
        }
        return Normalized(mantissa, exponent);
    }

    public static BigDecimal Round(BigDecimal x, int digits, MidpointRounding mode) => throw new NotImplementedException();


    public static BigDecimal MinMagnitude(BigDecimal x, BigDecimal y) => throw new NotImplementedException();

    public static BigDecimal MinMagnitudeNumber(BigDecimal x, BigDecimal y) => throw new NotImplementedException();

    public static BigDecimal MaxMagnitude(BigDecimal x, BigDecimal y) => throw new NotImplementedException();

    public static BigDecimal MaxMagnitudeNumber(BigDecimal x, BigDecimal y) => throw new NotImplementedException();
    #endregion

    #region IsXyz
    public static bool IsCanonical(BigDecimal _) =>
        // Right now, so long as everything goes through Normalize, this is always true
        true;

    public static bool IsComplexNumber(BigDecimal _) => false;
    public static bool IsImaginaryNumber(BigDecimal _) => false;
    public static bool IsRealNumber(BigDecimal _) => true;

    public static bool IsFinite(BigDecimal _) => true;
    public static bool IsNaN(BigDecimal _) => false;
    public static bool IsInfinity(BigDecimal _) => false;
    public static bool IsNegativeInfinity(BigDecimal _) => false;
    public static bool IsPositiveInfinity(BigDecimal _) => false;

    public static bool IsInteger(BigDecimal value) => value._exponent == 0;
    public static bool IsEvenInteger(BigDecimal value) => value._exponent == 0 && value._mantissa.IsEven;
    public static bool IsOddInteger(BigDecimal value) => value._exponent == 0 && !value._mantissa.IsEven;
    public static bool IsZero(BigDecimal value) => value._mantissa.IsZero;
    public static bool IsNegative(BigDecimal value) => value._mantissa < BigInteger.Zero;
    public static bool IsPositive(BigDecimal value) => value._mantissa > BigInteger.Zero;

    public static bool IsNormal(BigDecimal value) => value != Zero;

    public static bool IsSubnormal(BigDecimal _) => false;


    #endregion

    #region Non-Public Methods
    private static (BigInteger LeftMantissa, BigInteger RightMantissa, int Exponent) Aligned(
        BigDecimal left,
        BigDecimal right)
    {
        if (left._exponent == right._exponent)
        {
            return (left._mantissa, right._mantissa, left._exponent);
        }
        else if (left._exponent > right._exponent)
        {
            // left adjusts to right
            int shift = left._exponent - right._exponent;
            var leftMantissa = left._mantissa * MathHelper.TenPow(shift);
            return (leftMantissa, right._mantissa, right._exponent);
        }
        else
        {
            // right adjusts to left
            int shift = right._exponent - left._exponent;
            var rightMantissa = right._mantissa * MathHelper.TenPow(shift);
            return (left._mantissa, rightMantissa, left._exponent);
        }
    }
    private static (BigInteger LeftMantissa, BigInteger RightMantissa, int Exponent) Aligned(
        BigDecimal left,
        FloatingPointInfo right)
    {
        if (left._exponent == right.Exponent)
        {
            return (left._mantissa, right.Mantissa, left._exponent);
        }
        else if (left._exponent > right.Exponent)
        {
            // left adjusts to right
            int shift = left._exponent - right.Exponent;
            var leftMantissa = left._mantissa * MathHelper.TenPow(shift);
            return (leftMantissa, right.Mantissa, right.Exponent);
        }
        else
        {
            // right adjusts to left
            int shift = right.Exponent - left._exponent;
            var rightMantissa = right.Mantissa * MathHelper.TenPow(shift);
            return (left._mantissa, rightMantissa, left._exponent);
        }
    }
    #endregion

    #region Try Convert
    #region Parse

    public static BigDecimal Parse(string str, IFormatProvider? provider = default)
    {
        if (TryParse(str, provider, out var bigDec))
            return bigDec;
        throw new ParseException(str, typeof(BigDecimal))
        {
            {nameof(provider), provider },
        };
    }

    public static BigDecimal Parse(ReadOnlySpan<char> text, IFormatProvider? provider = default)
    {
        if (TryParse(text, provider, out var bigDec))
            return bigDec;
        throw new ParseException(text, typeof(BigDecimal))
        {
            {nameof(provider), provider },
        };
    }

    public static BigDecimal Parse(string str, NumberStyles style, IFormatProvider? provider = default)
    {
        if (TryParse(str, style, provider, out var bigDec))
            return bigDec;
        throw new ParseException(str, typeof(BigDecimal))
        {
            {nameof(style), style },
            {nameof(provider), provider },
        };
    }

    public static BigDecimal Parse(ReadOnlySpan<char> text, NumberStyles style, IFormatProvider? provider = default)
    {
        if (TryParse(text, style, provider, out var bigDec))
            return bigDec;
        throw new ParseException(text, typeof(BigDecimal))
        {
            {nameof(style), style },
            {nameof(provider), provider },
        };
    }


    public static bool TryParse([NotNullWhen(true)] string? str, IFormatProvider? provider, out BigDecimal bigDec)
    {
        if (str is null)
        {
            bigDec = Zero;
            return false;
        }

        return TryParse(str.AsSpan(), NumberStyles.Float, provider, out bigDec);
    }

    public static bool TryParse(ReadOnlySpan<char> text, IFormatProvider? provider, out BigDecimal bigDec) => TryParse(text, NumberStyles.Float, provider, out bigDec);

    public static bool TryParse([NotNullWhen(true)] string? str, NumberStyles style, IFormatProvider? provider, out BigDecimal bigDec)
    {
        if (str is null)
        {
            bigDec = Zero;
            return false;
        }

        return TryParse(str.AsSpan(), style, provider, out bigDec);
    }

    public static bool TryParse(ReadOnlySpan<char> text, NumberStyles style, IFormatProvider? provider, out BigDecimal bigDec)
    {
        // Find the floating point
        int pointIndex = text.IndexOf(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator.AsSpan(), StringComparison.OrdinalIgnoreCase);
        // Does not exists == all mantissa
        if (pointIndex == -1)
        {
#if NETFRAMEWORK || NETSTANDARD2_0
            if (BigInteger.TryParse(text.AsString(), style, provider, out BigInteger mantissa))
#else
            if (BigInteger.TryParse(text, style, provider, out BigInteger mantissa))
#endif
            {
                bigDec = new BigDecimal(mantissa, 0);
                return true;
            }

            bigDec = BigDecimal.Zero;
            return false;
        }
        else
        {
            // Parse everything before
            ReadOnlySpan<char> mantissaText = text[..pointIndex];
#if NETFRAMEWORK || NETSTANDARD2_0
            if (!BigInteger.TryParse(mantissaText.AsString(), style, provider, out BigInteger high))
#else
            if (!BigInteger.TryParse(mantissaText, style, provider, out BigInteger high))
#endif
            {
                bigDec = BigDecimal.Zero;
                return false;
            }
            int shift = text.Length - pointIndex - 1;
            var mantissa = high * MathHelper.TenPow(shift);

            // Parse everything after
            mantissaText = text.Slice(pointIndex + 1);
#if NETFRAMEWORK || NETSTANDARD2_0
            if (!BigInteger.TryParse(mantissaText.AsString(), style, provider, out BigInteger low))
#else
            if (!BigInteger.TryParse(mantissaText, style, provider, out BigInteger low))
#endif
            {
                bigDec = BigDecimal.Zero;
                return false;
            }

            if (mantissa < 0)
            {
                mantissa -= low;
            }
            else
            {
                mantissa += low;
            }

            bigDec = Normalized(mantissa, shift);
            return true;
        }
    }

    #endregion
    #region From
#if NET7_0_OR_GREATER
    static bool INumberBase<BigDecimal>.TryConvertFromChecked<TOther>(TOther value, out BigDecimal result) => throw new NotImplementedException();

    static bool INumberBase<BigDecimal>.TryConvertFromSaturating<TOther>(TOther value, out BigDecimal result) => throw new NotImplementedException();

    static bool INumberBase<BigDecimal>.TryConvertFromTruncating<TOther>(TOther value, out BigDecimal result) => throw new NotImplementedException();
#endif

    public static Result<BigDecimal, Exception> TryConvertFrom<TValue>(TValue? value)
    {
        switch (value)
        {
            case Rational rational:
            {
                var bigDec = rational.ToBigDecimal();
                return bigDec;
            }
            case decimal dec:
            {
                var info = dec.GetInfo();
                var bigDec = Normalized(info.Mantissa, info.Exponent);
                return bigDec;
            }
            case double f64:
            {
                var info = f64.GetInfo();
                var bigDec = Normalized(info.Mantissa, info.Exponent);
                return bigDec;
            }
            case BigInteger bigInt:
            {
                var bigDec = new BigDecimal(bigInt, 0);
                return bigDec;
            }
            case long i64:
            {
                var bigDec = new BigDecimal(i64, 0);
                return bigDec;
            }
            default:
                throw new NotImplementedException();
        }

        //return new NotImplementedException();
        //bigDec = BigDecimal.Zero;
        //return false;
    }

    #endregion
    #region To
#if NET7_0_OR_GREATER
    static bool INumberBase<BigDecimal>.TryConvertToChecked<TOther>(BigDecimal value, [MaybeNullWhen(false)] out TOther result) => throw new NotImplementedException();

    static bool INumberBase<BigDecimal>.TryConvertToSaturating<TOther>(BigDecimal value, [MaybeNullWhen(false)] out TOther result) => throw new NotImplementedException();

    static bool INumberBase<BigDecimal>.TryConvertToTruncating<TOther>(BigDecimal value, [MaybeNullWhen(false)] out TOther result) => throw new NotImplementedException();
#endif

    public static bool TryConvertTo<TOut>(BigDecimal bigDec, [MaybeNullWhen(false)] out TOut? result) => throw new NotImplementedException();

    #endregion

    #endregion


    #endregion

    private readonly BigInteger _mantissa;
    private readonly int _exponent;

    private BigDecimal(BigInteger mantissa, int exponent)
    {
        _mantissa = mantissa;
        _exponent = exponent;
    }
    private void Deconstruct(out BigInteger mantissa, out int exponent)
    {
        mantissa = _mantissa;
        exponent = _exponent;
    }



    #region Math
    //https://en.wikipedia.org/wiki/Floating-point_arithmetic

    public BigDecimal Plus() => this;
    public BigDecimal Negate() => new BigDecimal(-_mantissa, _exponent);

    public BigDecimal Increment() => Add(One);
    public BigDecimal Decrement() => Subtract(One);

    public BigDecimal Add(BigDecimal other)
    {
        var (leftMantissa, rightMantissa, exponent) = Aligned(this, other);
        return Normalized(leftMantissa + rightMantissa, exponent);
    }
    public BigDecimal Add(Rational other) => Add((BigDecimal)other);
    public BigDecimal Add(decimal other) => Add((BigDecimal)other);
    public BigDecimal Add(double other) => Add((BigDecimal)other);
    public BigDecimal Add(BigInteger other) => Add((BigDecimal)other);
    public BigDecimal Add(long other) => Add((BigDecimal)other);

    public BigDecimal Subtract(BigDecimal other)
    {
        var (leftMantissa, rightMantissa, exponent) = Aligned(this, other);
        return Normalized(leftMantissa - rightMantissa, exponent);
    }
    public BigDecimal Subtract(Rational other) => Subtract((BigDecimal)other);
    public BigDecimal Subtract(decimal other) => Subtract((BigDecimal)other);
    public BigDecimal Subtract(double other) => Subtract((BigDecimal)other);
    public BigDecimal Subtract(BigInteger other) => Subtract((BigDecimal)other);
    public BigDecimal Subtract(long other) => Subtract((BigDecimal)other);

    public BigDecimal Multiply(BigDecimal other)
    {
        return Normalized(
            _mantissa * other._mantissa,
            _exponent + other._exponent);
    }
    public BigDecimal Multiply(Rational other) => Multiply((BigDecimal)other);
    public BigDecimal Multiply(decimal other) => Multiply((BigDecimal)other);
    public BigDecimal Multiply(double other) => Multiply((BigDecimal)other);
    public BigDecimal Multiply(BigInteger other) => Multiply((BigDecimal)other);
    public BigDecimal Multiply(long other) => Multiply((BigDecimal)other);

    public BigDecimal Divide(BigDecimal other)
    {
        return Normalized(
            _mantissa / other._mantissa,
            _exponent - other._exponent);
    }
    public BigDecimal Divide(Rational other) => Divide((BigDecimal)other);
    public BigDecimal Divide(decimal other) => Divide((BigDecimal)other);
    public BigDecimal Divide(double other) => Divide((BigDecimal)other);
    public BigDecimal Divide(BigInteger other) => Divide((BigDecimal)other);
    public BigDecimal Divide(long other) => Divide((BigDecimal)other);


    public BigDecimal Mod(BigDecimal other) => this - (other * (this / other));
    public BigDecimal Mod(Rational other) => Mod((BigDecimal)other);
    public BigDecimal Mod(decimal other) => Mod((BigDecimal)other);
    public BigDecimal Mod(double other) => Mod((BigDecimal)other);
    public BigDecimal Mod(BigInteger other) => Mod((BigDecimal)other);
    public BigDecimal Mod(long other) => Mod((BigDecimal)other);

    #endregion

    public Rational ToRational() => Rational.FromBigDecimal(this);
   


    #region Compare
    public int CompareTo(BigDecimal other)
    {
        var (leftMantissa, rightMantissa, _) = Aligned(this, other);
        return leftMantissa.CompareTo(rightMantissa);
    }

    public int CompareTo(decimal other)
    {
        if (TryConvertFrom<decimal>(other).HasOk(out var bigDec))
            return CompareTo(bigDec);
        // unknown sorts before known
        return 1;
    }

    public int CompareTo(double other)
    {
        if (TryConvertFrom<double>(other).HasOk(out var bigDec))
            return CompareTo(bigDec);
        // unknown sorts before known
        return 1;
    }

    public int CompareTo(BigInteger other)
    {
        if (TryConvertFrom<BigInteger>(other).HasOk(out var bigDec))
            return CompareTo(bigDec);
        // unknown sorts before known
        return 1;
    }

    public int CompareTo(long other)
    {
        if (TryConvertFrom<long>(other).HasOk(out var bigDec))
            return CompareTo(bigDec);
        // unknown sorts before known
        return 1;
    }

    public int CompareTo(object? obj)
    {
        if (TryConvertFrom(obj).HasOk(out var bigDec))
            return CompareTo(bigDec);
        // unknown sorts before known
        return 1;
    }




    #endregion

    #region Equate
    public bool Equals(BigDecimal other)
    {
        return other._mantissa == _mantissa &&
            other._exponent == _exponent;
    }

    public bool Equals(decimal other)
    {
        if (TryConvertFrom<decimal>(other).HasOk(out var bigDec))
            return Equals(bigDec);
        // unknown sorts before known
        return false;
    }

    public bool Equals(double other)
    {
        if (TryConvertFrom<double>(other).HasOk(out var bigDec))
            return Equals(bigDec);
        // unknown sorts before known
        return false;
    }

    public bool Equals(BigInteger other)
    {
        if (TryConvertFrom<BigInteger>(other).HasOk(out var bigDec))
            return Equals(bigDec);
        // unknown sorts before known
        return false;
    }

    public bool Equals(long other)
    {
        if (TryConvertFrom<long>(other).HasOk(out var bigDec))
            return Equals(bigDec);
        // unknown sorts before known
        return false;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (TryConvertFrom(obj).HasOk(out var bigDec))
            return Equals(bigDec);
        return false;
    }

    #endregion

    public override int GetHashCode() => Hasher.Combine(_mantissa, _exponent);

    public bool TryFormat(Span<char> destination, out int charsWritten,
        ReadOnlySpan<char> format = default,
        IFormatProvider? provider = default) => throw new NotImplementedException();

    public string ToString(string? format, IFormatProvider? formatProvider = default) => throw new NotImplementedException();

    public override string ToString()
    {
        var text = new Buffer<char>();
        text.Write(_mantissa);
        if (_exponent == 0)
        {
            // fin
        }
        else if (_exponent > text.Count)
        {
            int zeros = _exponent - text.Count;
            int offset = 0;
            if (_mantissa < 0)
            {
                zeros += 1;
                offset = 1;
            }

            Span<char> buf = stackalloc char[2 + zeros];
            buf[0] = '0';
            buf[1] = '.';
            buf[2..].Fill('0');

            text.TryInsertMany(offset, buf).ThrowIfError();
        }
        else
        {
            int offset = text.Count - _exponent;
            text.TryInsert(offset, '.').ThrowIfError();
        }

        return text.ToStringAndDispose();
    }













    public int GetExponentByteCount() => throw new NotImplementedException();

    public int GetExponentShortestBitLength() => throw new NotImplementedException();

    public int GetSignificandBitLength() => throw new NotImplementedException();

    public int GetSignificandByteCount() => throw new NotImplementedException();



    public bool TryWriteExponentBigEndian(Span<byte> destination, out int bytesWritten) => throw new NotImplementedException();

    public bool TryWriteExponentLittleEndian(Span<byte> destination, out int bytesWritten) => throw new NotImplementedException();

    public bool TryWriteSignificandBigEndian(Span<byte> destination, out int bytesWritten) => throw new NotImplementedException();

    public bool TryWriteSignificandLittleEndian(Span<byte> destination, out int bytesWritten) => throw new NotImplementedException();


}
