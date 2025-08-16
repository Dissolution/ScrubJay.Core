using System.Reflection;

namespace ScrubJay.Extensions;

/// <summary>
/// Extensions on <see cref="Type"/> and <see cref="TypeInfo"/>
/// </summary>
[PublicAPI]
public static class TypeExtensions
{
    /// <summary>
    /// Get all base <see cref="Type"/>s for this <paramref name="type"/>
    /// </summary>
    /// <param name="type">
    /// The <see cref="Type"/> to get all base types for
    /// </param>
    /// <returns>
    /// An enumeration of all base types
    /// </returns>
    public static IEnumerable<Type> GetBaseTypes(this Type? type)
    {
        if (type is null)
            yield break;
        for (Type? baseType = type.BaseType; baseType is not null; baseType = baseType.BaseType)
        {
            yield return baseType;
        }
    }

    /// <summary>
    /// Does this <see cref="Type"/> have the given <paramref name="genericTypeDefinition"/>?
    /// </summary>
    /// <param name="type"></param>
    /// <param name="genericTypeDefinition"></param>
    /// <returns></returns>
    public static bool HasGenericTypeDefinition(this Type? type, Type? genericTypeDefinition)
    {
        if (type is null || !type.IsGenericType || genericTypeDefinition is null)
            return false;
        return type.GetGenericTypeDefinition() == genericTypeDefinition;
    }

    /// <summary>
    /// Does <c>this</c> <paramref name="type"/> implement the <paramref name="checkType"/>?
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to examine</param>
    /// <param name="checkType">The <see cref="Type"/> that this <paramref name="type"/> must implement</param>
    /// <returns>
    /// <c>true</c> if <paramref name="type"/> implements <paramref name="checkType"/>; otherwise, <c>false</c>
    /// </returns>
    public static bool Implements(this Type? type, Type? checkType)
    {
        if (checkType is null)
            return false;
        if (type is null)
            return false;

        if (type == checkType)
            return true;
        // Everything implements object
        if ((checkType == typeof(object)) && !type.IsPointer)
            return true;

        if (!checkType.IsGenericTypeDefinition)
        {
            if (checkType.IsInterface)
            {
                // Check my interfaces
                foreach (var interfaceType in type.GetInterfaces())
                {
                    if (interfaceType == checkType)
                        return true;
                }

                return false;
            }

            // Check my base types
            for (Type? baseType = type.BaseType; baseType is not null /*&& baseType != typeof(object)*/; baseType = baseType.BaseType)
            {
                if (baseType == checkType)
                    return true;
            }
        }
        else
        {
            // Check direct (List<T> : List<>)
            if (type.HasGenericTypeDefinition(checkType))
                return true;

            // Check my base types
            for (Type? baseType = type.BaseType; baseType is not null/* && baseType != typeof(object)*/; baseType = baseType.BaseType)
            {
                if (baseType.HasGenericTypeDefinition(checkType))
                    return true;
            }

            // Check my interfaces
            foreach (var interfaceType in type.GetInterfaces())
            {
                if (interfaceType.HasGenericTypeDefinition(checkType))
                    return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Does <c>this</c> <paramref name="type"/> implement <typeparamref name="T"/>?
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to examine</param>
    /// <typeparam name="T">The generic <see cref="Type"/> that this <paramref name="type"/> must implement</typeparam>
    /// <returns>
    /// <c>true</c> if <paramref name="type"/> implements <typeparamref name="T"/>; otherwise, <c>false</c>
    /// </returns>
    public static bool Implements<T>(this Type? type) => Implements(type, typeof(T));

    /// <summary>
    /// Is this <paramref name="type"/> a <c>static</c> <see cref="Type"/>?
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsStatic(this Type type) => type is { IsAbstract: true, IsSealed: true };

    /// <summary>
    /// Can instances of this <see cref="Type"/> be <c>null</c>?
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to examine</param>
    /// <returns>
    /// <c>true</c> if instances of this <see cref="Type"/> can contain <c>null</c><br/>
    /// <c>false</c> if they cannot
    /// </returns>
    /// <remarks>
    /// <c>class</c>, <c>interface</c>, and <see cref="Nullable{T}"/> instances can contain <c>null</c><br/>
    /// <c>enum</c>, <c>struct</c>, and other value types instances cannot
    /// </remarks>
    public static bool CanContainNull(this Type? type)
    {
        if (type is null)
            return true;
        if (type.IsStatic())
            return false;
        if (type.IsValueType)
            return Nullable.GetUnderlyingType(type) is not null;
        return true; // non-value types can always contain null
    }

    public static bool CanContainNull<T>()
    {
        if (typeof(T).IsValueType)
            return Nullable.GetUnderlyingType(typeof(T)) is not null;
        return true; // non-value types can always contain null
    }
}
