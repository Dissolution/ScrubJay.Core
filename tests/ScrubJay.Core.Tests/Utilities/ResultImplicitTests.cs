namespace ScrubJay.Tests.Utilities;

public class ImplicitTests
{
    [Fact]
    public void CanDealWithDefault()
    {
        Result alpha = default(Result);
        Assert.True(alpha.IsError());

        Result beta = new Result();
        Assert.True(beta.IsError());
    }
    
    [Fact]
    public void CanCastFromBool()
    {
        Result trueResult = (bool)true;
        Assert.Equal(Result.Ok(), trueResult);
        
        Result falseResult = (bool)false;
        Assert.NotEqual(Result.Ok(), falseResult);
    }
    
    [Fact]
    public void CanCastFromException()
    {
        Result nullErrorResult = (Exception?)(null);
        Assert.True(nullErrorResult.IsError());

        Result errorResult = (Exception?)(new Exception());
        Assert.True(errorResult.IsError());
    }
    
    [Fact]
    public void CanCastToBool()
    {
        bool ok = Result.Ok();
        Assert.True(ok);

        bool error = Result.Error(null);
        Assert.False(error);

        error = Result.Error(new Exception());
        Assert.False(error);
    }
}