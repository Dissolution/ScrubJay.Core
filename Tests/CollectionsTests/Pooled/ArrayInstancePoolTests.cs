using System.Reflection;
using ScrubJay.Functional;
using ScrubJay.Pooling;

namespace ScrubJay.Tests.CollectionsTests.Pooled;

public class ArrayInstancePoolTests
{
    public static TheoryData<Type> TestTypes { get; } = new()
    {
        typeof(char),
        typeof(bool),
        typeof(byte),
        typeof(Guid),
        typeof(int),
        typeof(ulong),
        typeof(None),
        typeof(object),
        typeof(string),
        typeof(List<int>),
    };


    [Theory]
    [MemberData(nameof(TestTypes))]
    public void PoolMinCapacityIsGreaterThanZero(Type type)
    {
        var arrayPoolType = typeof(ArrayInstancePool<>).MakeGenericType(type);
        var sharedProperty = arrayPoolType.GetProperty(nameof(ArrayInstancePool<char>.Shared), BindingFlags.Public | BindingFlags.Static);
        var rentMethod = sharedProperty!.PropertyType
            .GetMethod(
                nameof(ArrayInstancePool<char>.Rent),
                BindingFlags.Public | BindingFlags.Instance,
                null, CallingConventions.Any, [typeof(int)], null);
        var sharedInstance = sharedProperty.GetValue(null);
        var sharedArray = rentMethod!.Invoke(sharedInstance, new object[1] { 1 });
        Array? array = sharedArray as Array;
        Assert.NotNull(array);
        int length = array.GetLength(0);
        Assert.True(length is (16 or 64));
        Debug.WriteLine($"Type '{type}' has min capacity {length}");
    }
}
