// Collections should implement generic interface

#pragma warning disable CA1010
// Identifiers should have correct suffix
#pragma warning disable CA1710

namespace ScrubJay.Collections;

/// <summary>
/// An <see cref="IEnumerator{T}"/> that yields nothing
/// </summary>
[PublicAPI]
[MustDisposeResource(false)]
public sealed class Empty<T> :
    IHasDefault<Empty<T>>,
    IEnumerable<T>,
    IEnumerable,
    IEnumerator<T>,
    IEnumerator,
    IDisposable
{
    public static Empty<T> Default { get; } = new();

    object IEnumerator.Current => throw Ex.Invalid("Empty");

    T IEnumerator<T>.Current => throw Ex.Invalid("Empty");

    IEnumerator IEnumerable.GetEnumerator() => this;

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => this;

    bool IEnumerator.MoveNext() => false;

    void IEnumerator.Reset() { }

    void IDisposable.Dispose() { }
}
