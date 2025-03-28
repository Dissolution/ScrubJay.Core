// CA1716: Identifiers should not match keywords

#pragma warning disable CA1716

namespace ScrubJay.Functional;

[PublicAPI]
public static class Option
{
    /// <summary>
    /// Converts a nullable <paramref name="value"/> to an <see cref="Option{T}"/>
    /// </summary>
    /// <param name="value">
    /// The <typeparamref name="T"/> value that might be <c>null</c>
    /// </param>
    /// <returns>
    /// <see cref="Option{T}.Some"/> containing a non-<c>null</c> <paramref name="value"/> or <br/>
    /// <see cref="Option{T}.None"/> if <paramref name="value"/> is <c>null</c>
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> NotNull<T>(T? value)
        where T : notnull
    {
        if (value is not null)
            return Option<T>.Some(value);
        return Option<T>.None();
    }

    /// <summary>
    /// Converts a <see cref="Nullable{T}"/> to an <see cref="Option{T}"/>
    /// </summary>
    /// <param name="value">
    /// The <see cref="Nullable{T}"/> that might contain <c>null</c>
    /// </param>
    /// <returns>
    /// <see cref="Option{T}.Some"/> containing a non-<c>null</c> <paramref name="value"/> or <br/>
    /// <see cref="Option{T}.None"/> if <paramref name="value"/> is <c>null</c>
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // ReSharper disable once ConvertNullableToShortForm
    public static Option<T> NotNull<T>(Nullable<T> value)
        where T : struct
    {
        if (value.HasValue)
        {
            return Option<T>.Some(value.GetValueOrDefault()); // fastest path to a known value (.Value has an exception path)
        }
        return Option<T>.None();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static None None() => default(None);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> None<T>() => default(Option<T>);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Some<T>(T value) => Option<T>.Some(value);


    public static Option<T> Flatten<T>(this Option<Option<T>> nestedOption)
    {
        if (nestedOption.IsSome(out var option) && option.IsSome(out var value))
            return Some<T>(value);
        return None<T>();
    }
}
