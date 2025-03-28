using ScrubJay.Constraints;

namespace ScrubJay.Tests.CollectionsTests;

public class BoundsTests
{
    [Fact]
    public void FromBoundedRangeWorks()
    {
        var hasBounds = Bounds.FromRange(1..2);
        Assert.True(hasBounds.IsSome(out var bounds));

        Assert.True(bounds.Lower.IsSome(out var lowerBound));
        Assert.True(lowerBound.IsInclusive);
        Assert.Equal(1, lowerBound.Value);

        Assert.True(bounds.Upper.IsSome(out var upperBound));
        Assert.False(upperBound.IsInclusive);
        Assert.Equal(2, upperBound.Value);
    }
}