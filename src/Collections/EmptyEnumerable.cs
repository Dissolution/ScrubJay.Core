// Collections should implement generic interface
#pragma warning disable CA1010
// Identifiers should have correct suffix
#pragma warning disable CA1710

namespace ScrubJay.Collections;

[PublicAPI]
[MustDisposeResource(false)]
public class EmptyEnumerable :
    IHasDefault<EmptyEnumerable>,
    IEnumerable, IEnumerator, IDisposable
{
    public static EmptyEnumerable Default { get; } = new();

    object IEnumerator.Current => throw new InvalidOperationException("This enumerator is empty");

    void IEnumerator.Reset() { }
    void IDisposable.Dispose() { }

    public bool MoveNext() => false;
    public IEnumerator GetEnumerator() => this;
}


/// <summary>
/// An <see cref="IEnumerator{T}"/> that yields nothing
/// </summary>
[PublicAPI]
[MustDisposeResource(false)]
public sealed class EmptyEnumerable<T> :
    EmptyEnumerable,
    IHasDefault<EmptyEnumerable<T>>,
    IEnumerator<T>
{
    public static new EmptyEnumerable<T> Default { get; } = new();

    T IEnumerator<T>.Current => throw new InvalidOperationException("This enumerator is empty");
}
