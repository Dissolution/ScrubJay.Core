﻿using ScrubJay.Reflection;

namespace ScrubJay.Validation;

public static class ValidationExtensions
{
    /// <summary>
    /// Throws an <see cref="ArgumentNullException"/> if <paramref name="value"/> is <c>null</c>
    /// </summary>
    /// <typeparam name="T">
    /// The <see cref="Type"/> of <paramref name="value"/>
    /// </typeparam>
    /// <param name="value">
    /// The value to check for <c>null</c>
    /// </param>
    /// <param name="valueName">
    /// The name of the <paramref name="value"/> argument, passed to <see cref="ArgumentNullException"/>
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
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
        where T : class?
    {
        Throw.IfNull<T>(value, valueName);
        return value;
    }

    /// <summary>
    /// Casts this <see cref="object"/> <c>as</c> a <typeparamref name="TOut"/> value and returns it
    /// </summary>
    /// <param name="obj">
    /// The <see cref="object"/> to convert to a <typeparamref name="TOut"/> value
    /// </param>
    /// <param name="objName">
    /// The captured name for the <paramref name="obj"/> parameter, used with an <see cref="ArgumentException"/>
    /// </param>
    /// <typeparam name="TOut">
    /// The <see cref="Type"/> of value to cast <paramref name="obj"/> <c>as</c>
    /// </typeparam>
    /// <returns>
    /// <c>obj as TOut</c>
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown is <paramref name="obj"/> is not a valid <typeparamref name="TOut"/> value
    /// </exception>
    [return: NotNull]
    public static TOut AsValid<TOut>(
        [AllowNull, NotNull]
        this object? obj,
        [CallerArgumentExpression(nameof(obj))]
        string? objName = null)
    {
        if (obj is TOut output)
            return output;

        throw new ArgumentException(
            $"The given {obj?.GetType().NameOf()} value is not a valid {typeof(TOut).NameOf()} instance",
            objName);
    }
}