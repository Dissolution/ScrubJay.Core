// ReSharper disable EntityNameCapturedOnly.Global
namespace ScrubJay.Extensions;

public static class ObjectExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Is<T>(this object? input, [NotNullWhen(true)] out T? output)
    {
        if (input is T)
        {
            output = (T)input;
            return true;
        }
        output = default;
        return false;
    }

    public static bool CanBe<T>(this object? obj)
    {
        switch (obj)
        {
            case T:
            case null when typeof(T).CanContainNull():
                return true;
            default:
                return false;
        }
    }
    
    /// <summary>
    /// If this <see cref="object"/> can be a <typeparamref name="T"/> value,
    /// cast it to that value and return <c>true</c>.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <remarks>
    /// This differs from the <c>is</c> keyword in that,
    /// if <paramref name="obj"/> is <c>null</c> and <typeparamref name="T"/> can contain <c>null</c>,
    /// returning <c>null</c> as <paramref name="value"/> is valid.
    /// </remarks>
    public static bool CanBe<T>(this object? obj, out T? value)
    {
        if (obj is T)
        {
            value = (T)obj;
            return true;
        }
        value = default;
        return obj is null && typeof(T).CanContainNull();
    }
    
    /// <summary>
    /// Unbox this <see cref="object"/> into a <typeparamref name="T"/> value,
    /// throwing an <see cref="ArgumentException"/> if it is not a valid <typeparamref name="T"/> instance
    /// </summary>
    /// <param name="obj">The <see cref="object"/> to unbox</param>
    /// <param name="objName">The name of the <paramref name="obj"/> parameter, passed to a thrown <see cref="ArgumentException"/></param>
    /// <typeparam name="T">The <see cref="Type"/> of value to unbox to</typeparam>
    /// <returns>
    /// <paramref name="obj"/> unboxed to a <typeparamref name="T"/> value
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if <see cref="obj"/> is not a valid <typeparamref name="T"/> instance
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Unbox<T>(this object? obj, [CallerArgumentExpression(nameof(obj))] string? objName = null)
    {
        if (obj is T)
            return (T)obj;
        throw new ArgumentException($"Object '{obj}' is not a {typeof(T).Name}", objName);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T UnboxRef<T>(this object? obj, [CallerArgumentExpression(nameof(obj))] string? objName = null)
    {
        if (obj is T)
            return ref Scary.UnboxRef<T>(obj);
        throw new ArgumentException($"Object '{obj}' is not a {typeof(T).Name}", objName);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNullIfNotNull(nameof(obj))]
    public static T? CastClass<T>(this object? obj, [CallerArgumentExpression(nameof(obj))] string? objName = null)
        where T : class
    {
        if (obj is null)
            return null;
        if (obj is T)
            return (T)obj;
        throw new ArgumentException($"Object '{obj}' is not a {typeof(T).Name}", objName);
    }
}