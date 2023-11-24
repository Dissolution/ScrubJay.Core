namespace ScrubJay.Tests.Utilities;

public class ResultTests
{
    public static IEnumerable<object[]> GetResults()
    {
        yield return new object[1] { Result.Ok() };
        yield return new object[1] { Result.Error(null) };
        yield return new object[1] { Result.Error(new Exception("0xBAD")) };

        yield return new object[1] { (Result)true };
        yield return new object[1] { (Result)false };

        yield return new object[1] { (Result)(new Exception("0xBAD")) };
    }


    [Theory]
    [MemberData(nameof(GetResults))]
    public void CannotBeOkAndError(Result result)
    {
        bool ok = result.IsOk();
        bool error = result.IsError();
        Assert.NotEqual(ok, error);
    }

    [Theory]
    [MemberData(nameof(GetResults))]
    public void CanAlwaysGetError(Result result)
    {
        if (result.IsError(out var error))
        {
            Assert.NotNull(error);
        }
    }
}