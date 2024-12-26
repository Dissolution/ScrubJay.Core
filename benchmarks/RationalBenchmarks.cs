using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using BenchmarkDotNet.Attributes;

namespace Benchmarks;

public class RationalBenchmarks
{
    public static List<decimal> TestDecimals { get; } = [];

    static RationalBenchmarks()
    {
        TestDecimals.Add(0.00000000000000000000000000001m);
        var rand = new Random();
        while (TestDecimals.Count < 8)
        {
            decimal m = (decimal)rand.NextDouble();
            TestDecimals.Add(m);
        }
    }

    [Benchmark]
    [ArgumentsSource(nameof(TestDecimals))]
    public (BigInteger, BigInteger) RationalFromDecimalFirst(decimal dec)
    {
        var digitCount = dec.TrailingDigitCount();
        if (digitCount <= 18)
        {
            long denominator = (long)Math.Pow(10, digitCount);
            var numerator = new BigInteger(dec * denominator);
            var rational = (numerator, denominator);
            return rational;
        }
        else
        {
            return RationalFromDecimalSecond(dec);
        }
    }

    [Benchmark]
    [ArgumentsSource(nameof(TestDecimals))]
    public (BigInteger, BigInteger) RationalFromDecimalSecond(decimal dec)
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
        BigInteger denominator = BigInteger.Pow(10, exponent);

        var rational = (numerator, denominator);
        return rational;
    }


    [Benchmark]
    [ArgumentsSource(nameof(TestDecimals))]
    public (BigInteger, BigInteger) RationalFromDecimalThird(decimal dec)
    {
        Span<char> text = stackalloc char[31]; // Maximum number of characters in G representation (-Decimal.Epsilon).ToString().Length == 31
        bool wrote = dec.TryFormat(text, out int charsWritten, ['G'], CultureInfo.InvariantCulture);
        Debug.Assert(wrote);

        int exponent;
        int i = text.IndexOf('.');
        if (i >= 0)
        {
            charsWritten -= 1;
            exponent = (charsWritten - i);
            text[(i + 1)..].CopyTo(text[i..]);
        }
        else
        {
            exponent = 0;
        }

        var parsed = BigInteger.TryParse(
            text.Slice(0, charsWritten).ToString(),
            NumberStyles.Integer,
            CultureInfo.InvariantCulture,
            out BigInteger numerator);
        Debug.Assert(parsed);
        BigInteger denominator = BigInteger.Pow(10, exponent);

        var rational = (numerator, denominator);
        return rational;
    }


}