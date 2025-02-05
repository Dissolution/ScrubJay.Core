using System.Runtime.CompilerServices;
using System.Text;
using ScrubJay.Buffers;
using ScrubJay.Extensions;

// ReSharper disable AccessToDisposedClosure

namespace ScrubJay.Tests.BuffersTests;

public class ObjectPoolTests
{
    [Fact]
    public void InstanceIsTheSame()
    {
        using var pool = ObjectPool.Create(static () => new StrongBox<Guid>(Guid.NewGuid()));

        var firstInstance = pool.Rent();
        var firstGuid = firstInstance.Value;
        pool.Return(firstInstance);

        var secondInstance = pool.Rent();
        var secondGuid = secondInstance.Value;

        Assert.Equal(firstGuid, secondGuid);
    }

    [Fact]
    public void TotalCapacityWorks()
    {
        using var pool = ObjectPool.FromPolicy<StrongBox<Guid>>(new()
        {
            CreateInstance = static () => new(Guid.NewGuid()), MaxCapacity = 2,
        });

        var first = pool.Rent();
        var second = pool.Rent();
        Assert.NotEqual(first.Value, second.Value);
        var third = pool.Rent();
        Assert.NotEqual(first.Value, third.Value);
        Assert.NotEqual(second.Value, third.Value);
        pool.Return(first);
        pool.Return(second);
        pool.Return(third);

        var fourth = pool.Rent();
        Assert.Equal(first.Value, fourth.Value);
        var fifth = pool.Rent();
        Assert.Equal(second.Value, fifth.Value);
        var sixth = pool.Rent();
        Assert.NotEqual(third, sixth);
    }

    [Fact]
    public void CanCreateStringBuilder()
    {
        using var pool = ObjectPool.ForType<StringBuilder>();
        var builder = pool.Rent();
        Assert.NotNull(builder);
        Assert.Equal(0, builder.Length);
        pool.Return(builder);
    }

    [Fact]
    public void CanReuseStringBuilder()
    {
        using var pool = ObjectPool.ForType<StringBuilder>();

        var builder = pool.Rent();
        Assert.NotNull(builder);
        Assert.Equal(0, builder.Length);
        builder.Append("ABC");
        Assert.Equal(3, builder.Length);
        pool.Return(builder);

        var builder2 = pool.Rent();
        Assert.NotNull(builder2);
        Assert.Equal(3, builder2.Length);
        Assert.Equal("ABC", builder2.ToString());
        builder2.Append("DEF");
        Assert.Equal(6, builder2.Length);
        Assert.Equal("ABCDEF", builder2.ToString());
        pool.Return(builder2);

        var builder3 = pool.Rent();
        Assert.NotNull(builder3);
        Assert.Equal(6, builder3.Length);
        Assert.Equal("ABCDEF", builder3.ToString());
        pool.Return(builder3);
    }

    [Fact]
    public void CanCleanStringBuilder()
    {
        using var pool = ObjectPool.FromPolicy<StringBuilder>(new(
            static () => new(),
            static sb => sb.Clear()));

        var builder = pool.Rent();
        Assert.Equal(0, builder.Length);
        Assert.Equal("", builder.ToString());
        builder.Append("ABC");
        Assert.Equal(3, builder.Length);
        Assert.Equal("ABC", builder.ToString());
        pool.Return(builder);

        var builder2 = pool.Rent();
        Assert.Equal(0, builder2.Length);
        Assert.Equal("", builder2.ToString());
        builder2.Append("ABC");
        Assert.Equal(3, builder2.Length);
        Assert.Equal("ABC", builder2.ToString());
        pool.Return(builder2);

        var builder3 = pool.Rent();
        Assert.Equal(0, builder2.Length);
        Assert.Equal("", builder3.ToString());
        pool.Return(builder3);
    }

    [Fact]
    public void CanCleanArray()
    {
        using var pool = ObjectPool.FromPolicy<int[]>(new(
            static () => new int[8],
            static arr => arr.AsSpan().ForEach((ref int i) => i = 0)
        ));

        var array = pool.Rent();
        Assert.Equal(8, array.Length);
        // ReSharper disable once RedundantAssignment
        array.AsSpan().ForEach((ref int item) => item = 3);
        Assert.True(array.All(item => item == 3));
        pool.Return(array);
        Assert.True(array.All(item => item == 0));
    }

    [Fact]
    public async Task TestIfObjectPoolIsAsyncSafeAsync()
    {
        var rand = new Random();
        using var pool = ObjectPool.FromPolicy<StringBuilder>(new(
            static () => new(),
            static sb => sb.Clear()));
        const int Count = 100;
        var tasks = new Task<string>[Count];
        for (var i = 0; i < Count; i++)
        {
            tasks[i] = Task.Run<string>(async () =>
            {
                await Task.Delay(TimeSpan.FromMilliseconds(rand.Next(0, 100)));
                var sb = pool.Rent();
                Assert.NotNull(sb);
                Assert.Equal(0, sb.Length);
                Assert.Equal("", sb.ToString());
                sb.Append(Guid.NewGuid());
                Assert.True(sb.Length > 0);
                string str = sb.ToString();
                Assert.NotNull(str);
                pool.Return(sb);
                return str;
            });
        }
        var results = await Task.WhenAll(tasks);
        Assert.True(Array.TrueForAll(results, str => !string.IsNullOrWhiteSpace(str)));
        Assert.Equal(Count, results.Distinct().Count());
    }
}
