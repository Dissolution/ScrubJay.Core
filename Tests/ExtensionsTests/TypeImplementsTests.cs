// ReSharper disable InvokeAsExtensionMethod

using System.Collections;
using ScrubJay.Extensions;

namespace ScrubJay.Tests.ExtensionsTests;

public class TypeImplementsTests
{
    public static TheoryData<Type?> TheoryTypes() => new()
    {
        null,
        typeof(byte),
        typeof(void),
        typeof(List<int>),
        typeof(ISet<decimal>),
        typeof(IDictionary<,>),
        typeof(TypeExtensions),
    };


    [Theory]
    [MemberData(nameof(TheoryTypes))]
    public void OnlyNullImplementsNull(Type? type)
    {
        if (type is null)
        {
            Assert.True(TypeExtensions.Implements(type, (Type?)null));
            Assert.True(TypeExtensions.Implements((Type?)null, type));
        }
        else
        {
            Assert.False(TypeExtensions.Implements(type, (Type?)null));
            Assert.False(TypeExtensions.Implements((Type?)null, type));
        }
    }

    [Theory]
    [MemberData(nameof(TheoryTypes))]
    public void TypeImplementsSelf(Type? type)
    {
        Assert.True(TypeExtensions.Implements(type, type));
    }

    private class TestList<T> : List<T>;

    private class NestedTestList<T> : TestList<T>;

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