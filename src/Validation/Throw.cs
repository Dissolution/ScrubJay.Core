﻿using ScrubJay.Collections;

namespace ScrubJay.Validation;

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

    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void IfEnumerationChanged([DoesNotReturnIf(true)] bool areDifferent)
    {
        if (areDifferent)
        {
            throw new InvalidOperationException("Enumeration failed: Source has changed");
        }
    }

    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void IfConcurrentOperation([DoesNotReturnIf(true)] bool isConcurrent)
    {
        if (isConcurrent)
        {
            throw new InvalidOperationException("Concurrent operations are not supported");
        }
    }


    [StackTraceHidden]
    public static void IfBadEnumerationState<T>(
        T value,
        Bounds<T> notStarted,
        Bounds<T> finished)
    {
        if (notStarted.Contains(value))
            throw new InvalidOperationException("Enumeration has not yet started");
        if (finished.Contains(value))
            throw new InvalidOperationException("Enumeration has already finished");
    }


    [StackTraceHidden]
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void KeyNotFound<TKey>(TKey key)
    {
        throw new KeyNotFoundException($"Key '{key}' was not found");
    }

    [StackTraceHidden]
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DuplicateKeyException<TKey>(TKey key,
        [CallerMemberName] string? keyName = null)
    {
        throw new ArgumentException($"Duplicate key '{key}' was found", keyName);
    }


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
}