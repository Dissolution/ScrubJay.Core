// // https://gist.github.com/JcBernack/0b4eef59ca97ee931a2f45542b9ff06d
//
// using System.Globalization;
// using ScrubJay.Memory;
// using ScrubJay.Parsing;
// using ScrubJay.Text;
//
//
// /* The order when referencing other types (equality, comparison, etc):
//  * BigDecimal
//  * Rational
//  * decimal
//  * double
//  * float (only for helpful rounding circumstances)
//  * BigInteger
//  * long
//  */
//
// namespace ScrubJay.Maths;
//
// [PublicAPI]
// [StructLayout(LayoutKind.Auto)]
// public readonly struct BigDecimal :
// #if NET7_0_OR_GREATER
//     //IBinaryInteger<BigDecimal>,
//     /*ISignedNumber<BigDecimal>,
//     INumber<BigDecimal>,
//     INumberBase<BigDecimal>,
//     IFloatingPoint<BigDecimal>,*/
// #endif
//     IAlgebraic<BigDecimal, BigDecimal>,
//     IAlgebraic<BigDecimal, Rational>,
//     IAlgebraic<BigDecimal, decimal>,
//     IAlgebraic<BigDecimal, double>,
//     IEquatable<BigInteger>,
//     IComparable<BigInteger>,
//     ISpanFormattable,
//     IFormattable
// {
//     #region Operators
//
//     public static explicit operator BigDecimal(BigInteger bigInt) => FromBigInteger(bigInt);
//     public static explicit operator BigDecimal(double value) => FromDouble(value);
//     public static explicit operator BigDecimal(decimal value) => FromDecimal(value);
//     public static explicit operator BigDecimal(long i64) => FromInt64(i64);
//
//     public static explicit operator BigInteger(BigDecimal dec) => dec.ToBigInteger();
//     public static explicit operator double(BigDecimal dec) => dec.ToDouble();
//     public static explicit operator decimal(BigDecimal dec) => dec.ToDecimal();
//     public static explicit operator long(BigDecimal dec) => dec.ToInt64();
//
//     public static bool operator ==(BigDecimal left, BigDecimal right) => left.Equals(right);
//     public static bool operator ==(BigDecimal left, Rational right) => left.Equals(right);
//     public static bool operator ==(BigDecimal left, decimal right) => left.Equals(right);
//     public static bool operator ==(BigDecimal left, double right) => left.Equals(right);
//
//     public static bool operator !=(BigDecimal left, BigDecimal right) => !left.Equals(right);
//     public static bool operator !=(BigDecimal left, Rational right) => !left.Equals(right);
//     public static bool operator !=(BigDecimal left, decimal right) => !left.Equals(right);
//     public static bool operator !=(BigDecimal left, double right) => !left.Equals(right);
//
//     public static bool operator <(BigDecimal left, BigDecimal right) => left.CompareTo(right) < 0;
//     public static bool operator <(BigDecimal left, Rational right) => left.CompareTo(right) < 0;
//     public static bool operator <(BigDecimal left, decimal right) => left.CompareTo(right) < 0;
//     public static bool operator <(BigDecimal left, double right) => left.CompareTo(right) < 0;
//
//     public static bool operator <=(BigDecimal left, BigDecimal right) => left.CompareTo(right) <= 0;
//     public static bool operator <=(BigDecimal left, Rational right) => left.CompareTo(right) <= 0;
//     public static bool operator <=(BigDecimal left, decimal right) => left.CompareTo(right) <= 0;
//     public static bool operator <=(BigDecimal left, double right) => left.CompareTo(right) <= 0;
//
//     public static bool operator >(BigDecimal left, BigDecimal right) => left.CompareTo(right) > 0;
//     public static bool operator >(BigDecimal left, Rational right) => left.CompareTo(right) > 0;
//     public static bool operator >(BigDecimal left, decimal right) => left.CompareTo(right) > 0;
//     public static bool operator >(BigDecimal left, double right) => left.CompareTo(right) > 0;
//
//     public static bool operator >=(BigDecimal left, BigDecimal right) => left.CompareTo(right) >= 0;
//     public static bool operator >=(BigDecimal left, Rational right) => left.CompareTo(right) >= 0;
//     public static bool operator >=(BigDecimal left, decimal right) => left.CompareTo(right) >= 0;
//     public static bool operator >=(BigDecimal left, double right) => left.CompareTo(right) >= 0;
//
//     public static BigDecimal operator +(BigDecimal value) => value.Plus();
//     public static BigDecimal operator -(BigDecimal value) => value.Negate();
//     public static BigDecimal operator ++(BigDecimal value) => value.Increment();
//     public static BigDecimal operator --(BigDecimal value) => value.Decrement();
//
//     public static BigDecimal operator +(BigDecimal left, BigDecimal right) => left.Add(right);
//     public static BigDecimal operator +(BigDecimal left, Rational right) => left.Add(right);
//     public static BigDecimal operator +(BigDecimal left, decimal right) => left.Add(right);
//     public static BigDecimal operator +(BigDecimal left, double right) => left.Add(right);
//
//     public static BigDecimal operator -(BigDecimal left, BigDecimal right) => left.Subtract(right);
//     public static BigDecimal operator -(BigDecimal left, Rational right) => left.Subtract(right);
//     public static BigDecimal operator -(BigDecimal left, decimal right) => left.Subtract(right);
//     public static BigDecimal operator -(BigDecimal left, double right) => left.Subtract(right);
//
//     public static BigDecimal operator *(BigDecimal left, BigDecimal right) => left.Multiply(right);
//     public static BigDecimal operator *(BigDecimal left, Rational right) => left.Multiply(right);
//     public static BigDecimal operator *(BigDecimal left, decimal right) => left.Multiply(right);
//     public static BigDecimal operator *(BigDecimal left, double right) => left.Multiply(right);
//
//     public static BigDecimal operator /(BigDecimal left, BigDecimal right) => left.Divide(right);
//     public static BigDecimal operator /(BigDecimal left, Rational right) => left.Divide(right);
//     public static BigDecimal operator /(BigDecimal left, decimal right) => left.Divide(right);
//     public static BigDecimal operator /(BigDecimal left, double right) => left.Divide(right);
//
//     public static BigDecimal operator %(BigDecimal left, BigDecimal right) => left.Mod(right);
//     public static BigDecimal operator %(BigDecimal left, Rational right) => left.Mod(right);
//     public static BigDecimal operator %(BigDecimal left, decimal right) => left.Mod(right);
//     public static BigDecimal operator %(BigDecimal left, double right) => left.Mod(right);
//
//     #endregion
//
//     #region Static Properties
//
//     public static BigDecimal AdditiveIdentity => Zero;
//     public static BigDecimal MultiplicativeIdentity => One;
//     public static BigDecimal One { get; } = new BigDecimal(BigInteger.One, 0);
//     public static BigDecimal Zero { get; } = new BigDecimal(BigInteger.Zero, 0);
//     public static BigDecimal NegativeOne { get; } = new BigDecimal(BigInteger.MinusOne, 0);
//     public static int Radix { get; } = 10;
//
//     public static BigDecimal E { get; } = FromDecimal(2.7182818284590452353602874714m);
//     public static BigDecimal Pi { get; } = FromDecimal(3.1415926535897932384626433833m);
//     public static BigDecimal Tau { get; } = FromDecimal(6.2831853071795864769252867666m);
//
//     #endregion
//
//
//     private static (BigInteger LeftMantissa, BigInteger RightMantissa, int Exponent) Aligned(BigDecimal left,
//         BigDecimal right)
//     {
//         if (left.Exponent == right.Exponent)
//         {
//             return (left.Mantissa, right.Mantissa, left.Exponent);
//         }
//         else if (left.Exponent > right.Exponent)
//         {
//             // left adjusts to right
//             int shift = left.Exponent - right.Exponent;
//             var leftMantissa = left.Mantissa * MathHelper.TenPow(shift);
//             return (leftMantissa, right.Mantissa, right.Exponent);
//         }
//         else
//         {
//             // right adjusts to left
//             int shift = right.Exponent - left.Exponent;
//             var rightMantissa = right.Mantissa * MathHelper.TenPow(shift);
//             return (left.Mantissa, rightMantissa, left.Exponent);
//         }
//     }
//     
//     private static (BigInteger LeftMantissa, BigInteger RightMantissa, int Exponent) Aligned(
//         BigDecimal left, decimal right)
//     {
//
//         throw new NotImplementedException();
//     }
//
//     #region Parsing
//
//     #region From
//
//     public static BigDecimal FromInt64(long i64)
//     {
//         return new BigDecimal(new BigInteger(i64), 0);
//     }
//
//     public static BigDecimal FromBigInteger(BigInteger bigInt)
//     {
//         return new BigDecimal(bigInt, 0);
//     }
//
//     public static BigDecimal FromDouble(double f64)
//     {
//         BigInteger mantissa = new BigInteger(f64);
//         int exponent = 0;
//         double scaleFactor = 1.0d;
//         while (Math.Abs((f64 * scaleFactor) - (double)mantissa) > 0)
//         {
//             exponent -= 1;
//             scaleFactor *= 10.0d;
//             mantissa = new BigInteger(f64 * scaleFactor);
//         }
//
//         return Normalized(mantissa, exponent);
//     }
//
//     public static BigDecimal FromDecimal(decimal dec)
//     {
//         var mantissa = new BigInteger(dec);
//         var exponent = 0;
//         decimal scaleFactor = 1.0m;
//         while ((decimal)mantissa != (dec * scaleFactor))
//         {
//             exponent -= 1;
//             scaleFactor *= 10.0m;
//             mantissa = new BigInteger(dec * scaleFactor);
//         }
//
//         return Normalized(mantissa, exponent);
//     }
//     
//     public static BigDecimal FromRational(Rational rational)
//     {
//         return rational.ToBigDecimal();
//     }
//
//     #endregion
//
//     public static BigDecimal Parse(string str, IFormatProvider? provider = default)
//     {
//         if (TryParse(str, provider, out BigDecimal bigDec))
//             return bigDec;
//         throw new ParseException(str, typeof(BigDecimal))
//         {
//             { nameof(provider), provider },
//         };
//     }
//
//     public static BigDecimal Parse(ReadOnlySpan<char> text, IFormatProvider? provider = default)
//     {
//         if (TryParse(text, provider, out BigDecimal bigDec))
//             return bigDec;
//         throw new ParseException(text, typeof(BigDecimal))
//         {
//             { nameof(provider), provider },
//         };
//     }
//
//     public static BigDecimal Parse(string str, NumberStyles style, IFormatProvider? provider)
//     {
//         if (TryParse(str, style, provider, out BigDecimal bigDec))
//             return bigDec;
//         throw new ParseException(str, typeof(BigDecimal))
//         {
//             { nameof(style), style },
//             { nameof(provider), provider },
//         };
//     }
//     
//     public static BigDecimal Parse(ReadOnlySpan<char> text, NumberStyles style, IFormatProvider? provider)
//     {
//         if (TryParse(text, style, provider, out BigDecimal bigDec))
//             return bigDec;
//         throw new ParseException(text, typeof(BigDecimal))
//         {
//             { nameof(style), style },
//             { nameof(provider), provider },
//         };
//     }
//
//
//     public static bool TryParse([NotNullWhen(true)] string? str, IFormatProvider? provider, out BigDecimal result)
//     {
//         if (str is null)
//         {
//             result = Zero;
//             return false;
//         }
//         
//         return TryParse(str.AsSpan(), provider, out result);
//     }
//
//
//     public static bool TryParse(ReadOnlySpan<char> text, IFormatProvider? provider, out BigDecimal result)
//     {
//         throw new NotImplementedException();
//     }
//
//     public static bool TryParse(
//         [NotNullWhen(true)] string? str, 
//         NumberStyles style, 
//         IFormatProvider? provider,
//         out BigDecimal result)
//     {
//         if (str is null)
//         {
//             result = Zero;
//             return false;
//         }
//         
//         return TryParse(str.AsSpan(), style, provider, out result);
//     }
//     
//     public static bool TryParse(
//         ReadOnlySpan<char> text, 
//         NumberStyles numberStyle, 
//         IFormatProvider? provider,
//         out BigDecimal result)
//     {
//         // int exponent = 0;
//         //
//         // // Find the '.'
//         // int dotIndex = text.IndexOf('.');
//         // if (dotIndex > -1)
//         // {
//         //     exponent = text.Length - dotIndex;
//         // }
//         //
//         // if (!BigInteger.TryParse(mantissaPart, numberStyle, provider, out BigInteger mantissa) ||
//         //     !int.TryParse(exponentPart, numberStyle, provider, out int exponent))
//         // {
//         //     result = Zero;
//         //     return false;
//         // }
//         //
//         // result = new BigDecimal(mantissa, exponent);
//         // return true;
//
//         throw new NotImplementedException();
//     }
//
//     #endregion
//
//     #region Static Math
//
//     public static BigDecimal Exp(double exponent)
//     {
//         var tmp = (BigDecimal)1;
//         while (Math.Abs(exponent) > 100)
//         {
//             var diff = exponent > 0 ? 100 : -100;
//             tmp *= (BigDecimal)Math.Exp(diff);
//             exponent -= diff;
//         }
//
//         return tmp * (BigDecimal)Math.Exp(exponent);
//     }
//
//     public static BigDecimal Pow(double basis, double exponent)
//     {
//         var tmp = (BigDecimal)1;
//         while (Math.Abs(exponent) > 100)
//         {
//             var diff = exponent > 0 ? 100 : -100;
//             tmp *= (BigDecimal)Math.Pow(basis, diff);
//             exponent -= diff;
//         }
//
//         return tmp * (BigDecimal)Math.Pow(basis, exponent);
//     }
//
//     public static BigDecimal Normalized(BigInteger mantissa, int exponent)
//     {
//         if (mantissa.IsZero)
//         {
//             return new BigDecimal(mantissa, 0);
//         }
//
//         BigInteger remainder = BigInteger.Zero;
//         while (remainder == BigInteger.Zero)
//         {
//             var shortened = BigInteger.DivRem(mantissa, MathHelper.BigInteger_Ten, out remainder);
//             if (remainder == BigInteger.Zero)
//             {
//                 mantissa = shortened;
//                 exponent++;
//             }
//         }
//
//         return new BigDecimal(mantissa, exponent);
//     }
//
//     public static BigDecimal Abs(BigDecimal value)
//     {
//         throw new NotImplementedException();
//     }
//
//     public static BigDecimal MaxMagnitude(BigDecimal x, BigDecimal y)
//     {
//         throw new NotImplementedException();
//     }
//
//     public static BigDecimal MaxMagnitudeNumber(BigDecimal x, BigDecimal y)
//     {
//         throw new NotImplementedException();
//     }
//
//     public static BigDecimal MinMagnitude(BigDecimal x, BigDecimal y)
//     {
//         throw new NotImplementedException();
//     }
//
//     public static BigDecimal MinMagnitudeNumber(BigDecimal x, BigDecimal y)
//     {
//         throw new NotImplementedException();
//     }
//
//     public static BigDecimal Round(BigDecimal x, int digits, MidpointRounding mode)
//     {
//         throw new NotImplementedException();
//     }
//
//     #region IsXYZ
//
//     public static bool IsCanonical(BigDecimal value)
//     {
//         throw new NotImplementedException();
//     }
//
//     public static bool IsComplexNumber(BigDecimal value)
//     {
//         throw new NotImplementedException();
//     }
//
//     public static bool IsEvenInteger(BigDecimal value)
//     {
//         throw new NotImplementedException();
//     }
//
//     public static bool IsFinite(BigDecimal value)
//     {
//         throw new NotImplementedException();
//     }
//
//     public static bool IsImaginaryNumber(BigDecimal value)
//     {
//         throw new NotImplementedException();
//     }
//
//     public static bool IsInfinity(BigDecimal value)
//     {
//         throw new NotImplementedException();
//     }
//
//     public static bool IsInteger(BigDecimal value)
//     {
//         throw new NotImplementedException();
//     }
//
//     public static bool IsNaN(BigDecimal value)
//     {
//         throw new NotImplementedException();
//     }
//
//     public static bool IsNegative(BigDecimal value)
//     {
//         throw new NotImplementedException();
//     }
//
//     public static bool IsNegativeInfinity(BigDecimal value)
//     {
//         throw new NotImplementedException();
//     }
//
//     public static bool IsNormal(BigDecimal value)
//     {
//         throw new NotImplementedException();
//     }
//
//     public static bool IsOddInteger(BigDecimal value)
//     {
//         throw new NotImplementedException();
//     }
//
//     public static bool IsPositive(BigDecimal value)
//     {
//         throw new NotImplementedException();
//     }
//
//     public static bool IsPositiveInfinity(BigDecimal value)
//     {
//         throw new NotImplementedException();
//     }
//
//     public static bool IsRealNumber(BigDecimal value)
//     {
//         throw new NotImplementedException();
//     }
//
//     public static bool IsSubnormal(BigDecimal value)
//     {
//         throw new NotImplementedException();
//     }
//
//     public static bool IsZero(BigDecimal value)
//     {
//         throw new NotImplementedException();
//     }
//
//     public static bool IsPow2(BigDecimal value)
//     {
//         throw new NotImplementedException();
//     }
//
//     #endregion
//
//     #endregion
//
//
//     public readonly BigInteger Mantissa;
//     public readonly int Exponent;
//
//     private BigDecimal(BigInteger mantissa, int exponent)
//     {
//         Mantissa = mantissa;
//         Exponent = exponent;
//     }
//
//     private void Deconstruct(out BigInteger mantissa, out int exponent)
//     {
//         mantissa = this.Mantissa;
//         exponent = this.Exponent;
//     }
//
//     #region Floating Point
//
//     /// <summary>
//     /// Removes trailing zeros on the mantissa
//     /// </summary>
//     public BigDecimal Normalize() => Normalized(Mantissa, Exponent);
//
//     /// <summary>
//     /// Truncate the number to the given precision by removing the least significant digits.
//     /// </summary>
//     /// <returns>The truncated number</returns>
//     public BigDecimal Truncate(int precision = 64)
//     {
//         // copy this instance (remember it's a struct)
//         // save some time because the number of digits is not needed to remove trailing zeros
//         var (mantissa, exponent) = Normalize();
//         // remove the least significant digits, as long as the number of digits is higher than the given Precision
//         while (mantissa.NumberOfDigits() > precision)
//         {
//             mantissa /= 10;
//             exponent++;
//         }
//
//         // normalize again to make sure there are no trailing zeros left
//         return Normalized(mantissa, exponent);
//     }
//
//     public BigDecimal Floor()
//     {
//         return Truncate(Mantissa.NumberOfDigits() + Exponent);
//     }
//
//     public int GetExponentByteCount()
//     {
//         throw new NotImplementedException();
//     }
//
//     public int GetExponentShortestBitLength()
//     {
//         throw new NotImplementedException();
//     }
//
//     public int GetSignificandBitLength()
//     {
//         throw new NotImplementedException();
//     }
//
//     public int GetSignificandByteCount()
//     {
//         throw new NotImplementedException();
//     }
//
//     public bool TryWriteExponentBigEndian(Span<byte> destination, out int bytesWritten)
//     {
//         throw new NotImplementedException();
//     }
//
//     public bool TryWriteExponentLittleEndian(Span<byte> destination, out int bytesWritten)
//     {
//         throw new NotImplementedException();
//     }
//
//     public bool TryWriteSignificandBigEndian(Span<byte> destination, out int bytesWritten)
//     {
//         throw new NotImplementedException();
//     }
//
//     public bool TryWriteSignificandLittleEndian(Span<byte> destination, out int bytesWritten)
//     {
//         throw new NotImplementedException();
//     }
//
//     #endregion
//
//     #region Math
//
//     // https://en.wikipedia.org/wiki/Floating-point_arithmetic
//
//
//     public BigDecimal Plus()
//     {
//         // No-op
//         return this;
//     }
//
//     public BigDecimal Negate()
//     {
//         return new BigDecimal(Mantissa * BigInteger.MinusOne, Exponent);
//     }
//
//     public BigDecimal Increment()
//     {
//         return this + BigDecimal.One;
//     }
//
//     public BigDecimal Decrement()
//     {
//         return this - BigDecimal.One;
//     }
//
//     public BigDecimal Add(BigDecimal other)
//     {
//         var aligned = Aligned(this, other);
//         return Normalized(aligned.LeftMantissa + aligned.RightMantissa, aligned.Exponent);
//     }
//     
//     public BigDecimal Add(Rational other)
//     {
//         throw new NotImplementedException();
//     }
//     
//     public BigDecimal Add(decimal other)
//     {
//         throw new NotImplementedException();
//     }
//     
//     public BigDecimal Add(double other)
//     {
//         throw new NotImplementedException();
//     }
//
//     public BigDecimal Subtract(BigDecimal other)
//     {
//         var aligned = Aligned(this, other);
//         return Normalized(aligned.LeftMantissa - aligned.RightMantissa, aligned.Exponent);
//     }
//       
//     public BigDecimal Subtract(Rational other)
//     {
//         throw new NotImplementedException();
//     }
//     
//     public BigDecimal Subtract(decimal other)
//     {
//         throw new NotImplementedException();
//     }
//     
//     public BigDecimal Subtract(double other)
//     {
//         throw new NotImplementedException();
//     }
//     
//
//     public BigDecimal Multiply(BigDecimal other)
//     {
//         return Normalized(
//             this.Mantissa * other.Mantissa,
//             this.Exponent + other.Exponent);
//     }
//     
//     public BigDecimal Multiply(Rational other)
//     {
//         throw new NotImplementedException();
//     }
//     
//     public BigDecimal Multiply(decimal other)
//     {
//         throw new NotImplementedException();
//     }
//     
//     public BigDecimal Multiply(double other)
//     {
//         throw new NotImplementedException();
//     }
//
//     public BigDecimal Divide(BigDecimal other)
//     {
//         return Normalized(
//             this.Mantissa / other.Mantissa,
//             this.Exponent - other.Exponent);
//
//         // var (endMan, endExp) = this;
//         // var exponentChange = 64 - (endMan.NumberOfDigits() - other.Mantissa.NumberOfDigits());
//         // if (exponentChange < 0)
//         //     exponentChange = 0;
//         //
//         // endMan *= BigInteger.Pow(MathHelper.BigInteger_Ten, exponentChange);
//         // return new BigDecimal(endMan / other.Mantissa, this.Exponent - other.Exponent - exponentChange);
//     }
//     
//     public BigDecimal Divide(Rational other)
//     {
//         throw new NotImplementedException();
//     }
//     
//     public BigDecimal Divide(decimal other)
//     {
//         throw new NotImplementedException();
//     }
//     
//     public BigDecimal Divide(double other)
//     {
//         throw new NotImplementedException();
//     }
//
//     public BigDecimal Mod(BigDecimal other)
//     {
//         return this - (other * (this / other).Truncate());
//     }
//     
//     public BigDecimal Mod(Rational other)
//     {
//         throw new NotImplementedException();
//     }
//     
//     public BigDecimal Mod(decimal other)
//     {
//         throw new NotImplementedException();
//     }
//     
//     public BigDecimal Mod(double other)
//     {
//         throw new NotImplementedException();
//     }
//
//     #endregion
//
//     public BigInteger ToBigInteger()
//     {
//         return this.Mantissa * MathHelper.TenPow(this.Exponent);
//     }
//
//     public double ToDouble()
//     {
//         checked
//         {
//             return (double)Mantissa * Math.Pow(10d, Exponent);
//         }
//     }
//
//     public decimal ToDecimal()
//     {
//         checked
//         {
//             return (decimal)Mantissa * (decimal)MathHelper.TenPow(Exponent);
//         }
//     }
//
//     public long ToInt64()
//     {
//         checked
//         {
//             return (long)Mantissa * (long)MathHelper.TenPow(Exponent);
//         }
//     }
//
//     #region CompareTo
//     public int CompareTo(BigDecimal other)
//     {
//         var aligned = Aligned(this, other);
//         return aligned.LeftMantissa.CompareTo(aligned.RightMantissa);
//     }
//     
//     public int CompareTo(Rational other)
//     {
//         return CompareTo((BigDecimal)other);
//     }
//     
//     public int CompareTo(decimal other)
//     {
//         return this.CompareTo(FromDecimal(other));
//     }
//     
//     public int CompareTo(double other)
//     {
//         return this.CompareTo(FromDouble(other));
//     }
//     
//     public int CompareTo(BigInteger other)
//     {
//         return CompareTo((BigDecimal)other);
//     }
//
//     public int CompareTo(long other)
//     {
//         return this.CompareTo(FromInt64(other));
//     }
//
//     public int CompareTo(object? obj)
//     {
//         return obj switch
//         {
//             BigDecimal bigDec => CompareTo(bigDec),
//             decimal dec => CompareTo(dec),
//             double f64 => CompareTo(f64),
//             BigInteger bigInt => CompareTo(bigInt),
//             long i64 => CompareTo(i64),
//             _ => 1, // unknown sorts before known
//         };
//     }
// #endregion
// #region Equals
//     public bool Equals(BigDecimal other)
//     {
//         return other.Mantissa == Mantissa && other.Exponent == Exponent;
//     }
//     
//     public bool Equals(Rational other)
//     {
//         return Equals(FromRational(other));
//     }
//
//     public bool Equals(decimal other)
//     {
//         return Equals(FromDecimal(other));
//     }
//     
//     public bool Equals(double other)
//     {
//         return Equals(FromDouble(other));
//     }
//     
//     public bool Equals(BigInteger other)
//     {
//         return this.Exponent == 0 && this.Mantissa == other;
//     }
//
//     public bool Equals(long other)
//     {
//         return Equals(FromInt64(other));
//     }
//
//     public override bool Equals(object? obj)
//     {
//         return obj switch
//         {
//             BigDecimal bigDecimal => Equals(bigDecimal),
//             Rational rational => Equals(rational),
//             decimal dec => Equals(dec),
//             double f64 => Equals(f64),
//             BigInteger bigInteger => Equals(bigInteger),
//             long i64 => Equals(i64),
//             _ => false,
//         };
//     }
// #endregion
//     public override int GetHashCode() => Hasher.Combine(Mantissa, Exponent);
//
//     public bool TryFormat(
//         Span<char> destination,
//         out int charsWritten,
//         ReadOnlySpan<char> format = default,
//         IFormatProvider? provider = null)
//     {
//         string str = ToString(format.ToString(), provider);
//         if (str.AsSpan().TryCopyTo(destination))
//         {
//             charsWritten = str.Length;
//             return true;
//         }
//
//         charsWritten = 0;
//         return false;
//     }
//
//     public string ToString(string? format, IFormatProvider? provider = null)
//     {
//         var text = new DefaultInterpolatedStringHandler(1, 2, provider);
//         text.AppendFormatted(Mantissa, format);
//         text.AppendLiteral("e");
//         text.AppendFormatted(Exponent, format);
//         return text.ToStringAndClear();
//     }
//
//     public override string ToString()
//     {
// #if NET6_0_OR_GREATER
//         Buffer<char> buffer = stackalloc char[1024];
//         buffer.Write(Mantissa, "R");
//         if (Exponent < 0)
//         {
//             var dotIndex = buffer.Count + Exponent;
//             buffer.TryInsert(dotIndex, '.').ThrowIfError();
//         }
//         else
//             Debugger.Break();
//
//         return buffer.ToStringAndDispose();
// #else
//         var mStr = Mantissa.ToString("R");
//         if (Exponent > 0)
//         {
//             var dotIndex = mStr.Length - Exponent;
//             mStr = $"{mStr[..dotIndex]}.{mStr[dotIndex..]}";
//         }
//         return mStr;
// #endif
//     }
// }