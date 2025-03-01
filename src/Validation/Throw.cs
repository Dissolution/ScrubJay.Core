// CA1716: Identifiers should not match keywords
#pragma warning disable CA1716

namespace ScrubJay.Validation;

/// <summary>
/// Methods that throw an <seealso cref="Exception"/> if certain conditions are met
/// </summary>
[PublicAPI]
[StackTraceHidden]
public static class Throw
{
    /// <summary>
    /// Throw a <see cref="ObjectDisposedException"/> if a <paramref name="condition"/> indicates disposal of an <paramref name="instance"/>
    /// </summary>
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void IfDisposed<T>([DoesNotReturnIf(true)] bool condition, T? instance)
    {
        if (condition)
        {
            string? objectName = (instance?.GetType().FullName) ?? typeof(T).FullName;
            throw new ObjectDisposedException(objectName);
        }
    }

#region Enumeration
    /// <summary>
    /// Throws an <see cref="InvalidOperationException"/> that indicates an <see cref="IEnumerator{T}"/> has deviated from its source
    /// </summary>
    /// <param name="hasChanged"></param>
    /// <exception cref="InvalidOperationException"></exception>
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void IfEnumerationSourceHasChanged([DoesNotReturnIf(true)] bool hasChanged)
    {
        if (hasChanged)
        {
            throw new InvalidOperationException("Enumeration Failed: Source has changed");
        }
    }

    [StackTraceHidden]
    public static void IfBadEnumerationState(
        [DoesNotReturnIf(true)] bool badState)
    {
        if (badState)
            throw new InvalidOperationException("Enumeration has not yet started or has finished");
    }

    [StackTraceHidden]
    public static void IfBadEnumerationState(
        [DoesNotReturnIf(true)] bool notYetStarted,
        [DoesNotReturnIf(true)] bool hasFinished)
    {
        if (notYetStarted)
            throw new InvalidOperationException("Enumeration has not yet started");
        if (hasFinished)
            throw new InvalidOperationException("Enumeration has finished");
    }
#endregion

    [StackTraceHidden]
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void KeyNotFound<TKey>(TKey key) => throw new KeyNotFoundException($"Key '{key}' was not found");

    [StackTraceHidden]
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DuplicateKeyException<TKey>(
        TKey key,
        [CallerMemberName] string? keyName = null)
        => throw new ArgumentException($"Duplicate key '{key}' was found", keyName);


    /// <summary>
    /// Throw an <see cref="ArgumentNullException"/> if <paramref name="value"/> is <c>null</c>
    /// </summary>
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void IfNull<T>(
        [NotNull, NoEnumeration] T? value,
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
        where T : class?
    {
        if (value is null)
            throw new ArgumentNullException(valueName);
    }

    /// <summary>
    /// Throw an <see cref="ArgumentException"/> if a <see cref="string"/> is empty
    /// </summary>
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void IfEmpty(
        [NotNull] string? str,
        [CallerArgumentExpression(nameof(str))]
        string? stringName = null)
    {
        if (str is null)
            throw new ArgumentNullException(stringName);
        if (str.Length == 0)
            throw new ArgumentException("String cannot be empty", stringName);
    }

    /// <summary>
    /// Throw an <see cref="ArgumentException"/> if an <see cref="Array"/> is empty
    /// </summary>
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void IfEmpty<T>(
        [NotNull] T[]? array,
        [CallerArgumentExpression(nameof(array))]
        string? arrayName = null)
    {
        if (array is null)
            throw new ArgumentNullException(arrayName);
        if (array.Length == 0)
            throw new ArgumentException("Array cannot be empty", arrayName);
    }

    /// <summary>
    /// Throw an <see cref="ArgumentException"/> if a <see cref="Span{T}"/> is empty
    /// </summary>
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void IfEmpty<T>(
        Span<T> span,
        [CallerArgumentExpression(nameof(span))]
        string? spanName = null)
    {
        if (span.Length == 0)
            throw new ArgumentException("Span cannot be empty", spanName);
    }

    /// <summary>
    /// Throw an <see cref="ArgumentException"/> if a <see cref="ReadOnlySpan{T}"/> is empty
    /// </summary>
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void IfEmpty<T>(
        ReadOnlySpan<T> span,
        [CallerArgumentExpression(nameof(span))]
        string? spanName = null)
    {
        if (span.Length == 0)
            throw new ArgumentException("Span cannot be empty", spanName);
    }

    /// <summary>
    /// Throw an <see cref="ArgumentException"/> if an <see cref="ICollection{T}"/> is empty
    /// </summary>
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void IfEmpty<T>(
        [NotNull] ICollection<T>? collection,
        [CallerArgumentExpression(nameof(collection))]
        string? collectionName = null)
    {
        if (collection is null)
            throw new ArgumentNullException(collectionName);
        if (collection.Count == 0)
            throw new ArgumentException("Collection cannot be empty", collectionName);
    }

    [DoesNotReturn]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void IsReadOnly<T>(
        T? instance,
        [CallerArgumentExpression(nameof(instance))]
        string? instanceName = null)
        => throw new NotSupportedException($"{typeof(T).NameOf()} '{instance}' is read-only");

    [DoesNotReturn]
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TReturn IsReadOnly<T, TReturn>(
        T? instance,
        [CallerArgumentExpression(nameof(instance))]
        string? instanceName = null)
        => throw new NotSupportedException($"{typeof(T).NameOf()} '{instance}' is read-only");
}
