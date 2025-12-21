using ScrubJay.Validation;
using ScrubJay.Validation.Demanding;

namespace ScrubJay.Tests.TextBuilderTests;

public class ConstructionTests
{
    [Fact]
    public void DefaultConstructorWorks()
    {
        using TextBuffer buffer = new TextBuffer();
        Assert.Equal(0, buffer.Capacity);
        Assert.Equal(0, buffer.Count);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(1024)]
    [InlineData(16_000)]
    public void MinCapacityWorks(int minCapacity)
    {
        using TextBuffer buffer = new TextBuffer(minCapacity);
        Demand.That(buffer.Capacity).IsGreaterThanOrEqualTo(minCapacity);
        Demand.That(buffer.Count).IsEqualTo(0);
    }
}