using System.Collections;
using System.Diagnostics.CodeAnalysis;
// ReSharper disable EqualExpressionComparison

namespace ScrubJay.Tests.Scratch;

[SuppressMessage("Assertions", "xUnit2025:The boolean assertion statement can be simplified")]
[SuppressMessage("Usage", "xUnit1042:The member referenced by the MemberData attribute returns untyped data rows")]
public class ResultTests
{
    public static MiscTheoryData Data => new()
    {
        Result<int, Exception>.Ok(147),
        Result<int, Exception>.Error(new Exception("BAD")),
        Result<Array?, Exception?>.Ok(null),
        Result<Array?, Exception?>.Error(null),
//        Result<BindingFlags>.Ok(BindingFlags.ExactBinding),
//        Result<Nothing>.Ok(default),
//        Result<string>.Ok("ABC"),
//        Result<EventArgs?>.Ok(null),
//        Result<int>.Error(null),
//        Result<int>.Error(new Exception("Bad")),
//        Result<BindingFlags>.Error(null),
//        Result<BindingFlags>.Error(new Exception("Bad")),
//        Result<Nothing>.Error(null),
//        Result<Nothing>.Error(new Exception("Bad")),
//        Result<string>.Error(null),
//        Result<string>.Error(new Exception("Bad")),
//        Result<EventArgs?>.Error(null),
//        Result<EventArgs?>.Error(new Exception("Bad")),
    };

    public static MiscTheoryData MultiData(int columns) => Data.Combinations(columns);
    
    
    
    [Fact]
    public void ImplicitOkWorks()
    {
        Result<int, Exception> r1 = 147;
        Assert.True(r1.IsOk(out var r1ok));
        Assert.Equal(147, r1ok);

        var nl = new List<int>();
        Result<List<int>?, InvalidOperationException?> r2 = nl;
        Assert.True(r2.IsOk(out var r2ok));
        Assert.Equal(nl, r2ok);
    }
    
    [Fact]
    public void ImplicitErrorWorks()
    {
        var ex1 = new Exception();
        Result<int, Exception> r1 = ex1;
        Assert.True(r1.IsError(out var r1error));
        Assert.Equal(ex1, r1error);

        var ex2 = new InvalidOperationException();
        Result<int, Exception> r2 = ex2;
        Assert.True(r2.IsError(out var r2error));
        Assert.Equal(ex2, r2error);
        
        var ex3 = new List<int>();
        Result<int, IEnumerable> r3 = ex3;
        Assert.True(r3.IsError(out var r3error));
        Assert.Equal(ex3, r3error);
        
        Result<byte, Exception?> r4 = null;
        Assert.True(r4.IsError(out var r4error));
        Assert.Null(r4error);
    }

    [Theory]
    [MemberData(nameof(Data))]
    public void ImplicitTrueWorks<TOk, TError>(Result<TOk, TError> result)
    {
        if (result)
        {
            Assert.True(result.IsOk());
        }
        else
        {
            Assert.True(result.IsError());
        }
    }
    
    [Theory]
    [MemberData(nameof(Data))]
    public void BitwiseNotWorks<TOk, TError>(Result<TOk, TError> result)
    {
        if (!result)
        {
            Assert.False(result.IsOk());
        }
        else
        {
            Assert.False(result.IsError());
        }
    }

    [Theory]
    [MemberData(nameof(Data))]
    public void BitwiseAndWorks<TOk, TError>(Result<TOk, TError> result)
    {
        if (result.IsOk())
        {
            Assert.True(result & result);
            Assert.True(result & true);
            Assert.True(true & result);
            Assert.False(result & false);
            Assert.False(false & result);
        }
        else
        {
            Assert.False(result & result);
            Assert.False(result & true);
            Assert.False(true & result);
            Assert.False(result & false);
            Assert.False(false & result);
        }
    }
    
    [Theory]
    [MemberData(nameof(Data))]
    public void BitwiseOrWorks<TOk, TError>(Result<TOk, TError> result)
    {
        if (result.IsOk())
        {
            Assert.True(result | result);
            Assert.True(result | true);
            Assert.True(true | result);
            Assert.True(result | false);
            Assert.True(false | result);
        }
        else
        {
            Assert.False(result | result);
            Assert.True(result | true);
            Assert.True(true | result);
            Assert.False(result | false);
            Assert.False(false | result);
        }
    }
    
    [Theory]
    [MemberData(nameof(Data))]
    public void BitwiseXorWorks<TOk, TError>(Result<TOk, TError> result)
    {
        if (result.IsOk())
        {
            Assert.False(result ^ result);
            Assert.False(result ^ true);
            Assert.False(true ^ result);
            Assert.True(result ^ false);
            Assert.True(false ^ result);
        }
        else
        {
            Assert.False(result ^ result);
            Assert.True(result ^ true);
            Assert.True(true ^ result);
            Assert.False(result ^ false);
            Assert.False(false ^ result);
        }
    }
    
    [Theory]
    [MemberData(nameof(Data))]
    public void OpEqualsWorks<TOk, TError>(Result<TOk, TError> result)
    {
        Assert.True(result == result);
        result.Match(ok =>
        {
            Assert.True(result == ok);
            Assert.True(ok == result);
            Assert.True(result == true);
            Assert.True(true == result);
            Assert.False(result == false);
            Assert.False(false == result);
        }, error =>
        {
            Assert.True(result == error);
            Assert.True(error == result);
            Assert.True(result == false);
            Assert.True(false == result);
            Assert.False(result == true);
            Assert.False(true == result);
        });
    }
    
    [Theory]
    [MemberData(nameof(Data))]
    public void OpNotEqualsWorks<TOk, TError>(Result<TOk, TError> result)
    {
        Assert.False(result != result);
        result.Match(ok =>
        {
            Assert.False(result != ok);
            Assert.False(ok != result);
            Assert.False(result != true);
            Assert.False(true != result);
            Assert.True(result != false);
            Assert.True(false != result);
        }, error =>
        {
            Assert.False(result != error);
            Assert.False(error != result);
            Assert.False(result != false);
            Assert.False(false != result);
            Assert.True(result != true);
            Assert.True(true != result);
        });
    }

    [Fact]
    public void OkWorks()
    {
        Result<int, Exception> r1 = Result<int, Exception>.Ok(147);
        Assert.True(r1.IsOk(out var r1ok));
        Assert.Equal(147, r1ok);
        Assert.False(r1.IsError());

        Result<byte, byte> r2 = Result<byte, byte>.Ok(147);
        Assert.True(r2.IsOk(out var r2ok));
        Assert.Equal(147, r2ok);
        Assert.False(r2.IsError());

        Result<List<int>?, Exception?> r3 = Result<List<int>?, Exception?>.Ok(null);
        Assert.True(r3.IsOk(out var r3ok));
        Assert.Null(r3ok);
        Assert.False(r3.IsError());
    }
    
    [Fact]
    public void ErrorWorks()
    {
        var ex = new InvalidOperationException("BAD");
        
        Result<int, Exception> r1 = Result<int, Exception>.Error(ex);
        Assert.True(r1.IsError(out var r1error));
        Assert.Equal(ex, r1error);
        Assert.False(r1.IsOk());

        Result<byte, byte> r2 = Result<byte, byte>.Error(147);
        Assert.True(r2.IsError(out var r2error));
        Assert.Equal(147, r2error);
        Assert.False(r2.IsOk());

        Result<List<int>?, Exception?> r3 = Result<List<int>?, Exception?>.Error(null);
        Assert.True(r3.IsError(out var r3error));
        Assert.Null(r3error);
        Assert.False(r3.IsOk());
    }
}


