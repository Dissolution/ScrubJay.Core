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
    /// Casts this <see cref="object"/> <c>as</c> a <typeparamref name="TOut"/> value and returns it
    /// </summary>
    /// <param name="obj">
    /// The <see cref="object"/> to convert to a <typeparamref name="TOut"/> value
    /// </param>
    /// <param name="objName">
    /// The automatically captured name of the <paramref name="obj"/> argument, used for a thrown <see cref="ArgumentNullException"/>
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
    public static TOut ThrowIfNot<TOut>([NotNull] this object? obj,
        [CallerArgumentExpression(nameof(obj))] string? objName = null)
    {
        return obj switch
        {
            null => throw new ArgumentNullException(objName),
            TOut output => output,
            _ => throw new ArgumentException($"The given {obj.GetType().NameOf()} value is not a valid {typeof(TOut).NameOf()} instance", objName)
        };
    }


    [return: NotNullIfNotNull(nameof(obj))]
    public static TOut? ThrowIfCannotBe<TOut>(
        this object? obj,
        [CallerArgumentExpression(nameof(obj))] string? objName = null)
    {
        if (obj.Is<TOut?>(out TOut? output))
            return output;
        throw new ArgumentException($"The given {obj?.GetType().NameOf()} value is not a valid {typeof(TOut).NameOf()} instance", objName);
    }
}
