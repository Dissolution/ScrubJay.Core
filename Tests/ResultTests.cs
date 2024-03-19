//namespace ScrubJay.Tests;
//
///// <summary>
///// Tests for <see cref="ScrubJay.Result"/>
///// </summary>
//public class ResultTests
//{
//    private static Result[] TestResults { get; } = new Result[]
//    {
//        default(Result),
//        (Result)true,
//        (Result)false,
//        Result.Ok(),
//        Result.Error(null!),
//        Result.Error(new Exception("0xBAD")), 
//    };
//    
//    public static IEnumerable<object[]> SingleTestData() 
//        => TestHelper.ToMemberData<Result>(1,TestResults);
//
//    public static IEnumerable<object[]> DoubleTestData()
//        => TestHelper.Double<Result>(TestResults)!;
// 
//
//#region Operators
//
//#region implicit/explicit
//
//    [Fact]
//    public void CanImplicitlyCastFromBool()
//    {
//        Result trueResult = true;
//        Assert.True(trueResult.IsOk());
//
//        Result falseResult = false;
//        Assert.True(falseResult.IsError());
//    }
//
//    [Fact]
//    public void CanExplicitlyCastFromBool()
//    {
//        Result trueResult = (Result)true;
//        Assert.True(trueResult.IsOk());
//
//        Result falseResult = (Result)false;
//        Assert.True(falseResult.IsError());
//    }
//
//    [Fact]
//    public void CanImplicitlyCastFromException()
//    {
//        InvalidOperationException exception = new InvalidOperationException("BAD");
//        Result exResult = exception;
//        Assert.True(exResult.IsError(out var ex));
//        Assert.NotNull(ex);
//        Assert.True(object.ReferenceEquals(ex, exception));
//    }
//
//    [Fact]
//    public void CanExplicitlyCastFromException()
//    {
//        InvalidOperationException exception = new InvalidOperationException("BAD");
//        Result exResult = (Result)exception;
//        Assert.True(exResult.IsError(out var ex));
//        Assert.NotNull(ex);
//        Assert.True(object.ReferenceEquals(ex, exception));
//    }
//
//    [Theory]
//    [MemberData(nameof(SingleTestData))]
//    public void CanImplicitlyCastToBool(Result result)
//    {
//        bool b = result;
//        Assert.Equal(b, result.IsOk());
//        Assert.NotEqual(b, result.IsError());
//    }
//
//    [Theory]
//    [MemberData(nameof(SingleTestData))]
//    public void CanExplicitlyCastToBool(Result result)
//    {
//        bool b = (bool)result;
//        Assert.Equal(b, result.IsOk());
//        Assert.NotEqual(b, result.IsError());
//    }
//
//#endregion
//
//#region bitwise
//
//    [Theory]
//    [MemberData(nameof(SingleTestData))]
//    public void CanBitwiseNot(Result result)
//    {
//        var not = !result;
//        Assert.IsType<bool>(not);
//        Assert.Equal(not, result.IsError());
//        Assert.NotEqual(not, result.IsOk());
//    }
//    
//    [Theory]
//    [MemberData(nameof(SingleTestData))]
//    [Obsolete("ignore", false)]
//    public void CannotBitwiseNegate(Result result)
//    {
//        Assert.Throws<NotSupportedException>(() => ~result);
//    }
//
//    [Theory]
//    [MemberData(nameof(DoubleTestData))]
//    public void CanBitwiseOr(Result left, Result right)
//    {
//        var resultOr = left | right;
//        Assert.IsType<Result>(resultOr);
//        bool boolOr = ((bool)left) | ((bool)right);
//        Assert.Equal(boolOr, resultOr.IsOk());
//
//        var resultShortOr = left || right;
//        Assert.IsType<Result>(resultShortOr);
//        bool boolShortOr = ((bool)left) || ((bool)right);
//        Assert.Equal(boolShortOr, resultShortOr.IsOk());
//    }
//    
//    [Theory]
//    [MemberData(nameof(DoubleTestData))]
//    public void CanBitwiseAnd(Result left, Result right)
//    {
//        var resultAnd = left & right;
//        Assert.IsType<Result>(resultAnd);
//        bool boolAnd = ((bool)left) & ((bool)right);
//        Assert.Equal(boolAnd, resultAnd.IsOk());
//
//        var resultShortAnd = left && right;
//        Assert.IsType<Result>(resultShortAnd);
//        bool boolShortAnd = ((bool)left) && ((bool)right);
//        Assert.Equal(boolShortAnd, resultShortAnd.IsOk());
//    }
//    
//    [Theory]
//    [MemberData(nameof(DoubleTestData))]
//    public void CanBitwiseXor(Result left, Result right)
//    {
//        var resultXor = left ^ right;
//        Assert.IsType<Result>(resultXor);
//        bool boolXor = ((bool)left) ^ ((bool)right);
//        Assert.Equal(boolXor, resultXor.IsOk());
//    }
//#endregion
//
//#endregion
//
//
//    [Fact]
//    [Obsolete("", false)]
//    public void CanDealWithDefault()
//    {
//        Result alpha = default(Result);
//        Assert.True(alpha.IsError());
//
//        Result beta = new Result();
//        Assert.True(beta.IsError());
//    }
//
//
//    [Theory]
//    [MemberData(nameof(SingleTestData))]
//    public void CannotBeOkAndError(Result result)
//    {
//        bool ok = result.IsOk();
//        bool error = result.IsError();
//        Assert.NotEqual(ok, error);
//    }
//
//    [Theory]
//    [MemberData(nameof(SingleTestData))]
//    public void ErrorAlwaysHasException(Result result)
//    {
//        if (result.IsError(out var error))
//        {
//            Assert.NotNull(error);
//        }
//    }
//}