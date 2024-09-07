// ReSharper disable InvokeAsExtensionMethod

using System.Collections;
using System.Reflection;
using ScrubJay.Extensions;
using TypeExtensions = ScrubJay.Extensions.TypeExtensions;

namespace ScrubJay.Tests.ExtensionsTests;

public class TypeExtensionsTests
{
    private class TestList<T> : List<T>, IList<T>;

    private class NestedTestList<T> : TestList<T>, IList<T>;
    
    public static TheoryData<Type?> TestTypes { get; } = new()
    {
        null,
        typeof(void),
        typeof(byte),
        typeof(void*),
        typeof(byte*),
        typeof(object),
        typeof(None),
        typeof(ValueType),
        typeof(TypeExtensions),
        typeof(IEnumerable),
        typeof(ICollection<>),
        typeof(IDictionary<,>),
        typeof(List<char>),
        typeof(List<>),
        typeof(TestList<object>),
        typeof(NestedTestList<DateTime>),
        typeof(AttributeTargets),
        typeof((int Id, string Name)),
        (new { Id = 3, Name = "TJ"}).GetType(),
    };

    // support for testing
    private static T? GetDefault<T>() => default(T);


    [Theory]
    [MemberData(nameof(TestTypes))]
    public void GetBaseTypes_Works(Type? type)
    {
        var baseTypesList = TypeExtensions.GetBaseTypes(type).ToList();
        Assert.NotNull(baseTypesList);
    }
    
    
    // [Theory]
    // [MemberData(nameof(TheoryTypes))]
    // public void Implements_Null_OnlyNullShould(Type? type)
    // {
    //     if (type is null)
    //     {
    //         Assert.True(TypeExtensions.Implements(type, (Type?)null));
    //         Assert.True(TypeExtensions.Implements((Type?)null, type));
    //     }
    //     else
    //     {
    //         Assert.False(TypeExtensions.Implements(type, (Type?)null));
    //         Assert.False(TypeExtensions.Implements((Type?)null, type));
    //     }
    // }

    [Theory]
    [MemberData(nameof(TestTypes))]
    public void Implements_Self_AllShould(Type? type)
    {
        Assert.True(TypeExtensions.Implements(type, type));
    }

    [Theory]
    [MemberData(nameof(TestTypes))]
    public void Implements_Object_MostShould(Type? type)
    {

        if (type is null)
        {
            Assert.False(type.Implements<object>());
        }
        else if (type.IsPointer)
        {
#if NET481
            Assert.True(typeof(object).IsAssignableFrom(typeof(void*)));
            Assert.True(type.Implements<object>());
#else
            Assert.False(typeof(object).IsAssignableFrom(typeof(void*)));
            Assert.False(type.Implements<object>());
#endif
        }
        else
        {
            Assert.True(type.Implements<object>());
        }
    }

    [Theory]
    [MemberData(nameof(TestTypes))]
    public void Implements_BaseClasses_AllShould(Type? type)
    {
        foreach (Type baseType in type.GetBaseTypes())
        {
            Assert.True(TypeExtensions.Implements(type, baseType));
            if (baseType.IsGenericType)
            {
                var genericTypeDef = baseType.GetGenericTypeDefinition();
                Assert.True(TypeExtensions.Implements(type, genericTypeDef));
            }
        }
    }

    [Theory]
    [MemberData(nameof(TestTypes))]
    public void Implements_Interfaces_AllShould(Type? type)
    {
        if (type is null) return;
        foreach (Type interfaceType in type.GetInterfaces())
        {
            Assert.True(TypeExtensions.Implements(type, interfaceType));
            if (interfaceType.IsGenericType)
            {
                var genericTypeDef = interfaceType.GetGenericTypeDefinition();
                Assert.True(TypeExtensions.Implements(type, genericTypeDef));
            }
        }
    }

    [Theory]
    [MemberData(nameof(TestTypes))]
    public void Type_CanContainNull_Works(Type? type)
    {
        bool canBeNull = TypeExtensions.CanContainNull(type);
        
        if (type is null || type.IsPointer)
        {
            Assert.True(canBeNull);
        }
        else if (type.IsStatic())
        {
            Assert.False(canBeNull);
        }
        else if (type.ContainsGenericParameters || type == typeof(void))
        {
            // Skip these
        }
        else
        {
            object? def = typeof(TypeExtensionsTests)
                .GetMethod(nameof(GetDefault), BindingFlags.NonPublic | BindingFlags.Static)!
                .MakeGenericMethod(type)
                .Invoke(null, null);
            if (canBeNull)
            {
                Assert.Null(def);
            }
            else
            {
                Assert.NotNull(def);
            }
        }
    }
}