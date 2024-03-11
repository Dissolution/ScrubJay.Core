namespace ScrubJay.Tests;

public static class TestHelper
{

    
    static TestHelper()
    {
    }


    private static IEnumerable<object?[]> SingleMemberDataIterator<T>(T?[] items)
    {
        var itemCount = items.Length;

        for (var i = 0; i < itemCount; i++)
        {
            yield return new object?[1] { (object?)items[i] };
        }
    }

    private static IEnumerable<object?[]> DoubleMemberDataIterator<T>(T?[] items)
    {
        var itemCount = items.Length;
        if (itemCount < 2)
            yield break;

        int offset = 0;
        while ((offset + 2) <= itemCount)
        {
            object?[] objects = new object?[2]
            {
                (object?)items[offset],
                (object?)items[offset + 1]
            };
            yield return objects;
            offset += 2;
        }
    }

    private static IEnumerable<object?[]> MemberDataIterator<T>(int size, T?[] items)
    {
        var itemCount = items.Length;
        if (itemCount < size)
            yield break;

        int offset = 0;
        while ((offset + size) <= itemCount)
        {
            object?[] objects = new object?[size];
            for (var i = 0; i < size; i++)
            {
                objects[i] = (object?)items[offset + i];
            }

            yield return objects;
            offset += size;
        }
    }

    public static IEnumerable<object[]> ToMemberData<T>(int itemsPerArray, params T?[] items)
    {
        return (itemsPerArray switch
        {
            < 1 => throw new ArgumentOutOfRangeException(nameof(itemsPerArray)),
            1 => SingleMemberDataIterator<T>(items),
            2 => DoubleMemberDataIterator<T>(items),
            _ => MemberDataIterator<T>(itemsPerArray, items)
        })!;
    }


    public static IEnumerable<object?[]> ToNullableMemberData<T>(int itemsPerArray, params T?[] items)
    {
        return (itemsPerArray switch
        {
            < 1 => throw new ArgumentOutOfRangeException(nameof(itemsPerArray)),
            1 => SingleMemberDataIterator<T>(items),
            2 => DoubleMemberDataIterator<T>(items),
            _ => MemberDataIterator<T>(itemsPerArray, items)
        });
    }

    public static IEnumerable<object?[]> Double<T>(params T?[] items)
    {
        int itemCount = items.Length;
        for (var i = 0; i < itemCount; i++)
        for (var j = 0; j < itemCount; j++)
        {
            yield return new object?[2] { items[i], items[j] };
        }
    }
    
   
}