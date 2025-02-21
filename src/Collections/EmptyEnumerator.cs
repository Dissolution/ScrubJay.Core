namespace ScrubJay.Collections;

/// <summary>
/// An <see cref="IEnumerator{T}"/> that yields nothing
/// </summary>
[PublicAPI]
[MustDisposeResource(false)]
public sealed class EmptyEnumerator<T> : IEnumerator<T>, IEnumerator, IDisposable
{
    object IEnumerator.Current => throw new InvalidOperationException("This enumerator is empty");
    T IEnumerator<T>.Current => throw new InvalidOperationException("This enumerator is empty");

    bool IEnumerator.MoveNext() => false;
    void IEnumerator.Reset() { }

    void IDisposable.Dispose() { }
}
