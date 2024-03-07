using System.Reflection;

namespace ScrubJay.Tests;

public class ResultTTests
{
    public static IEnumerable<object[]> GetResults() => TestHelper.ToMemberData<object>(1,
        Result<int>.Ok(147),
        Result<BindingFlags>.Ok(BindingFlags.ExactBinding),
        Result<Nothing>.Ok(default),
        Result<string>.Ok("ABC"),
        Result<EventArgs?>.Ok(null),
        Result<int>.Error(null),
        Result<int>.Error(new Exception("Bad")),
        Result<BindingFlags>.Error(null),
        Result<BindingFlags>.Error(new Exception("Bad")),
        Result<Nothing>.Error(null),
        Result<Nothing>.Error(new Exception("Bad")),
        Result<string>.Error(null),
        Result<string>.Error(new Exception("Bad")),
        Result<EventArgs?>.Error(null),
        Result<EventArgs?>.Error(new Exception("Bad")));


    [Theory]
    [MemberData(nameof(GetResults))]
    public void CannotBeOkAndError<T>(Result<T> result)
    {
        bool ok = result.IsOk();
        bool error = result.IsError();
        Assert.NotEqual(ok, error);
    }

    [Theory]
    [MemberData(nameof(GetResults))]
    public void GetErrorIsAlwaysNotNull<T>(Result<T> result)
    {
        if (result.IsError(out var error))
        {
            Assert.NotNull(error);
        }
    }

    [Fact]
    public void CanDealWithOkExceptionType()
    {
        Result<Exception> exResult = new Result<Exception>();
        Assert.True(exResult.IsError());
        Assert.False(exResult.IsOk());

        exResult = Result<Exception>.Ok(new Exception("Ok"));
        bool isOk = exResult.IsOk(out Exception? okValue);
        Assert.True(isOk);
        Assert.False(exResult.IsError());
        Assert.NotNull(okValue);
        Assert.Equal("Ok", okValue.Message);

        exResult = Result<Exception>.Ok(null!);
        isOk = exResult.IsOk(out okValue);
        Assert.True(isOk);
        Assert.False(exResult.IsError());
        Assert.Null(okValue);

        exResult = Result<Exception>.Error(new Exception("Ok"));
        bool isError = exResult.IsError(out Exception? error);
        Assert.True(isError);
        Assert.False(exResult.IsOk());
        Assert.NotNull(error);
        Assert.Equal("Ok", error.Message);

        exResult = Result<Exception>.Error(null);
        isError = exResult.IsError(out error);
        Assert.True(isError);
        Assert.False(exResult.IsOk());
        Assert.NotNull(error);
        Assert.Equal("Error", error.Message);
    }
}