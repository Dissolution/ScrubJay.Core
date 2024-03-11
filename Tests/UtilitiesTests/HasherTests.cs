using ScrubJay.Utilities;

// ReSharper disable InconsistentNaming
// ReSharper disable PossibleMultipleEnumeration
namespace ScrubJay.Tests.UtilitiesTests;

public class HasherTests
{
    /// <summary>
    /// Gets the data for any Theory test
    /// </summary>
    public static IEnumerable<object?[]> TypedData(int argCount)
    {
        return TheoryHelper.ShuffledCombinations(argCount, TheoryHelper.Data.Objects);
    }

    public static TheoryData<object?[]?> ArrayData()
    {
        var arrays = new List<object?[]>();
        
        var theoryObjects = TheoryHelper.Data.Objects;
        for (var i = 0; i < theoryObjects.Length; i++)
        {
            TheoryHelper.Shuffle<object?>(theoryObjects);
            var array = new object?[i];
            theoryObjects.AsSpan(0, i).CopyTo(array);
            arrays.Add(array);
        }
        return new(arrays);
    }

    [Theory]
    [MemberData(nameof(TypedData), 1)]
    public void CanCombine1<T>(T? value)
    {
        var hash = Hasher.GetHashCode<T>(value);
        Assert.NotEqual(0, hash);
    }
    
    [Theory]
    [MemberData(nameof(TypedData), 2)]
    public void CanCombine2<T1, T2>(T1? first, T2? second)
    {
        var hash = Hasher.Combine<T1, T2>(first, second);
        Assert.NotEqual(0, hash);
    }
    
    [Theory]
    [MemberData(nameof(TypedData), 3)]
    public void CanCombine3<T1, T2, T3>(T1? first, T2? second, T3? third)
    {
        var hash = Hasher.Combine<T1, T2, T3>(first, second, third);
        Assert.NotEqual(0, hash);
    }
    
    [Theory]
    [MemberData(nameof(TypedData), 4)]
    public void CanCombine4<T1, T2, T3, T4>(T1? first, T2? second, T3? third, T4? fourth)
    {
        var hash = Hasher.Combine<T1, T2, T3, T4>(first, second, third, fourth);
        Assert.NotEqual(0, hash);
    }

    [Theory]
    [MemberData(nameof(TypedData), 5)]
    public void CanCombine5<T1, T2, T3, T4, T5>(T1? first, T2? second, T3? third, T4? fourth, T5? fifth)
    {
        var hash = Hasher.Combine<T1, T2, T3, T4, T5>(first, second, third, fourth, fifth);
        Assert.NotEqual(0, hash);
    }

    [Theory]
    [MemberData(nameof(TypedData), 6)]
    public void CanCombine6<T1, T2, T3, T4, T5, T6>(T1? first, T2? second, T3? third, T4? fourth, T5? fifth, T6? sixth)
    {
        var hash = Hasher.Combine<T1, T2, T3, T4, T5, T6>(first, second, third, fourth, fifth, sixth);
        Assert.NotEqual(0, hash);
    }

    [Theory]
    [MemberData(nameof(TypedData), 7)]
    public void CanCombine7<T1, T2, T3, T4, T5, T6, T7>(T1? first, T2? second, T3? third, T4? fourth, T5? fifth, T6? sixth, T7? seventh)
    {
        var hash = Hasher.Combine<T1, T2, T3, T4, T5, T6, T7>(first, second, third, fourth, fifth, sixth, seventh);
        Assert.NotEqual(0, hash);
    }

    [Theory]
    [MemberData(nameof(TypedData), 8)]
    public void CanCombine8<T1, T2, T3, T4, T5, T6, T7, T8>(T1? first, T2? second, T3? third, T4? fourth, T5? fifth, T6? sixth, T7? seventh, T8? eighth)
    {
        var hash = Hasher.Combine<T1, T2, T3, T4, T5, T6, T7, T8>(first, second, third, fourth, fifth, sixth, seventh, eighth);
        Assert.NotEqual(0, hash);
    }

    [Theory]
    [MemberData(nameof(TypedData), 9)]
    public void CanCombine9<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1? first, T2? second, T3? third, T4? fourth, T5? fifth, T6? sixth, T7? seventh, T8? eighth, T9? ninth)
    {
        var hash = Hasher.Combine<object?>(first, second, third, fourth, fifth, sixth, seventh, eighth, ninth);
        Assert.NotEqual(0, hash);
    }

    [Theory]
    [MemberData(nameof(ArrayData))]
    public void CanCombineArray(object?[]? array)
    {
        var hash = Hasher.Combine<object?>(array);
        Assert.NotEqual(0, hash);
    }
    
    [Theory]
    [MemberData(nameof(TypedData), 1)]
    public void GetHashCodeIsConsistent<T>(T? value)
    {
        var alpha = Hasher.GetHashCode(value);
        var beta = Hasher.GetHashCode(value);
        Assert.Equal(alpha, beta);
    }
    
    [Theory]
    [MemberData(nameof(TypedData), 1)]
    public void GetHashCodeIsTheSameAsAdd<T>(T? value)
    {
        var alpha = Hasher.GetHashCode(value);
        var hasher = new Hasher();
        hasher.Add<T>(value);
        var beta = hasher.ToHashCode();
        Assert.Equal(alpha, beta);
    }
    
    [Theory]
    [MemberData(nameof(ArrayData))]
    public void CombineIsConsistent(object?[]? array)
    {
        var alpha = Hasher.Combine<object?>(array);
        var beta = Hasher.Combine<object?>(array);
        Assert.Equal(alpha, beta);
    }

    [Theory]
    [MemberData(nameof(ArrayData))]
    public void HashOrderMatters(object?[]? array)
    {
        var copy = TheoryHelper.ReverseShallowCopy<object?>(array);
        Assert.True(array is null == copy is null);
        int alpha = Hasher.Combine<object?>(array);
        int beta = Hasher.Combine<object?>(copy);
        if (array is null || array.Length == 1)
        {
            Assert.Equal(alpha, beta);
        }
        else
        {
            if (array.SequenceEqual<object?>(copy!))
            {
                Assert.Equal(alpha, beta);
            }
            else
            {
                Assert.NotEqual(alpha, beta);
            }
        }
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
        var theoryObjects = TheoryHelper.Data.Objects;

        IEnumerable<object?> dataEnumerable = theoryObjects.AsEnumerable();
        object?[] dataArray = theoryObjects.ToArray();
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
        for (var i = 0; i < theoryObjects.Length; i++)
        {
            hasher.Add<object?>(theoryObjects[i]);
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

