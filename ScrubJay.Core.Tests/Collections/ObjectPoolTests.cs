using ScrubJay.Collections;

namespace ScrubJay.Tests.Collections;

public class ObjectPoolTests
{
    [Fact]
    public void InstanceIsTheSame()
    {
        var pool = new ObjectPool<StrongBox<Guid>>(
            totalCapacity: 10,
            factory: () => new StrongBox<Guid>(Guid.NewGuid()));

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
        var pool = new ObjectPool<StrongBox<Guid>>(
            totalCapacity: 2,
            factory: () => new StrongBox<Guid>(Guid.NewGuid()));

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
}