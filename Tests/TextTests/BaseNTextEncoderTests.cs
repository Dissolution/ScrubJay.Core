using System.Numerics;
using ScrubJay.Text;

namespace ScrubJay.Tests.TextTests;

public class BaseNTextEncoderTests
{
    public static TheoryData<BigInteger> BigIntegers { get; } = new TheoryData<BigInteger>()
    {
        BigInteger.Zero,
        BigInteger.One,
        1024,
        ((BigInteger)int.MaxValue) + ((BigInteger)147),

    };


    [Theory]
    [MemberData(nameof(BigIntegers))]
    public void Base10Works(BigInteger iBig)
    {
        var encoder = BaseNTextEncoder.Base10;

        var encoded = encoder.TryEncode(iBig);
        Assert.True(encoded.IsOk(out var baseString));
        Assert.NotNull(baseString);
        Assert.NotEmpty(baseString);

        long i64 = (long)iBig;
        string i64Str = i64.ToString();
        Assert.Equal(i64Str, baseString);
    }
}
