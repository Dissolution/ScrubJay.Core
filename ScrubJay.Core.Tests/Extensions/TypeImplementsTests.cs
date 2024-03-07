// ReSharper disable InvokeAsExtensionMethod

namespace ScrubJay.Tests.Extensions;

public class TypeImplementsTests
{
    public static readonly Type?[] TestTypes = new Type?[]
    {
    };

    public static IEnumerable<object?[]> TestData() => TestHelper.ToNullableMemberData<Type?>(1, 
        null,
        typeof(byte),
        typeof(void),
        typeof(List<int>),
        typeof(ISet<decimal>),
        typeof(IDictionary<,>),
        typeof(TypeExtensions));


    [Fact]
    public void OnlyNullImplementsNull()
    {
        Assert.True(TypeExtensions.Implements(null, null));
        Assert.False(TypeExtensions.Implements(null, typeof(byte)));
        Assert.False(TypeExtensions.Implements(null, typeof(void)));
        Assert.False(TypeExtensions.Implements(null, typeof(List<>)));
        Assert.False(TypeExtensions.Implements(typeof(byte), null));
        Assert.False(TypeExtensions.Implements(typeof(void), null));
        Assert.False(TypeExtensions.Implements(typeof(List<>), null));
    }

    [Theory]
    [MemberData(nameof(TestData))]
    public void TypeImplementsSelf(Type? type)
    {
        bool impl = TypeExtensions.Implements(type, type);
        Assert.True(impl);
    }

    public class TestList<T> : List<T>;

    public class NestedTestList<T> : TestList<T>;

    [Fact]
    public void SubClasses()
    {
        var listType = typeof(List<int>);
        var testListType = typeof(TestList<int>);
        var nestedTestListType = typeof(NestedTestList<int>);
        Assert.True(TypeExtensions.Implements(nestedTestListType, testListType));
        Assert.True(TypeExtensions.Implements(nestedTestListType, listType));
        Assert.False(TypeExtensions.Implements(testListType, nestedTestListType));
        Assert.False(TypeExtensions.Implements(listType, nestedTestListType));
    }

    [Fact]
    public void AllInterfaces()
    {
        var listType = typeof(List<int>);
        var ilistType = typeof(IList<int>);
        var icollectionType = typeof(ICollection<int>);
        var ienumerableType = typeof(IEnumerable);
        Assert.True(TypeExtensions.Implements(listType, ilistType));
        Assert.True(TypeExtensions.Implements(listType, icollectionType));
        Assert.True(TypeExtensions.Implements(listType, ienumerableType));
        Assert.False(TypeExtensions.Implements(listType, typeof(IDictionary)));
    }

    [Fact]
    public void OpenInterfaces()
    {
        var listType = typeof(List<int>);
        var dictionaryType = typeof(Dictionary<,>);
        var idictionaryType = typeof(IDictionary<,>);
        var ilistType = typeof(IList<>);
        var icollectionType = typeof(ICollection<>);

        Assert.True(TypeExtensions.Implements(listType, ilistType));
        Assert.True(TypeExtensions.Implements(listType, icollectionType));
        Assert.False(TypeExtensions.Implements(listType, idictionaryType));
        Assert.True(TypeExtensions.Implements(dictionaryType, idictionaryType));
        Assert.True(TypeExtensions.Implements(dictionaryType, icollectionType));
        Assert.True(TypeExtensions.Implements(dictionaryType, typeof(IEnumerable<>)));
        Assert.False(TypeExtensions.Implements(dictionaryType, ilistType));
    }
}