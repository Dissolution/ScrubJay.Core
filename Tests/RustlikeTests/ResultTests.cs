using static ScrubJay.Rustlike.RustlikePrelude;
using text = System.ReadOnlySpan<char>;
using ScrubJay.Rustlike;

namespace ScrubJay.Tests.RustlikeTests;

public class ResultTests
{
    [Fact]
    public void IsOkWorks()
    {
        Result<int, text> x = Ok(-3);
        Assert.True(x.IsOk());

        Result<int, text> y = Err("Some error message".AsSpan());
        Assert.False(y.IsOk());
    }

    [Fact]
    public void IsOkAndWorks()
    {
        Result<uint, text> x = Ok(2U);
        Assert.True(x.IsOkAnd(v => v > 1));

        Result<uint, text> y = Ok(0U);
        Assert.False(y.IsOkAnd(v => v > 1));

        Result<uint, text> z = Err("hey".AsSpan());
        Assert.False(z.IsOkAnd(v => v > 1));
    }

    [Fact]
    public void OkWorks()
    {
        Result<uint, text> x = Ok(2U);
        Assert.True(x.Ok() == Some(2U));

        Result<uint, text> y = Err("Nothing here".AsSpan());
        Assert.True(x.Ok() == None);
    }

    [Fact]
    public void IsErrWorks()
    {
        Result<int, text> x = Ok(-3);
        Assert.False(x.IsErr());

        Result<int, text> y = Err("Some error message".AsSpan());
        Assert.True(y.IsErr());
    }

    [Fact]
    public void IsErrAndWorks()
    {
        Result<uint, Exception> x = Err<Exception>(new DllNotFoundException());
        Assert.True(x.IsErrAnd(v => v is DllNotFoundException));

        Result<uint, Exception> y = Err<Exception>(new InvalidCastException());
        Assert.False(x.IsErrAnd(v => v is DllNotFoundException));

        Result<uint, Exception> z = Ok(123U);
        Assert.False(x.IsErrAnd(v => v is DllNotFoundException));
    }
}
