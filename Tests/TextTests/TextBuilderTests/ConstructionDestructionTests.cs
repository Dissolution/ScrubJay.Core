using ScrubJay.Text;

namespace ScrubJay.Tests.TextTests.TextBuilderTests;

public class ConstructionDestructionTests
{
    [Fact]
    public void CanConstructAndDispose()
    {
        TextBuilder builder = new TextBuilder();
        Assert.NotNull(builder);
        Assert.Empty(builder);

        builder.Dispose();
        Assert.NotNull(builder);
        Assert.Empty(builder);
    }

    [Fact]
    public void CanNewAndDispose()
    {
        TextBuilder builder = TextBuilder.New;
        Assert.NotNull(builder);
        Assert.Empty(builder);

        builder.Dispose();
        Assert.NotNull(builder);
        Assert.Empty(builder);
    }

    // [Theory]
    // //[InlineData(data: [0,64,1025,25000])]
    // public void CtorMinCapacityWorks(int minCapacity)
    // {
    //     using TextBuilder builder = new(minCapacity);
    //     Assert.True(builder.Capacity >= minCapacity);
    // }
}