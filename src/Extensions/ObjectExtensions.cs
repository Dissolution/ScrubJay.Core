#pragma warning disable S3247

// ReSharper disable UseSwitchCasePatternVariable
// ReSharper disable MergeCastWithTypeCheck
namespace ScrubJay.Extensions;

/// <summary>
/// Extensions on <see cref="object"/>
/// </summary>
[PublicAPI]
public static class ObjectExtensions
{
    /// <summary>
    /// If the <paramref name="input"/> <see cref="object"/> <c>is</c><sup>1</sup> a <typeparamref name="T"/> value,<br/>
    /// stores that value in <paramref name="output"/> and returns <c>true</c><br/>
    /// otherwise sets <paramref name="output"/> to <c>default(T)</c> and returns <c>false</c>
    /// </summary>
    /// <param name="input"></param>
    /// <param name="output"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <remarks>
    /// <sup>1</sup> - <c>Unbox</c> or <c>Cast class</c>
    /// </remarks>
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

    public static Option<T> Is<T>(this object? input)
    {
        if (input is T)
        {
            return Some((T)input);
        }
        return None();
    }

    public static bool As<T>(this object? input, [NotNullIfNotNull(nameof(input))] out T? output)
    {
        if (input is T)
        {
            output = (T)input;
            return true;
        }

        output = default;
        return typeof(T).CanContainNull();
    }

    public static Option<T?> As<T>(this object? input)
    {
        if (input is T)
        {
            return Some<T?>((T)input);
        }

        if (input is null && typeof(T).CanContainNull())
        {
            return Some<T?>(default(T));
        }

        return None();
    }

    public static Option<object?> As(this object? input, Type? type)
    {
        if (input is null)
            return type.CanContainNull() ? Some<object?>(null) : None();
        if (input.GetType().Implements(type))
            return Some<object?>(input);
        return None();
    }

    public static Option<T> NotNull<T>(this T? input)
    {
        if (input is not null)
            return Some<T>(input);
        return None<T>();
    }
}
