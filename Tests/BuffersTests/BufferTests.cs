using System.Reflection;
using System.Runtime.CompilerServices;
using ScrubJay.Buffers;
using ScrubJay.Comparison;

namespace ScrubJay.Tests.BuffersTests;

public class BufferTests
{
    [Fact]
    public void EmptyBufferDoesNotAllocate()
    {
        using Buffer<int> defaultBuffer = default;
        Assert.Null(defaultBuffer._array);
        Assert.Equal(0, defaultBuffer._span.Length);

        using Buffer<char> newBuffer = new Buffer<char>();
        Assert.Null(newBuffer._array);
        Assert.Equal(0, newBuffer._span.Length);

        using Buffer<DateTime> emptyBuffer = new(0);
        Assert.Null(emptyBuffer._array);
        Assert.Equal(0, emptyBuffer._span.Length);
    }

    [Fact]
    public void BufferCanBeDisposed()
    {
        Buffer<int> defaultBuffer = default;
        defaultBuffer.Dispose();

        Buffer<char> newBuffer = new Buffer<char>();
        newBuffer.Dispose();

        Buffer<DateTime> emptyBuffer = new(0);
        emptyBuffer.Dispose();

        Buffer<byte> ungrownEmptyStackBuffer = stackalloc byte[32];
        ungrownEmptyStackBuffer.Dispose();

        Buffer<byte> ungrownStackBuffer = stackalloc byte[32];
        ungrownStackBuffer.AddMany(1, 4, 7);
        ungrownStackBuffer.Dispose();

        Buffer<object> ungrownEmptyBuffer = new(32);
        ungrownEmptyBuffer.Dispose();

        Buffer<object> ungrownBuffer = new(32);
        ungrownBuffer.AddMany("Eat", BindingFlags.Static, DateTime.Now);
        ungrownBuffer.Dispose();

        Buffer<byte> grownStackBuffer = stackalloc byte[32];
        grownStackBuffer.AddMany(Enumerable.Range(0, 147).Select(static i => (byte)i));
        grownStackBuffer.Dispose();

        Buffer<object> grownBuffer = new(32);
        for (int i = 0; i < 10; i++)
        {
            grownBuffer.AddMany(
                "Eat", BindingFlags.Static, DateTime.Now, Guid.NewGuid(), typeof(void), 1, 2, 3,
                4, 5, 6);
        }

        grownBuffer.Dispose();

        Assert.True(true);
    }

    [Fact]
    public void BufferCanGrow()
    {
        using var buffer = new Buffer<int>(1);
        Assert.Equal(0, buffer.Count);
        Assert.Equal(ArrayPool.MinCapacity<int>(), buffer.Capacity);

        int[] numbers = Enumerable.Range(0, buffer.Capacity * 10).ToArray();
        buffer.AddMany(numbers);

        Assert.Equal(numbers.Length, buffer.Count);
        Assert.True(buffer.Capacity > ArrayPool.MinCapacity<int>());
        Assert.True(buffer.Capacity >= buffer.Count);

        for (int i = 0; i < buffer.Count; i++)
        {
            Assert.Equal(numbers[i], buffer[i]);
        }
    }

    [Fact]
    public void IntIndexerWorks()
    {
        using Buffer<int> buffer = new();
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
        using Buffer<int> buffer = new();
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
        using Buffer<int> buffer = new();
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
        using var buffer = new Buffer<object?>();
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
        using var buffer = new Buffer<object?>();
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
        using var buffer = new Buffer<object?>();
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
        using var buffer = new Buffer<object?>();
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

        using var buffer = new Buffer<byte>();
        buffer.AddMany(startArray);

        Index index = 0;
        buffer.TryInsert(index, 147);
        byte[] bufferArray = buffer.ToArray();
        Assert.Equal(endArray.Length, buffer.Count);
        Assert.Equal(endArray, bufferArray);
    }

    [Fact]
    public void InsertMiddleWorks()
    {
        byte[] startArray = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15];
        byte[] endArray = [0, 1, 2, 3, 4, 5, 6, 7, 147, 8, 9, 10, 11, 12, 13, 14, 15];

        using var buffer = new Buffer<byte>();
        buffer.AddMany(startArray);

        Index index = 8;
        buffer.TryInsert(index, 147);
        byte[] bufferArray = buffer.ToArray();
        Assert.Equal(endArray.Length, buffer.Count);
        Assert.Equal(endArray, bufferArray);
    }

    [Fact]
    public void InsertEndWorks()
    {
        byte[] startArray = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15];
        byte[] endArray = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 147];

        using var buffer = new Buffer<byte>();
        buffer.AddMany(startArray);

        Index index = ^0;
        buffer.TryInsert(index, 147);
        byte[] bufferArray = buffer.ToArray();
        Assert.Equal(endArray.Length, buffer.Count);
        Assert.Equal(endArray, bufferArray);
    }

    [Fact]
    public void InsertManyStartWorks()
    {
        byte[] startArray = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15];
        byte[] endArray = [255, 250, 245, 240, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15];

        using var buffer = new Buffer<byte>();
        buffer.AddMany(startArray);

        Index index = 0;
        buffer.TryInsertMany(index, [255, 250, 245, 240]);
        byte[] bufferArray = buffer.ToArray();
        Assert.Equal(endArray.Length, buffer.Count);
        Assert.Equal(endArray, bufferArray);
    }

    [Fact]
    public void InsertManyMiddleWorks()
    {
        byte[] startArray = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15];
        byte[] endArray = [0, 1, 2, 3, 4, 5, 6, 7, 255, 250, 245, 240, 8, 9, 10, 11, 12, 13, 14, 15];

        using var buffer = new Buffer<byte>();
        buffer.AddMany(startArray);

        Index index = 8;
        buffer.TryInsertMany(index, [255, 250, 245, 240]);
        byte[] bufferArray = buffer.ToArray();
        Assert.Equal(endArray.Length, buffer.Count);
        Assert.Equal(endArray, bufferArray);
    }

    [Fact]
    public void InsertManyEndWorks()
    {
        byte[] startArray = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15];
        byte[] endArray = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 255, 250, 245, 240];

        using Buffer<byte> buffer = new();
        buffer.AddMany(startArray);

        Index index = ^0;
        buffer.TryInsertMany(index, [255, 250, 245, 240]);
        byte[] bufferArray = buffer.ToArray();
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

        using var buffer = new Buffer<byte>();
        buffer.AddMany(startArray);

        Index index = 0;
        buffer.TryInsertMany(index, EnumerateInsertBytes());
        byte[] bufferArray = buffer.ToArray();
        Assert.Equal(endArray.Length, buffer.Count);
        Assert.Equal(endArray, bufferArray);
    }

    [Fact]
    public void InsertManyEnumerableMiddleWorks()
    {
        byte[] startArray = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15];
        byte[] endArray = [0, 1, 2, 3, 4, 5, 6, 7, 0, 111, 147, 255, 8, 9, 10, 11, 12, 13, 14, 15];

        using var buffer = new Buffer<byte>();
        buffer.AddMany(startArray);

        Index index = 8;
        buffer.TryInsertMany(index, EnumerateInsertBytes());
        byte[] bufferArray = buffer.ToArray();
        Assert.Equal(endArray.Length, buffer.Count);
        Assert.Equal(endArray, bufferArray);
    }

    [Fact]
    public void InsertManyEnumerableEndWorks()
    {
        byte[] startArray = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15];
        byte[] endArray = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 0, 111, 147, 255];

        using Buffer<byte> buffer = new();
        buffer.AddMany(startArray);

        Index index = ^0;
        buffer.TryInsertMany(index, EnumerateInsertBytes());
        byte[] bufferArray = buffer.ToArray();
        Assert.Equal(endArray.Length, buffer.Count);
        Assert.Equal(endArray, bufferArray);
    }

    [Fact]
    public void ContainsWorks()
    {
        using Buffer<int> intBuffer = new();
        intBuffer.AddMany(0, 1, 2, 3, 4, 5, 6, 7);

        for (int i = -10; i <= 20; i++)
        {
            if (i is >= 0 and <= 7)
            {
                Assert.True(intBuffer.Contains(i));
            }
            else
            {
                Assert.False(intBuffer.Contains(i));
            }
        }

        Span<Guid> guids = [Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()];
        using var guidBuffer = new Buffer<Guid>();
        guidBuffer.AddMany(guids);

        for (int i = 0; i < 100; i++)
        {
            Assert.False(guidBuffer.Contains(Guid.NewGuid()));
        }

        foreach (var guid in guids)
        {
            Assert.True(guidBuffer.Contains(guid));
        }


        using var recordBuffer = new Buffer<TestClassRecord>();
        List<TestClassRecord> records = new List<TestClassRecord>();
        for (int i = 0; i < 10; i++)
        {
            var obj = new TestClassRecord(i, "Record #{i}", i % 2 == 0);
            recordBuffer.Add(obj);
            records.Add(obj);
        }

        foreach (var record in records)
        {
            Assert.True(recordBuffer.Contains(record));
            Assert.False(recordBuffer.Contains(record with { IsAdmin = !record.IsAdmin }));
        }
    }


    [Fact]
    public void ToEnumerableWorks()
    {
        using Buffer<int> buffer = new();
        buffer.AddMany(
            0, 1, 2, 3, 4, 5, 6, 7,
            8, 9, 10, 11, 12, 13, 14, 15);

        var midItems = buffer.ToArray().Skip(4).Take(4).ToList();
        Assert.Equal([4, 5, 6, 7], midItems);
    }

    [Fact]
    public void TryFindIndexSingleWorks()
    {
        using Buffer<int> buffer = new();
        buffer.AddMany(
            0, 1, 2, 3, 4, 5, 6, 7,
            8, 9, 10, 11, 12, 13, 14, 15);

        // basic search
        for (int i = 0; i < 16; i++)
        {
            var found = buffer.TryFindIndex(i);
            Assert.True(found.HasSome(out int index));
            Assert.Equal(i, index);

            found = buffer.TryFindIndex(i, firstToLast: false);
            Assert.True(found.HasSome(out index));
            Assert.Equal(i, index);
        }

        // with equality comparer
        {
            IEqualityComparer<int> oddnessEqualityComparer = Equate.CreateEqualityComparer<int>(static (a, b) => (a % 2 == 0) == (b % 2 == 0), static i => i % 2);
            var found = buffer.TryFindIndex(3, itemComparer: oddnessEqualityComparer);
            Assert.True(found.HasSome(out int index));
            Assert.Equal(1, index); // first odd item is item #1

            found = buffer.TryFindIndex(3, firstToLast: false, itemComparer: oddnessEqualityComparer);
            Assert.True(found.HasSome(out index));
            Assert.Equal(15, index); // last odd item is item #15
        }

        // with index
        {
            var found = buffer.TryFindIndex(4, offset: 3);
            Assert.True(found.HasSome(out int index));
            Assert.Equal(4, index);

            found = buffer.TryFindIndex(2, offset: 3);
            Assert.True(found.IsNone);

            found = buffer.TryFindIndex(10, offset: ^8);
            Assert.True(found.HasSome(out index));
            Assert.Equal(10, index);

            found = buffer.TryFindIndex(7, offset: ^4);
            Assert.True(found.IsNone);


            found = buffer.TryFindIndex(4, offset: 3, firstToLast: false);
            Assert.True(found.IsNone);

            found = buffer.TryFindIndex(2, offset: 3, firstToLast: false);
            Assert.True(found.HasSome(out index));
            Assert.Equal(2, index);

            found = buffer.TryFindIndex(10, offset: ^8, firstToLast: false);
            Assert.True(found.IsNone);

            found = buffer.TryFindIndex(7, offset: ^4, firstToLast: false);
            Assert.True(found.HasSome(out index));
            Assert.Equal(7, index);
        }
    }

    [Fact]
    public void TryFindIndexMultiWorks()
    {
        using Buffer<int> buffer = new();
        buffer.AddMany(
            0, 1, 2, 3, 4, 5, 6, 7,
            8, 9, 10, 11, 12, 13, 14, 15);

        // basic search
        {
            var found = buffer.TryFindIndex([1, 2, 3]);
            Assert.True(found.HasSome(out int index));
            Assert.Equal(1, index);

            found = buffer.TryFindIndex([8, 9, 10]);
            Assert.True(found.HasSome(out index));
            Assert.Equal(8, index);

            found = buffer.TryFindIndex([14, 15, 16]);
            Assert.True(found.IsNone);

            found = buffer.TryFindIndex([3]);
            Assert.True(found.HasSome(out index));
            Assert.Equal(3, index);
        }

        // with equality comparer
        {
            IEqualityComparer<int> oddnessEqualityComparer = Equate.CreateEqualityComparer<int>(static (a, b) => (a % 2 == 0) == (b % 2 == 0), static i => i % 2);
            var found = buffer.TryFindIndex([33, 22, 33], itemComparer: oddnessEqualityComparer);
            Assert.True(found.HasSome(out int index));
            Assert.Equal(1, index); // first odd/even/odd is 1,2,3

            found = buffer.TryFindIndex([33, 22, 33], firstToLast: false, itemComparer: oddnessEqualityComparer);
            Assert.True(found.HasSome(out index));
            Assert.Equal(13, index); // last odd/even/odd is 13,14,15
        }

        // with index
        {
        }
    }
}
