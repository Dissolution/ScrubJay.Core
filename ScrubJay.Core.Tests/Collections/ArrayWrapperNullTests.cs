using ScrubJay.Collections;

namespace ScrubJay.Tests.Collections;

public class ArrayWrapperNullTests
{
    public static IEnumerable<object?[]> GetTestData()
        => TestHelper.ToNullableMemberData<object?>(1, Guid.NewGuid(), DateTime.UtcNow, null);


    [Fact]
    public void CanGetNullObject()
    {
        object?[] objectArray = new object?[3] { Guid.NewGuid(), DateTime.UtcNow, null };
        Array array = objectArray;
        ArrayWrapper<object?> arrayWrapper = new(array);

        object? first = arrayWrapper[0];
        Assert.NotNull(first);
        Assert.Equal(first, objectArray[0]);
        object? second = arrayWrapper[1];
        Assert.NotNull(second);
        Assert.Equal(second, objectArray[1]);
        object? third = arrayWrapper[2];
        Assert.Null(third);
        Assert.Equal(third, objectArray[2]);
    }

    [Fact]
    public void CanGetNullableInt()
    {
        int?[] nullableIntArray = new int?[3] { 147, 13, null };
        Array array = nullableIntArray;
        ArrayWrapper<int?> arrayWrapper = new(array);

        int? first = arrayWrapper[0];
        Assert.NotNull(first);
        Assert.Equal(first, nullableIntArray[0]);
        int? second = arrayWrapper[1];
        Assert.NotNull(second);
        Assert.Equal(second, nullableIntArray[1]);
        int? third = arrayWrapper[2];
        Assert.Null(third);
        Assert.Equal(third, nullableIntArray[2]);
    }

    [Fact]
    public void CanSetNullObject()
    {
        object?[] objectArray = new object?[2] { Guid.NewGuid(), DateTime.UtcNow };
        Array array = objectArray;
        ArrayWrapper<object?> arrayWrapper = new(array);

        object? first = arrayWrapper[0];
        Assert.NotNull(first);
        Assert.Equal(first, objectArray[0]);
        arrayWrapper[0] = null;
        Assert.Null(objectArray[0]);

        first = arrayWrapper[0];
        Assert.Null(first);
        Assert.Equal(first, objectArray[0]);
    }

    [Fact]
    public void CanSetNullableInt()
    {
        int?[] nullableIntArray = new int?[2] { 147, 13 };
        Array array = nullableIntArray;
        ArrayWrapper<int?> arrayWrapper = new(array);

        int? first = arrayWrapper[0];
        Assert.NotNull(first);
        Assert.Equal(first, nullableIntArray[0]);
        arrayWrapper[0] = null;
        Assert.Null(nullableIntArray[0]);

        first = arrayWrapper[0];
        Assert.Null(first);
        Assert.Equal(first, nullableIntArray[0]);
    }
}