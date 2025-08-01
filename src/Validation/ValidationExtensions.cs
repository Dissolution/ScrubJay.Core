﻿using ScrubJay.Text.Rendering;

namespace ScrubJay.Validation;

/// <summary>
/// Extensions related to validation
/// </summary>
[PublicAPI]
public static class ValidationExtensions
{
    /// <summary>
    /// Returns <paramref name="value"/> or throws an <see cref="ArgumentNullException"/> if it is <c>null</c>
    /// </summary>
    /// <typeparam name="T">
    /// The <see cref="Type"/> of <paramref name="value"/>
    /// </typeparam>
    /// <param name="value">
    /// The value to check for <c>null</c>
    /// </param>
    /// <param name="message">
    /// The optional message used for a thrown <see cref="ArgumentNullException"/>
    /// </param>
    /// <param name="valueName">
    /// The automatically captured name of the <paramref name="value"/> argument, used for a thrown <see cref="ArgumentNullException"/>
    /// </param>
    /// <returns>
    /// A non-<c>null</c> <typeparamref name="T"/> <paramref name="value"/>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="value"/> is <c>null</c>
    /// </exception>
    [return: NotNull]
    public static T ThrowIfNull<T>(
        [AllowNull, NotNull] this T value,
        string? message = null,
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
        where T : class?
    {
        if (value is not null)
            return value;
        throw new ArgumentNullException(valueName, message);
    }

    /// <summary>
    /// Casts this <see cref="object"/> <c>as</c> a <typeparamref name="T"/> value and returns it
    /// </summary>
    /// <param name="obj">
    /// The <see cref="object"/> to convert to a <typeparamref name="T"/> value
    /// </param>
    /// <param name="objName">
    /// The automatically captured name of the <paramref name="obj"/> argument, used for a thrown <see cref="ArgumentNullException"/>
    /// </param>
    /// <typeparam name="T">
    /// The <see cref="Type"/> of value to cast <paramref name="obj"/> <c>as</c>
    /// </typeparam>
    /// <returns>
    /// <c>obj as TOut</c>
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown is <paramref name="obj"/> is not a valid <typeparamref name="T"/> value
    /// </exception>
    [return: NotNull]
    public static T ThrowIfNot<T>([NotNull] this object? obj,
        [CallerArgumentExpression(nameof(obj))]
        string? objName = null)
    {
        return obj switch
        {
            null => throw new ArgumentNullException(objName),
            T output => output,
            _ => throw new ArgumentException(
                TextBuilder.Build($"(object){obj.GetType():@} `{obj:@}` is not a {typeof(T):@} instance"), objName),
        };
    }

    [return: NotNullIfNotNull(nameof(obj))]
    [return: NotNullIfNotNull(nameof(type))]
    public static object? ThrowIfNot(this object? obj,
        Type? type,
        [CallerArgumentExpression(nameof(obj))]
        string? objName = null)
    {
        if (type is null)
        {
            if (obj is null)
                return obj;
            throw new ArgumentException(
                TextBuilder.Build($"(object){obj.GetType():@} `{obj:@}` was expected to be null"), objName);
        }

        if (obj is null)
            throw new ArgumentNullException(objName);

        if (obj.GetType().Implements(type))
            return obj;

        throw new ArgumentException(
            TextBuilder.Build($"(object){obj.GetType():@} `{obj:@}` is not a {type:@} instance"), objName);
    }


    [return: NotNullIfNotNull(nameof(obj))]
    public static T? ThrowIfCannotBe<T>(
        this object? obj,
        [CallerArgumentExpression(nameof(obj))]
        string? objName = null)
    {
        if (obj.As<T?>(out T? output))
            return output;
        throw new ArgumentException($"The given {obj?.GetType().Render()} value is not a valid {typeof(T).Render()} instance",
            objName);
    }
}