using System.Reflection;

namespace ScrubJay.Tests.Utilities;

public class ResultVTests
{
    public static IEnumerable<object[]> GetResults()
    {
        yield return new object[1] { Result<int>.Ok(147) };
        yield return new object[1] { Result<BindingFlags>.Ok(BindingFlags.ExactBinding) };
        yield return new object[1] { Result<Nothing>.Ok(default) };
        yield return new object[1] { Result<string>.Ok("ABC") };
        yield return new object[1] { Result<EventArgs?>.Ok(null) };
        
        
        yield return new object[1] { Result<int>.Error(null) };
        yield return new object[1] { Result<int>.Error(new Exception("Bad")) };
        yield return new object[1] { Result<BindingFlags>.Error(null) };
        yield return new object[1] { Result<BindingFlags>.Error(new Exception("Bad")) };
        yield return new object[1] { Result<Nothing>.Error(null) };
        yield return new object[1] { Result<Nothing>.Error(new Exception("Bad")) };
        yield return new object[1] { Result<string>.Error(null) };
        yield return new object[1] { Result<string>.Error(new Exception("Bad")) };
        yield return new object[1] { Result<EventArgs?>.Error(null) };
        yield return new object[1] { Result<EventArgs?>.Error(new Exception("Bad")) };
    }


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

        exResult = Result.Ok(new Exception("Ok"));
        bool isOk = exResult.IsOk(out Exception? okValue);
        Assert.True(isOk);
        Assert.False(exResult.IsError());
        Assert.NotNull(okValue);
        Assert.Equal("Ok", okValue.Message);
        
        exResult = Result.Ok<Exception>(null!);
        isOk = exResult.IsOk(out okValue);
        Assert.True(isOk);
        Assert.False(exResult.IsError());
        Assert.Null(okValue);

        exResult = Result.Error<Exception>(new Exception("Ok"));
        bool isError = exResult.IsError(out Exception? error);
        Assert.True(isError);
        Assert.False(exResult.IsOk());
        Assert.NotNull(error);
        Assert.Equal("Ok", error.Message);
        
        exResult = Result.Error<Exception>(null);
        isError = exResult.IsError(out error);
        Assert.True(isError);
        Assert.False(exResult.IsOk());
        Assert.NotNull(error);
        Assert.Equal("Error", error.Message);
    }
}