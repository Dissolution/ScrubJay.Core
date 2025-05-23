﻿// Collections should implement generic interface

#pragma warning disable CA1010
// Identifiers should have correct suffix
#pragma warning disable CA1710

namespace ScrubJay.Collections;

/// <summary>
/// An <see cref="IEnumerator{T}"/> that yields nothing
/// </summary>
[PublicAPI]
[MustDisposeResource(false)]
public sealed class EmptyEnumerable<T> :
    IHasDefault<EmptyEnumerable<T>>,
    IEnumerable<T>,
    IEnumerable,
    IEnumerator<T>,
    IEnumerator,
    IDisposable
{
    public static EmptyEnumerable<T> Default { get; } = new();

    object IEnumerator.Current => throw new InvalidOperationException("Empty");

    T IEnumerator<T>.Current => throw new InvalidOperationException("Empty");

    IEnumerator IEnumerable.GetEnumerator() => this;

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => this;

    bool IEnumerator.MoveNext() => false;

    void IEnumerator.Reset() { }

    void IDisposable.Dispose() { }
}
