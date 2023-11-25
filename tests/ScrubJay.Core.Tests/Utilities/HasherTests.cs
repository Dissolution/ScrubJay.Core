// ReSharper disable InconsistentNaming
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

//    [Theory]
//    [MemberData(nameof(TestData_1))]
//    public void HashOrderMatters<T>(T? value)
//    {
//        
//    }
}