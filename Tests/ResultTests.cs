using System.Collections;
using System.Reflection;
using ScrubJay.Functional;
using NIExResult = ScrubJay.Functional.Result<int?, System.Exception?>;

// ReSharper disable VariableCanBeNotNullable

namespace ScrubJay.Tests;

public class ResultTests
{
    public static MiscTheoryData ResultsData { get; } = new MiscTheoryData()
    {
        Result<int, Exception?>.Ok(147),
        Result<BindingFlags, Exception?>.Ok(BindingFlags.ExactBinding),
        Result<Unit, Exception?>.Ok(default),
        Result<string, Exception?>.Ok("ABC"),
        Result<EventArgs?, Exception?>.Ok(null),
        Result<int, Exception?>.Error(null),
        Result<int, Exception?>.Error(new Exception("Bad")),
        Result<BindingFlags, Exception?>.Error(null),
        Result<BindingFlags, Exception?>.Error(new Exception("Bad")),
        Result<Unit, Exception?>.Error(null),
        Result<Unit, Exception?>.Error(new Exception("Bad")),
        Result<string, Exception?>.Error(null),
        Result<string, Exception?>.Error(new Exception("Bad")),
        Result<EventArgs?, Exception?>.Error(null),
        Result<EventArgs?, Exception?>.Error(new Exception("Bad")),
    };


    [Theory]
    [MemberData(nameof(ResultsData))]
    public void CannotBeOkAndError<T>(Result<T, Exception?> result)
    {
        bool ok = result.IsOk();
        bool error = result.IsError();
        Assert.NotEqual(ok, error);
    }

    [Fact]
    public void DefaultIsError()
    {
        Result<object?, object> result;

        result = default;
        Assert.False(result);

        result = new Result<object?, object>();
        Assert.False(result);

        result = Activator.CreateInstance<Result<object?, object?>>()!;
        Assert.False(result);
    }

    [Fact]
    public void OkAndErrorDoNotGetConfused()
    {
        // Obj?, Obj?
        {
            Result<object?, object?> result;

            result = Result<object?, object?>.Ok(null);
            Assert.True(result);

            result = Result<object?, object?>.Ok(147);
            Assert.True(result);

            result = Result<object?, object?>.Ok(new Exception("Bad"));
            Assert.True(result);

            result = Result<object?, object?>.Error(null);
            Assert.False(result);

            result = Result<object?, object?>.Error(147);
            Assert.False(result);

            result = Result<object?, object?>.Error(new Exception("Bad"));
            Assert.False(result);
        }

        // int?, ex?
        {
            Result<int?, Exception?> result;

            result = NIExResult.Ok(null);
            Assert.True(result);

            result = NIExResult.Error(null);
            Assert.False(result);
        }

        // ie, iet
        {
            Result<IEnumerable?, IEnumerable<int>?> result;

            result = Result<IEnumerable?, IEnumerable<int>?>.Ok(null);
            Assert.True(result);

            result = Result<IEnumerable?, IEnumerable<int>?>.Ok(new int[1, 4, 7]);
            Assert.True(result);

            result = Result<IEnumerable?, IEnumerable<int>?>.Error(null);
            Assert.False(result);

            result = Result<IEnumerable?, IEnumerable<int>?>.Error(
                new List<int>
                {
                    1,
                    4,
                    7,
                });
            Assert.False(result);
        }
    }

    [Fact]
    public void CanImplicitlyCastFromT()
    {
        Result<object?, Exception> objectResult = (object?)null;
        Assert.True(objectResult.IsOk());
        objectResult = new object();
        Assert.True(objectResult.IsOk());

        int? nullNullableInt = null;
        Result<int?, Exception> nullableResult = nullNullableInt;
        Assert.True(nullableResult.IsOk());
        int? nonnullNullableInt = 147;
        nullableResult = nonnullNullableInt;
        Assert.True(nullableResult.IsOk());

        byte b = 255;
        Result<byte, Exception> byteResult = b;
        Assert.True(byteResult.IsOk());

        BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;
        Result<BindingFlags, Exception> bindingFlagsResult = bindingFlags;
        Assert.True(bindingFlagsResult.IsOk());

        string? nullString = null;
        Result<string?, Exception> stringResult = nullString;
        Assert.True(stringResult.IsOk());
        string? nonnullString = "abc";
        Result<string?, Exception> nonnullResultString = nonnullString;
        Assert.True(nonnullResultString.IsOk());

        int[] numbers = [1, 4, 7];
        Result<IEnumerable<int>, Exception> numbersResult = numbers;
        Assert.True(numbersResult.IsOk());
        numbersResult = new List<int>
        {
            1,
            2,
            3,
            4,
            5
        };
        Assert.True(numbersResult.IsOk());
    }


    [Theory]
    [MemberData(nameof(ResultsData))]
    public void CanImplicitlyCastToBool<T>(Result<T, Exception?> result)
    {
        bool b = result;
        Assert.Equal(b, result.IsOk());
        Assert.NotEqual(b, result.IsError());
    }

    [Theory]
    [MemberData(nameof(ResultsData))]
    public void CanEnumerate<T>(Result<T, Exception?> result)
    {
        using var e = result.GetEnumerator();
        if (result.IsOk(out var ok))
        {
            Assert.True(e.MoveNext());
            Assert.Equal<T>(ok, e.Current);
            Assert.False(e.MoveNext());
        }
        else if (result.IsError())
        {
            Assert.False(e.MoveNext());
        }
        else
        {
            Assert.Fail();
        }
    }

    [Fact]
    public void ImplicitOkWorks()
    {
        Result<int, Exception> r1 = 147;
        Assert.True(r1.IsOk(out var r1Ok));
        Assert.Equal(147, r1Ok);

        var nl = new List<int>();
        Result<List<int>?, InvalidOperationException?> r2 = nl;
        Assert.True(r2.IsOk(out var r2Ok));
        Assert.Equal(nl, r2Ok);
    }

    [Fact]
    public void ImplicitErrorWorks()
    {
        var ex1 = new Exception();
        Result<int, Exception> r1 = ex1;
        Assert.True(r1.IsError(out var r1Error));
        Assert.Equal(ex1, r1Error);

        var ex2 = new InvalidOperationException();
        Result<int, Exception> r2 = ex2;
        Assert.True(r2.IsError(out var r2Error));
        Assert.Equal(ex2, r2Error);

        var ex3 = new List<int>();
        Result<int, IEnumerable> r3 = ex3;
        Assert.True(r3.IsError(out var r3Error));
        Assert.Equal(ex3, r3Error);

        Result<byte, Exception?> r4 = null;
        Assert.True(r4.IsError(out var r4Error));
        Assert.Null(r4Error);
    }

    [Theory]
    [MemberData(nameof(ResultsData))]
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


#pragma warning disable S1125
    [Theory]
    [MemberData(nameof(ResultsData))]
    public void OpEqualsWorks<TOk, TError>(Result<TOk, TError> result)
    {
        // ReSharper disable once EqualExpressionComparison
        Assert.True(result == result);
        if (result.IsOk(out var ok))
        {
            Assert.True(result == ok);
            Assert.True(ok == result);
            Assert.True(result == true);
            Assert.True(true == result);
            Assert.False(result == false);
            Assert.False(false == result);
        }
        else
        {
            var isError = result.IsError(out var error);
            Assert.True(isError);
            Assert.True(result == error!);
            Assert.True(error! == result);
            Assert.True(result == false);
            Assert.True(false == result);
            Assert.False(result == true);
            Assert.False(true == result);
        }
    }

    [Theory]
    [MemberData(nameof(ResultsData))]
    public void OpNotEqualsWorks<TOk, TError>(Result<TOk, TError> result)
    {
        // ReSharper disable once EqualExpressionComparison
        Assert.False(result != result);
        if (result.IsOk(out var ok))
        {
            Assert.False(result != ok);
            Assert.False(ok != result);
            Assert.False(result != true);
            Assert.False(true != result);
            Assert.True(result != false);
            Assert.True(false != result);
        }
        else
        {
            var isError = result.IsError(out var error);
            Assert.True(isError);
            Assert.False(result != error!);
            Assert.False(error! != result);
            Assert.False(result != false);
            Assert.False(false != result);
            Assert.True(result != true);
            Assert.True(true != result);
        }
    }
#pragma warning restore S1125

    [Fact]
    public void OkWorks()
    {
        Result<int, Exception> r1 = Result<int, Exception>.Ok(147);
        Assert.True(r1.IsOk(out var r1Ok));
        Assert.Equal(147, r1Ok);
        Assert.False(r1.IsError());

        Result<byte, byte> r2 = Result<byte, byte>.Ok(147);
        Assert.True(r2.IsOk(out var r2Ok));
        Assert.Equal(147, r2Ok);
        Assert.False(r2.IsError());

        Result<List<int>?, Exception?> r3 = Result<List<int>?, Exception?>.Ok(null);
        Assert.True(r3.IsOk(out var r3Ok));
        Assert.Null(r3Ok);
        Assert.False(r3.IsError());
    }

    [Fact]
    public void ErrorWorks()
    {
        var ex = new InvalidOperationException("BAD");

        Result<int, Exception> r1 = Result<int, Exception>.Error(ex);
        Assert.True(r1.IsError(out var r1Error));
        Assert.Equal(ex, r1Error);
        Assert.False(r1.IsOk());

        Result<byte, byte> r2 = Result<byte, byte>.Error(147);
        Assert.True(r2.IsError(out var r2Error));
        Assert.Equal(147, r2Error);
        Assert.False(r2.IsOk());

        Result<List<int>?, Exception?> r3 = Result<List<int>?, Exception?>.Error(null);
        Assert.True(r3.IsError(out var r3Error));
        Assert.Null(r3Error);
        Assert.False(r3.IsOk());
    }
}