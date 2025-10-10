#pragma warning disable CA1812

// ReSharper disable InvokeAsExtensionMethod

using System.Collections;
using System.Reflection;
using ScrubJay.Extensions;
using ScrubJay.Functional;
using TypeExtensions = ScrubJay.Extensions.TypeExtensions;

namespace ScrubJay.Tests.ExtensionsTests;

public class TypeExtensionsTests
{
    private class TestList<T> : List<T>, IList<T>;

    private sealed class NestedTestList<T> : TestList<T>, IList<T>;

    public static IEnumerable<Type?> TestTypes { get; } =
    [
        null,
        typeof(void),
        typeof(byte),
        typeof(void*),
        typeof(object),
        typeof(Unit),
        typeof(ValueType),
        typeof(TypeExtensions),
        typeof(IEnumerable),
        typeof(IList<>),
        typeof(IDictionary<,>),
        typeof(List<char>),
        typeof(List<>),
        typeof(TestList<object>),
        typeof(NestedTestList<DateTime>),
        typeof(AttributeTargets),
        typeof((int Id, string Name)),
        (new
        {
            Id = 3,
            Name = "TJ"
        }).GetType(),
    ];

    public static TheoryData<Type?> MemberDataTypes => new(TestTypes);

    // support for testing
    private static T? GetDefault<T>() => default(T);


    [Theory]
    [MemberData(nameof(MemberDataTypes))]
    public void GetBaseTypesWorks(Type? type)
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
    [MemberData(nameof(MemberDataTypes))]
    public void ImplementsSelfAllShould(Type? type)
    {
        bool impl = TypeExtensions.Implements(type, type);
        if (type is null)
        {
            Assert.False(impl);
        }
        else
        {
            Assert.True(impl);
        }
    }

    [Theory]
    [MemberData(nameof(MemberDataTypes))]
    public void ImplementsObjectMostShould(Type? type)
    {
        if (type is null)
        {
            Assert.False(type.Implements<object>());
        }
        else if (type.IsPointer)
        {
#if NETFRAMEWORK
            Assert.True(typeof(object).IsAssignableFrom(typeof(void*)));
#else
            Assert.False(typeof(object).IsAssignableFrom(typeof(void*)));
#endif
            Assert.False(type.Implements<object>());
        }
        else
        {
            Assert.True(type.Implements<object>());
        }
    }

    [Theory]
    [MemberData(nameof(MemberDataTypes))]
    public void ImplementsBaseClassesAllShould(Type? type)
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
    [MemberData(nameof(MemberDataTypes))]
    public void ImplementsInterfacesAllShould(Type? type)
    {
        if (type is null)
        {
            return;
        }

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
    [MemberData(nameof(MemberDataTypes))]
    public void TypeCanContainNullWorks(Type? type)
    {
        bool canBeNull = TypeExtensions.get_CanBeNull(type);

        if (type is null || type.IsPointer)
        {
            Assert.True(canBeNull);
        }
        else if (type.IsStatic)
        {
            Assert.False(canBeNull);
        }
        else if (type.ContainsGenericParameters || (type == typeof(void)))
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

    [Theory, CombinatorialData]
    public void ImplementsAgreesWithTypeIsAssignableTo(
        [CombinatorialMemberData(nameof(TestTypes))] Type? left,
        [CombinatorialMemberData(nameof(TestTypes))] Type? right)
    {
        bool impl = left.Implements(right);

        bool assignable;
        if (left is null || right is null)
        {
            assignable = false;
        }
        else
        {
            if (right.IsGenericTypeDefinition)
            {
                assignable = left.HasGenericTypeDefinition(right) ||
                    (left.GetBaseTypes().Any(t => t.HasGenericTypeDefinition(right)) |
                        left.GetInterfaces().Any(i => i.HasGenericTypeDefinition(right)));
            }
#if NETFRAMEWORK
            else if (left.IsPointer)
            {
                assignable = left == right;
            }
#endif
            else
            {
                assignable = left.IsAssignableTo(right);
            }
        }


        Assert.Equal(assignable, impl);
    }
}
