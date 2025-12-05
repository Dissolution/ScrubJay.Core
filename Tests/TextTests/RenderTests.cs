using ScrubJay.Functional;
using ScrubJay.Rendering;


namespace ScrubJay.Tests.TextTests;

public class RenderTests
{
    public static TheoryData<Type> AllTypes { get; }

    static RenderTests()
    {
        AllTypes = new(AppDomain
            .CurrentDomain
            .GetAssemblies()
            .SelectMany(static ass => Result.Try(ass, static a => a.GetTypes()).OkOr([]))
            .ToHashSet());
    }


    [Theory]
    [MemberData(nameof(AllTypes))]
    public void CanRenderType(Type type)
    {
        string? str = type.Render();
        Assert.NotNull(str);
    }

    [Fact]
    public void CanRenderTypeArray()
    {
        Type[] allTypes = AllTypes.Select(r => r.Data).ToArray();
        Assert.NotNull(allTypes);
        string? str = allTypes.Render();
        Assert.NotNull(str);
    }
}