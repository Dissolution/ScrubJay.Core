using System.Reflection;
using ScrubJay.Buffers;
using Buffer = ScrubJay.Buffers.Buffer;

namespace ScrubJay.Tests.Buffers;

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

        Buffer<byte> ungrownEmptyStackBuffer = Buffer.FromStackAlloc(stackalloc byte[32]);
        ungrownEmptyStackBuffer.Dispose();

        Buffer<byte> ungrownStackBuffer = Buffer.FromStackAlloc(stackalloc byte[32]);
        ungrownStackBuffer.AddMany(1, 4, 7);
        ungrownStackBuffer.Dispose();

        Buffer<object> ungrownEmptyBuffer = new(32);
        ungrownEmptyBuffer.Dispose();

        Buffer<object> ungrownBuffer = new(32);
        ungrownBuffer.AddMany("Eat", BindingFlags.Static, DateTime.Now);
        ungrownBuffer.Dispose();

        Buffer<byte> grownStackBuffer = Buffer.FromStackAlloc(stackalloc byte[32]);
        grownStackBuffer.AddMany(Enumerable.Range(0, 147).Select(static i => (byte)i));
        grownStackBuffer.Dispose();

        Buffer<object> grownBuffer = new(32);
        for (var i = 0; i < 10; i++)
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
        Assert.Equal(ArrayPool.MinCapacity, buffer.Capacity);

        var numbers = Enumerable.Range(0, buffer.Capacity * 10).ToArray();
        buffer.AddMany(numbers);

        Assert.Equal(numbers.Length, buffer.Count);
        Assert.True(buffer.Capacity > ArrayPool.MinCapacity);
        Assert.True(buffer.Capacity >= buffer.Count);

        for (var i = 0; i < buffer.Count; i++)
        {
            Assert.Equal(numbers[i], buffer[i]);
        }
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

        for (var i = 0; i < buffer.Count; i++)
        {
            Assert.Equal(objects[i], buffer[i]);
        }
    }

    [Fact]
    public void AddManyWorks()
    {
        using var buffer = new Buffer<object?>();
        List<object?> list = new();

        var objects = TestHelper.TestObjects;
        buffer.AddMany(objects);
        list.AddRange(objects);

        Assert.Equal(objects.Count, buffer.Count);
        Assert.Equal(list.Count, buffer.Count);

        for (var i = 0; i < buffer.Count; i++)
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
        buffer.Insert(index, 147);
        var bufferArray = buffer.ToArray();
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
        buffer.Insert(index, 147);
        var bufferArray = buffer.ToArray();
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
        buffer.Insert(index, 147);
        var bufferArray = buffer.ToArray();
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
        buffer.InsertMany(index, [255, 250, 245, 240]);
        var bufferArray = buffer.ToArray();
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
        buffer.InsertMany(index, [255, 250, 245, 240]);
        var bufferArray = buffer.ToArray();
        Assert.Equal(endArray.Length, buffer.Count);
        Assert.Equal(endArray, bufferArray);
    }

    [Fact]
    public void InsertManyEndWorks()
    {
        byte[] startArray = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15];
        byte[] endArray = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 255, 250, 245, 240];

        using var buffer = Buffer.FromSpan<byte>(startArray);

        Index index = ^0;
        buffer.InsertMany(index, [255, 250, 245, 240]);
        var bufferArray = buffer.ToArray();
        Assert.Equal(endArray.Length, buffer.Count);
        Assert.Equal(endArray, bufferArray);
    }

    [Fact]
    public void ToEnumerableWorks()
    {
        using Buffer<int> buffer = Buffer.FromSpan([0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15]);

        var midItems = buffer.ToEnumerable().Skip(4).Take(4).ToList();
        Assert.Equal([4, 5, 6, 7], midItems);
    }
}