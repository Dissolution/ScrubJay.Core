using ScrubJay.Collections;

namespace ScrubJay.Tests.CollectionsTests;

public class BoundsTests
{
    [Fact]
    public void FromBoundedRangeWorks()
    {
        var hasBounds = Bounds.FromRange(1..2);
        Assert.True(hasBounds.HasSome(out var bounds));

        Assert.True(bounds.Lower.HasSome(out var lowerBound));
        Assert.True(lowerBound.IsInclusive);
        Assert.Equal(1, lowerBound.Value);

        Assert.True(bounds.Upper.HasSome(out var upperBound));
        Assert.False(upperBound.IsInclusive);
        Assert.Equal(2, upperBound.Value);
    }
}