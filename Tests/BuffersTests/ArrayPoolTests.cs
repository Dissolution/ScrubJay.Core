using System.Buffers;
using System.Reflection;
using ScrubJay.Functional;

namespace ScrubJay.Tests.BuffersTests;

public class ArrayPoolTests
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
        var arrayPoolType = typeof(ArrayPool<>).MakeGenericType(type);
        var sharedProperty = arrayPoolType.GetProperty(nameof(ArrayPool<char>.Shared), BindingFlags.Public | BindingFlags.Static);
        var rentMethod = sharedProperty!.PropertyType.GetMethod(nameof(ArrayPool<char>.Rent), BindingFlags.Public | BindingFlags.Instance);
        var sharedInstance = sharedProperty.GetValue(null);
        var sharedArray = rentMethod!.Invoke(sharedInstance, new object[1] { 1 });
        Array? array = sharedArray as Array;
        Assert.NotNull(array);
        int length = array.GetLength(0);
        Assert.Equal(16, length);
        Debug.WriteLine($"Type '{type}' has min capacity {length}");
    }
}