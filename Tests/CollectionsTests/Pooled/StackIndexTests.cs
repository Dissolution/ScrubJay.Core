using ScrubJay.Collections;
using ScrubJay.Collections.Pooling;

namespace ScrubJay.Tests.CollectionsTests.Pooled;

public class StackIndexTests
{
    [Fact]
    public void StackIndexDefaultsToPopOrder()
    {
        StackIndex si = 4;
        Assert.True(si.InPopOrder);

        si = Index.FromStart(3);
        Assert.True(si.InPopOrder);

    }

    [Fact]
    public void OffsetWorks()
    {
        int[] array = [1, 4, 7];

        Index[] indices = [0, 1, 2, 3, ^0, ^1, ^2, ^3];

        foreach (var index in indices)
        {
            int offset = index.GetOffset(array.Length);
            bool offsetIsValid = (offset >= 0) && (offset < array.Length);

            int k = (array.Length - offset) - 1;
            bool kIsValid = (k >= 0) && (k < array.Length);

            Assert.Equal(offsetIsValid, kIsValid);
        }
    }
}
