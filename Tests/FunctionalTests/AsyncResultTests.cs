// ReSharper disable InconsistentNaming
#pragma warning disable CA1510, CS1998

using ScrubJay.Functional;

namespace ScrubJay.Tests.FunctionalTests;

public class AsyncResultTests
{
    private static Result<double> ParseDouble(string? str)
        => Result.Try(() => double.Parse(str!));

    private static Result<double> DivideDoubles(double numerator, double denominator)
    {
        if (denominator == 0d)
            return new DivideByZeroException();
        return Result<double>.Ok(numerator / denominator);
    }


    [Fact]
    public async Task AsyncWorks()
    {
        double f64 = await ParseDouble("3.14159");
        Assert.Equal(3.14159d, f64);
    }

    [Fact]
    public async Task ComplexAsyncWorks()
    {
        double a = await ParseDouble("11");
        double b = await ParseDouble("4");
        double div = await DivideDoubles(a, b);
        Assert.Equal(11d / 4d, div);
    }


    [Fact]
    public async Task EarlyReturnWorks()
    {
        var task = Task.Run(async () =>
        {
            double a = await ParseDouble("abc");
            Assert.Fail("Should have returned already");
            double b = await ParseDouble("4");
            double div = await DivideDoubles(a, b);
            Assert.Equal(11d / 4d, div);
            return div;
        });
        await Assert.ThrowsAsync<FormatException>(async () => await task);
        Assert.True(task.IsFaulted);
        Assert.NotNull(task.Exception);
        Assert.Single(task.Exception.InnerExceptions);
        Assert.IsType<FormatException>(task.Exception.InnerExceptions[0]);
    }

    [Fact]
    public async Task AsReturnTypeWorks()
    {
        double f64 = await localParseAsync("3.14159");
        Assert.Equal(3.14159d, f64);
        return;


        static async Result<double> localParseAsync(string? str)
        {
            return await ParseDouble(str);
        }
    }

    [Fact]
    public async Task AsEarlyReturnUniversalWorks()
    {
        Result<double> result = tryParse(null);
        Assert.True(result.IsError(out var ex));
        Assert.IsType<ArgumentNullException>(ex);

        result = tryParse("3.14159");
        Assert.True(result.IsOk(out var ok));
        Assert.Equal(3.14159d, ok);

        await Task.Delay(1, TestContext.Current.CancellationToken);
        return;


        static async Result<double> tryParse(string? str)
        {
            if (str is null)
                throw new ArgumentNullException(nameof(str));
            return double.Parse(str);
        }
    }

    [Fact]
    public async Task AsEarlyReturnUniversalOnlyWorksWithAsync()
    {
        Result<double> result = default!;
        Assert.Throws<ArgumentNullException>(() =>
        {
            result = tryParse(null);
        });
        Assert.Equal(default, result);
        Assert.True(result.IsError(out var ex));
        Assert.Null(ex);

        result = tryParse("3.14159");
        Assert.True(result.IsOk(out var ok));
        Assert.Equal(3.14159d, ok);

        await Task.Delay(1, TestContext.Current.CancellationToken);
        return;


        static Result<double> tryParse(string? str)
        {
            if (str is null)
                throw new ArgumentNullException(nameof(str));
            return Result<double>.Ok(double.Parse(str));
        }
    }
}
