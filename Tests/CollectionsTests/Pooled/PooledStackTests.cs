using ScrubJay.Pooling;

namespace ScrubJay.Tests.CollectionsTests.Pooled;

public class PooledStackTests
{
    [Fact]
    public void ToArrayWorks()
    {
        using var stack = new PooledStack<int>();
        stack.Push(1);
        stack.Push(4);
        stack.Push(7);

        int[] array1 = stack.ToArray(popOrder: true);
        Assert.Equal(3, array1.Length);
        Assert.Equal(7, array1[0]);
        Assert.Equal(4, array1[1]);
        Assert.Equal(1, array1[2]);

        int[] array2 = stack.ToArray(popOrder: false);
        Assert.Equal(3, array2.Length);
        Assert.Equal(1, array2[0]);
        Assert.Equal(4, array2[1]);
        Assert.Equal(7, array2[2]);
    }

    [Fact]
    public void PushWorks()
    {
        using var stack = new PooledStack<int>();
        Assert.Empty(stack);

        for (int i = 1; i <= 10; i++)
        {
            stack.Push(i);
            Assert.Equal(i, stack.Count);
        }
    }

    [Fact]
    public void PushManyWorks()
    {
        using var stack = new PooledStack<int>();
        Assert.Empty(stack);

        int[] array = [1, 4, 7];
        stack.PushMany(array);
        Assert.Equal(3, stack.Count);
    }

    [Fact]
    public void PeekManyWorks()
    {
        using var stack = new PooledStack<int>();
        int[] array = [1, 4, 7];
        stack.PushMany(array);
        Assert.Equal(3, stack.Count);

        Assert.True(stack.TryPeekMany(2).IsOk(out int[]? peeked));
        Assert.Equal(3, stack.Count);

        Assert.Equal(2, peeked.Length);
        Assert.Equal(7, peeked[0]);
        Assert.Equal(4, peeked[1]);

        Assert.Equal(1, stack[^1]);
    }

    [Fact]
    public void PushAtWorks()
    {
        using var stack = new PooledStack<int>();

        stack.PushMany([1, 4, 7]);
        Assert.Equal(3, stack.Count);

        var pushResult = stack.TryPushAt(0, 13);
        Assert.True(pushResult);
        Assert.Equal(4, stack.Count);
        Assert.Equal<int>([13,7,4,1], stack);

        pushResult = stack.TryPushAt(^1, 13);
        Assert.True(pushResult);
        Assert.Equal(5, stack.Count);
        Assert.Equal<int>([13,7,4,1,13], stack);

        stack.Clear();
        stack.PushMany([1, 4, 7]);
        Assert.Equal(3, stack.Count);

        pushResult = stack.TryPushAt(new(0,popOrder: false), 13);
        Assert.True(pushResult);
        Assert.Equal(4, stack.Count);
        Assert.Equal<int>([7,4,1,13], stack);
    }
}
