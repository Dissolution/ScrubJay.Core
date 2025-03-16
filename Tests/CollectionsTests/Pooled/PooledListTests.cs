using System.Reflection;
using System.Runtime.CompilerServices;
using ScrubJay.Pooling;

// ReSharper disable CollectionNeverUpdated.Local

namespace ScrubJay.Tests.CollectionsTests.Pooled;

public class PooledListTests
{
    [Fact]
    public void PooledListCanBeDisposed()
    {
        PooledList<char> newPooledList = new PooledList<char>();
        newPooledList.Dispose();

        PooledList<DateTime> emptyPooledList = new(0);
        emptyPooledList.Dispose();

        PooledList<object> ungrownEmptyPooledList = new(32);
        ungrownEmptyPooledList.Dispose();

        PooledList<object> ungrownPooledList = new(32);
        ungrownPooledList.AddMany("Eat", BindingFlags.Static, DateTime.Now);
        ungrownPooledList.Dispose();

        PooledList<object> grownPooledList = new(32);
        for (int i = 0; i < 10; i++)
        {
            grownPooledList.AddMany(
                "Eat", BindingFlags.Static, DateTime.Now, Guid.NewGuid(), typeof(void), 1, 2, 3,
                4, 5, 6);
        }

        grownPooledList.Dispose();

        Assert.True(true);
    }

    [Fact]
    public void PooledListCanGrow()
    {
        using var buffer = new PooledList<int>(1);
        Assert.Empty(buffer);

        int[]? numbers = Enumerable.Range(0, buffer.Capacity * 10).ToArray();
        buffer.AddMany(numbers);

        Assert.Equal(numbers.Length, buffer.Count);
        Assert.True(buffer.Capacity >= buffer.Count);

        for (int i = 0; i < buffer.Count; i++)
        {
            Assert.Equal(numbers[i], buffer[i]);
        }
    }

    [Fact]
    public void IntIndexerWorks()
    {
        using PooledList<int> buffer = new();
        buffer.AddMany(0, 1, 2, 3, 4, 5, 6, 7);

        for (int i = 0; i < buffer.Count; i++)
        {
            ref int refItem = ref buffer[i];
            Assert.Equal(i, refItem);
            refItem = 147;
            Assert.Equal(147, refItem);
            Assert.Equal(147, buffer[i]);
        }

        Assert.All(buffer.ToArray(), static b => Assert.Equal(147, b));
    }

    [Fact]
    public void IndexIndexerWorks()
    {
        using PooledList<int> buffer = new();
        buffer.AddMany(0, 1, 2, 3, 4, 5, 6, 7);
        int bufferCount = buffer.Count;

        for (int i = 1; i <= bufferCount; i++)
        {
            Index index = ^i;
            ref int refItem = ref buffer[index];
            Assert.Equal(bufferCount - i, refItem);
            refItem = 147;
            Assert.Equal(147, refItem);
            Assert.Equal(147, buffer[index]);
        }

        Assert.All(buffer.ToArray(), static b => Assert.Equal(147, b));
    }

    [Fact]
    public void RangeIndexerWorks()
    {
        using PooledList<int> buffer = new();
        buffer.AddMany(0, 1, 2, 3, 4, 5, 6, 7);

#if !NETFRAMEWORK
        Assert.Equal<int>(buffer[0..2], [0, 1]);
        Assert.Equal<int>(buffer[3..5], [3, 4]);
        Assert.Equal<int>(buffer[..^4], [0, 1, 2, 3]);
#else
        Assert.Equal(buffer[0..2], [0, 1]);
        Assert.Equal(buffer[3..5], [3, 4]);
        Assert.Equal(buffer[..^4], [0, 1, 2, 3]);
#endif
    }

    [Fact]
    public void AddWorks()
    {
        using var buffer = new PooledList<object?>();
        List<object?> list = new();

        var objects = TestHelper.TestObjects;
        foreach (object? obj in objects)
        {
            buffer.Add(obj);
            list.Add(obj);
        }

        Assert.Equal(objects.Count, buffer.Count);
        Assert.Equal(list.Count, buffer.Count);

        for (int i = 0; i < buffer.Count; i++)
        {
            Assert.Equal(objects[i], buffer[i]);
        }
    }

    [Fact]
    public void AddManySpanWorks()
    {
        using var buffer = new PooledList<object?>();
        List<object?> list = new();

        Span<object?> objects = TestHelper.TestObjects.ToArray();
        buffer.AddMany(objects);
        list.AddRange(TestHelper.TestObjects);

        Assert.Equal(TestHelper.TestObjects.Count, buffer.Count);
        Assert.Equal(list.Count, buffer.Count);

        for (int i = 0; i < buffer.Count; i++)
        {
            Assert.Equal(objects[i], buffer[i]);
        }
    }

    [Fact]
    public void AddManyCountableWorks()
    {
        using var buffer = new PooledList<object?>();
        List<object?> list = new();

        var objects = TestHelper.TestObjects;
        buffer.AddMany(objects);
        list.AddRange(objects);

        Assert.Equal(objects.Count, buffer.Count);
        Assert.Equal(list.Count, buffer.Count);

        for (int i = 0; i < buffer.Count; i++)
        {
            Assert.Equal(objects[i], buffer[i]);
        }
    }

    [Fact]
    public void AddManyUncountableWorks()
    {
        using var buffer = new PooledList<object?>();
        List<object?> list = new();

        var objects = TestHelper.TestObjects;
        buffer.AddMany(objects.AsEnumerable());
        list.AddRange(objects);

        Assert.Equal(objects.Count, buffer.Count);
        Assert.Equal(list.Count, buffer.Count);

        for (int i = 0; i < buffer.Count; i++)
        {
            Assert.Equal(objects[i], buffer[i]);
        }
    }

    [Fact]
    public void InsertStartWorks()
    {
        byte[] startArray = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15];
        byte[] endArray = [147, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15];

        using var buffer = new PooledList<byte>();
        buffer.AddMany(startArray);

        Index index = 0;
        buffer.TryInsert(index, 147);
        byte[]? bufferArray = buffer.ToArray();
        Assert.Equal(endArray.Length, buffer.Count);
        Assert.Equal(endArray, bufferArray);
    }

    [Fact]
    public void InsertMiddleWorks()
    {
        byte[] startArray = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15];
        byte[] endArray = [0, 1, 2, 3, 4, 5, 6, 7, 147, 8, 9, 10, 11, 12, 13, 14, 15];

        using var buffer = new PooledList<byte>();
        buffer.AddMany(startArray);

        Index index = 8;
        buffer.TryInsert(index, 147);
        byte[]? bufferArray = buffer.ToArray();
        Assert.Equal(endArray.Length, buffer.Count);
        Assert.Equal(endArray, bufferArray);
    }

    [Fact]
    public void InsertEndWorks()
    {
        byte[] startArray = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15];
        byte[] endArray = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 147];

        using var buffer = new PooledList<byte>();
        buffer.AddMany(startArray);

        Index index = ^0;
        buffer.TryInsert(index, 147);
        byte[]? bufferArray = buffer.ToArray();
        Assert.Equal(endArray.Length, buffer.Count);
        Assert.Equal(endArray, bufferArray);
    }

    [Fact]
    public void InsertManyStartWorks()
    {
        byte[] startArray = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15];
        byte[] endArray = [255, 250, 245, 240, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15];

        using var buffer = new PooledList<byte>();
        buffer.AddMany(startArray);

        Index index = 0;
        buffer.TryInsertMany(index, [255, 250, 245, 240]);
        byte[]? bufferArray = buffer.ToArray();
        Assert.Equal(endArray.Length, buffer.Count);
        Assert.Equal(endArray, bufferArray);
    }

    [Fact]
    public void InsertManyMiddleWorks()
    {
        byte[] startArray = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15];
        byte[] endArray = [0, 1, 2, 3, 4, 5, 6, 7, 255, 250, 245, 240, 8, 9, 10, 11, 12, 13, 14, 15];

        using var buffer = new PooledList<byte>();
        buffer.AddMany(startArray);

        Index index = 8;
        buffer.TryInsertMany(index, [255, 250, 245, 240]);
        byte[]? bufferArray = buffer.ToArray();
        Assert.Equal(endArray.Length, buffer.Count);
        Assert.Equal(endArray, bufferArray);
    }

    [Fact]
    public void InsertManyEndWorks()
    {
        byte[] startArray = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15];
        byte[] endArray = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 255, 250, 245, 240];

        using PooledList<byte> buffer = new();
        buffer.AddMany(startArray);

        Index index = ^0;
        buffer.TryInsertMany(index, [255, 250, 245, 240]);
        byte[]? bufferArray = buffer.ToArray();
        Assert.Equal(endArray.Length, buffer.Count);
        Assert.Equal(endArray, bufferArray);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static IEnumerable<byte> EnumerateInsertBytes()
    {
        yield return 0;
        yield return 111;
        yield return 147;
        yield return 255;
    }

    [Fact]
    public void InsertManyEnumerableStartWorks()
    {
        byte[] startArray = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15];
        byte[] endArray = [0, 111, 147, 255, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15];

        using var buffer = new PooledList<byte>();
        buffer.AddMany(startArray);

        Index index = 0;
        buffer.TryInsertMany(index, EnumerateInsertBytes());
        byte[]? bufferArray = buffer.ToArray();
        Assert.Equal(endArray.Length, buffer.Count);
        Assert.Equal(endArray, bufferArray);
    }

    [Fact]
    public void InsertManyEnumerableMiddleWorks()
    {
        byte[] startArray = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15];
        byte[] endArray = [0, 1, 2, 3, 4, 5, 6, 7, 0, 111, 147, 255, 8, 9, 10, 11, 12, 13, 14, 15];

        using var buffer = new PooledList<byte>();
        buffer.AddMany(startArray);

        Index index = 8;
        buffer.TryInsertMany(index, EnumerateInsertBytes());
        byte[]? bufferArray = buffer.ToArray();
        Assert.Equal(endArray.Length, buffer.Count);
        Assert.Equal(endArray, bufferArray);
    }

    [Fact]
    public void InsertManyEnumerableEndWorks()
    {
        byte[] startArray = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15];
        byte[] endArray = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 0, 111, 147, 255];

        using PooledList<byte> buffer = new();
        buffer.AddMany(startArray);

        Index index = ^0;
        buffer.TryInsertMany(index, EnumerateInsertBytes());
        byte[]? bufferArray = buffer.ToArray();
        Assert.Equal(endArray.Length, buffer.Count);
        Assert.Equal(endArray, bufferArray);
    }

    [Fact]
    public void ContainsWorks()
    {
        using PooledList<int> intPooledList = new();
        intPooledList.AddMany(0, 1, 2, 3, 4, 5, 6, 7);

        for (int i = -10; i <= 20; i++)
        {
            if (i is >= 0 and <= 7)
            {
                Assert.Contains(i, intPooledList);
            }
            else
            {
                Assert.DoesNotContain(i, intPooledList);
            }
        }

        Span<Guid> guids = [Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()];
        using var guidPooledList = new PooledList<Guid>();
        guidPooledList.AddMany(guids);

        for (int i = 0; i < 100; i++)
        {
            Assert.DoesNotContain(Guid.NewGuid(), guidPooledList);
        }

        foreach (var guid in guids)
        {
            Assert.Contains(guid, guidPooledList);
        }


        using var recordPooledList = new PooledList<TestClassRecord>();
        List<TestClassRecord> records = new List<TestClassRecord>();
        for (int i = 0; i < 10; i++)
        {
            var obj = new TestClassRecord(i, "Record #{i}", (i % 2) == 0);
            recordPooledList.Add(obj);
            records.Add(obj);
        }

        foreach (var record in records)
        {
            Assert.Contains(record, recordPooledList);
            Assert.DoesNotContain(record with { IsAdmin = !record.IsAdmin }, recordPooledList);
        }
    }


    [Fact]
    public void ToEnumerableWorks()
    {
        using PooledList<int> buffer = new();
        buffer.AddMany(
            0, 1, 2, 3, 4, 5, 6, 7,
            8, 9, 10, 11, 12, 13, 14, 15);

        var midItems = buffer.Skip(4).Take(4).ToList();
        Assert.Equal([4, 5, 6, 7], midItems);
    }

    [Fact]
    public void SliceWorks()
    {
        int[] numbers = [1,2,3,4,5];

        using PooledList<int> list = [];
        list.AddMany(numbers);
        Assert.Equal(5, list.Count);
        Assert.Equal(numbers, list);

        for (int i = 0; i < 5; i++)
        {
            var listSlice = list.Slice(i);
            var arraySlice = numbers.AsSpan().Slice(i);
            Assert.Equal(arraySlice, listSlice);
        }
    }
}
