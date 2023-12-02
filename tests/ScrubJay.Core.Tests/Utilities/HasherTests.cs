// ReSharper disable InconsistentNaming

namespace ScrubJay.Tests.Utilities;

public class HasherTests
{
    /// <summary>
    /// Gets the data for any Theory test
    /// </summary>
    public static IEnumerable<object?[]> Data(int arrayLen)
    {
        return TestHelper.ToEnumerableNullableObjects(TestHelper.TestObjects, arrayLen);
    }

    [Theory]
    [MemberData(nameof(Data), 1)]
    public int CanCombine1<T>(T? value)
    {
        return Hasher.GetHashCode<T>(value);
    }
    
    [Theory]
    [MemberData(nameof(Data), 2)]
    public int CanCombine2<T1, T2>(T1? first, T2? second)
    {
        return Hasher.Combine<T1, T2>(first, second);
    }
    
    [Theory]
    [MemberData(nameof(Data), 3)]
    public int CanCombine3<T1, T2, T3>(T1? first, T2? second, T3? third)
    {
        return Hasher.Combine<T1, T2, T3>(first, second, third);
    }
    
    [Theory]
    [MemberData(nameof(Data), 4)]
    public int CanCombine4<T1, T2, T3, T4>(T1? first, T2? second, T3? third, T4? fourth)
    {
        return Hasher.Combine<T1, T2, T3, T4>(first, second, third, fourth);
    }

    [Theory]
    [MemberData(nameof(Data), 5)]
    public int CanCombine5<T1, T2, T3, T4, T5>(T1? first, T2? second, T3? third, T4? fourth, T5? fifth)
    {
        return Hasher.Combine<T1, T2, T3, T4, T5>(first, second, third, fourth, fifth);
    }

    [Theory]
    [MemberData(nameof(Data), 6)]
    public int CanCombine6<T1, T2, T3, T4, T5, T6>(T1? first, T2? second, T3? third, T4? fourth, T5? fifth, T6? sixth)
    {
        return Hasher.Combine<T1, T2, T3, T4, T5, T6>(first, second, third, fourth, fifth, sixth);
    }

    [Theory]
    [MemberData(nameof(Data), 7)]
    public int CanCombine7<T1, T2, T3, T4, T5, T6, T7>(T1? first, T2? second, T3? third, T4? fourth, T5? fifth, T6? sixth, T7? seventh)
    {
        return Hasher.Combine<T1, T2, T3, T4, T5, T6, T7>(first, second, third, fourth, fifth, sixth, seventh);
    }

    [Theory]
    [MemberData(nameof(Data), 8)]
    public int CanCombine8<T1, T2, T3, T4, T5, T6, T7, T8>(T1? first, T2? second, T3? third, T4? fourth, T5? fifth, T6? sixth, T7? seventh, T8? eighth)
    {
        return Hasher.Combine<T1, T2, T3, T4, T5, T6, T7, T8>(first, second, third, fourth, fifth, sixth, seventh, eighth);
    }

    [Theory]
    [MemberData(nameof(Data), 9)]
    public int CanCombine9<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1? first, T2? second, T3? third, T4? fourth, T5? fifth, T6? sixth, T7? seventh, T8? eighth, T9? ninth)
    {
        return Hasher.Combine<object?>(first, second, third, fourth, fifth, sixth, seventh, eighth, ninth);
    }
    
    [Theory]
    [MemberData(nameof(Data), 1)]
    public void GetHashCodeIsConsistent<T>(T? value)
    {
        var alpha = Hasher.GetHashCode(value);
        var beta = Hasher.GetHashCode(value);
        Assert.Equal(alpha, beta);
    }
    
    [Theory]
    [MemberData(nameof(Data), 3)]
    public void CombineIsConsistent<T1, T2, T3>(T1? value1, T2? value2, T3? value3)
    {
        var alpha = Hasher.Combine(value1, value2, value3);
        var beta = Hasher.Combine(value1, value2, value3);
        Assert.Equal(alpha, beta);
    }

    [Theory]
    [MemberData(nameof(Data), 2)]
    public void HashOrderMatters<T1, T2>(T1? first, T2? second)
    {
        int alpha = Hasher.Combine<T1, T2>(first, second);
        int beta = Hasher.Combine<T2, T1>(second, first);
        Assert.NotEqual(alpha, beta);
    }
    
    [Fact]
    public void NullHashingIsConsistent()
    {
        int alpha = Hasher.GetHashCode<object?>(null);
        int beta = Hasher.GetHashCode<int?>(null);
        Assert.Equal(alpha, beta);
        int gamma = Hasher.GetHashCode<string>((string)null!);
        Assert.Equal(alpha, gamma);
        int delta = Hasher.Combine<object?>((object?[]?)null);
        Assert.Equal(alpha, delta);
        int epsilon = Hasher.Combine<object?>((IEnumerable<object?>)null!);
        Assert.Equal(alpha, epsilon);
    }
    
    [Fact]
    public void EmptyHashingIsConsistent()
    {
        int alpha = new Hasher().ToHashCode();
        int beta = Hasher.Combine<object?>(ReadOnlySpan<object?>.Empty);
        Assert.Equal(alpha, beta);
        int gamma = Hasher.Combine<object?>(Array.Empty<object?>());
        Assert.Equal(alpha, gamma);
        int delta = Hasher.Combine<object?>(Enumerable.Empty<object?>());
        Assert.Equal(alpha, delta);
    }
   
    [Fact]
    public void NullHashIsDifferentThanEmptyHash()
    {
        var emptyHash = new Hasher().ToHashCode();
        var nullHash = Hasher.GetHashCode<object?>(null);
        Assert.NotEqual(emptyHash, nullHash);
    }
    
    [Fact]
    public void AllWaysToCombineHashesAreTheSame()
    {
        var data = TestHelper.TestObjects;

        IEnumerable<object?> dataEnumerable = data.AsEnumerable();
        object?[] dataArray = data.ToArray();
        Span<object?> dataSpan = dataArray.AsSpan();

        Hasher hasher;
        
        hasher = new Hasher();
        hasher.AddMany<object?>(dataSpan);
        int alpha = hasher.ToHashCode();

        hasher = new Hasher();
        hasher.AddMany<object?>(dataArray);
        int beta = hasher.ToHashCode();
        Assert.Equal(alpha, beta);

        hasher = new Hasher();
        hasher.AddMany<object?>(dataEnumerable);
        int gamma = hasher.ToHashCode();
        Assert.Equal(alpha, gamma);
        
        hasher = new Hasher();
        for (var i = 0; i < data.Count; i++)
        {
            hasher.Add<object?>(data[i]);
        }
        int delta = hasher.ToHashCode();
        Assert.Equal(alpha, delta);

        int epsilon = Hasher.Combine<object?>(dataSpan);
        Assert.Equal(alpha, epsilon);
        
        int zeta = Hasher.Combine<object?>(dataArray);
        Assert.Equal(alpha, zeta);
        
        int eta = Hasher.Combine<object?>(dataEnumerable);
        Assert.Equal(alpha, eta);

    }
}

