// ReSharper disable InconsistentNaming

using System.ComponentModel;
using System.Reflection;

namespace ScrubJay.Tests.Utilities;

public class HasherTests
{
    public static IEnumerable<object?[]> TestData_1()
    {
        foreach (object? obj in TestHelper.TestObjects)
        {
            yield return new object?[1] { obj };
        }
    }
    public static IEnumerable<object?[]> TestData(int len)
    {
        var infData = TestHelper.InfiniteObjects.Value!;
        int total = TestHelper.TestObjects.Count;
        total = (total * total);
        for (var t = 0; t < total; t++)
        {
            object?[] objArray = infData.PopArray(len);
            yield return objArray;
        }
    }

    [Theory]
    [MemberData(nameof(TestData_1))]
    public int CanGetHashCode<T>(T? value)
    {
        return Hasher.GetHashCode<T>(value);
    }
    
    [Theory]
    [MemberData(nameof(TestData), 2)]
    public int CanCombine<T1, T2>(T1? first, T2? second)
    {
        return Hasher.Combine<T1, T2>(first, second);
    }
    

    [Theory]
    [MemberData(nameof(TestData_1))]
    public void GetHashCodeIsConsistent<T>(T? value)
    {
        int? firstResult = null;
        for (var i = 0; i < 10; i++)
        {
            if (firstResult is null)
            {
                firstResult = Hasher.GetHashCode<T>(value);
            }
            else
            {
                var result = Hasher.GetHashCode<T>(value);
                Assert.Equal(firstResult, result);
            }
        }
    }

    [Fact]
    public void CanHashNullArray()
    {
        object?[]? nullArray = (object?[]?)null;
        int hash = Hasher.Combine<object?>(nullArray);
        Assert.Equal(0, hash);
    }
    
    [Fact]
    public void CanHashArrays()
    {
        var infData = TestHelper.InfiniteObjects.Value!;
        for (var len = 0; len <= 16; len++)
        {
            object?[] objArray = infData.PopArray(len);
            int hash = Hasher.Combine<object?>(objArray);
            Assert.NotEqual(0, hash);
        }
    }

    [Theory]
    [MemberData(nameof(TestData), 2)]
    public void HashOrderMatters<T1, T2>(T1? first, T2? second)
    {
        int alpha = Hasher.Combine<T1, T2>(first, second);
        int beta = Hasher.Combine<T2, T1>(second, first);
        Assert.NotEqual(alpha, beta);
    }

    [Fact]
    public void NullHashIsDifferentThanEmptyHash()
    {
        var hasher = new Hasher();
        int alpha = hasher.ToHashCode();

        int beta = Hasher.GetHashCode<object>(null);
        Assert.NotEqual(alpha, beta);

        int gamma = Hasher.Combine<object>((object[]?)null);
        Assert.NotEqual(alpha, gamma);
        Assert.Equal(beta, gamma);

        int delta = Hasher.Combine<object>((object[]?)null, new BadComparer<object>());
        Assert.NotEqual(alpha, delta);
        Assert.Equal(beta, delta);
        Assert.Equal(gamma, delta);
    }

    [Fact]
    public void NullHashesTheSame()
    {
        var hasher = new Hasher();
        hasher.Add<object>(null);
        int alpha = hasher.ToHashCode();

        int beta = Hasher.GetHashCode<object>(null);
        Assert.Equal(alpha, beta);

        int gamma = Hasher.Combine<object>((object[]?)null);
        Assert.Equal(alpha, gamma);
        Assert.Equal(beta, gamma);

        int delta = Hasher.Combine<object>((object[]?)null, new BadComparer<object>());
        Assert.Equal(alpha, delta);
        Assert.Equal(beta, delta);
        Assert.Equal(gamma, delta);
    }

    [Fact]
    public void AllWaysToCombineHashesAreTheSame()
    {
        int a = 147;
        string b = "ABC";
        List<byte>? c = null;
        
        var hasher = new Hasher();
        hasher.Add(a);
        hasher.Add(b);
        hasher.Add(c);
        int alpha = hasher.ToHashCode();

        int beta = Hasher.Combine(a, b, c);

        Assert.Equal(alpha, beta);

        int gamma = Hasher.Combine<object?>(a, b, c);

        Assert.Equal(alpha, gamma);
    }
}

internal sealed class BadComparer<T> : IEqualityComparer<T>
{
    public bool Equals(T? x, T? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;
        return EqualityComparer<T>.Default.Equals(x, y);
    }

    public int GetHashCode(T? obj)
    {
        if (obj is null)
            throw new ArgumentNullException(nameof(obj));
        return obj.GetHashCode();
    }
}