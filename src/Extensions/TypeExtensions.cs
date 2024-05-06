using System.Reflection;

namespace ScrubJay.Extensions;

/// <summary>
/// Extensions on <see cref="Type"/> and <see cref="TypeInfo"/>
/// </summary>
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
    /// Does <c>this</c> <paramref name="type"/> implement the <paramref name="checkType"/>?
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to examine</param>
    /// <param name="checkType">The <see cref="Type"/> that this <paramref name="type"/> must implement</param>
    /// <returns>
    /// <c>true</c> if <paramref name="type"/> implements <paramref name="checkType"/>; otherwise, <c>false</c>
    /// </returns>
    public static bool Implements(this Type? type, Type? checkType)
    {
        if (checkType is null) return type is null;
        if (type is null) return false;

        // Shortcut a bunch of checks
        if (checkType.IsAssignableFrom(type)) return true;
        
        if (checkType.IsGenericTypeDefinition)
        {
            // Check direct (List<T> : List<>)
            if (type.IsGenericType && type.GetGenericTypeDefinition() == checkType)
                return true;

            // Check my base types
            foreach (var baseType in type.GetBaseTypes())
            {
                if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == checkType)
                    return true;
            }

            // Check my interfaces
            foreach (var iface in type.GetInterfaces())
            {
                if (iface.IsGenericType && iface.GetGenericTypeDefinition() == checkType)
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
    public static bool IsStatic(this Type type)
    {
        return type is { IsAbstract: true, IsSealed: true };
    }
    
    /// <summary>
    /// Can values of this <see cref="Type"/> contain <c>null</c>?
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to examine</param>
    /// <returns>
    /// <c>true</c> if instances of this <see cref="Type"/> can contain <c>null</c> (such as classes and Nullable&lt;&gt;)<br/>
    /// <c>false</c> if they cannot (value types)
    /// </returns>
    public static bool CanContainNull(this Type? type)
    {
        if (type is null) return true;
        if (type.IsStatic()) return false;
        if (type.IsValueType)
            return Nullable.GetUnderlyingType(type) is not null;
        return true; // non-value types can always contain null
    }

    /// <summary>
    /// Is this <see cref="Type"/> a <see cref="Nullable{T}"/>?
    /// </summary>
    public static bool IsNullable([AllowNull, NotNullWhen(true)] this Type? type)
    {
        return type is { IsValueType: true, IsGenericType: true }
            && type.GetGenericTypeDefinition() == typeof(Nullable<>);
    }

    /// <summary>
    /// Is this <see cref="Type"/> a <see cref="Nullable{T}"/>?<br/>
    /// If so, also return the <paramref name="underlyingType"/>
    /// </summary>
    public static bool IsNullable(this Type? type, [NotNullWhen(true)] out Type? underlyingType)
    {
        if (type.IsNullable())
        {
            underlyingType = type.GetGenericArguments()[0];
            return true;
        }

        underlyingType = null;
        return false;
    }

    /// <inheritdoc cref="Type.MakeGenericType"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Type MakeGenericType<T>(this Type type)
    {
        return type.MakeGenericType(typeof(T));
    }

    /// <summary>
    /// Is this <see cref="Type"/> a <c>ref</c>?<br/>
    /// If so, also return the <paramref name="underlyingType"/>
    /// </summary>
    /// <param name="type"></param>
    /// <param name="underlyingType"></param>
    /// <returns></returns>
    public static bool IsByRef(this Type? type, [NotNullWhen(true)] out Type? underlyingType)
    {
        if (type is null)
        {
            underlyingType = null;
            return false;
        }

        if (type.IsByRef)
        {
            underlyingType = type.GetElementType()!;
            return true;
        }

        underlyingType = type;
        return false;
    }

    public static IReadOnlyList<MemberInfo> AllMembers(this Type type)
    {
        return type.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
    }
}