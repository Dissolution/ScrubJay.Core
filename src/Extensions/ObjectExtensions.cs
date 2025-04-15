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

    public static bool K<T>(this object? input, [NotNullIfNotNull(nameof(input)), MaybeNullWhen(false)] out T? output)
    {
        if (input is T)
        {
            output = (T)input;
            return true;
        }

        output = default;
        return typeof(T).CanContainNull();
    }

    public static bool CanUnboxAs<T>(this object? input)
    {
        if (input is T)
            return true;
        if (input is null)
            return typeof(T).CanContainNull();
        return false;
    }

    public static bool CanUnboxAs(this object? input, Type? type)
    {
        if (input is null)
            return type.CanContainNull();
        return input.GetType().Implements(type);

    }

    public static Option<T> NotNull<T>(this T? input)
    {
        if (input is not null)
            return Some<T>(input);
        return None<T>();
    }
}
