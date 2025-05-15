#pragma warning disable CA1710

namespace ScrubJay.Collections;

/// <summary>
/// An <see cref="IEnumerator{T}"/> that yields a single value
/// </summary>
[PublicAPI]
[MustDisposeResource(false)]
public sealed class SingleEnumerable<T> :
    IEnumerable<T>, IEnumerable,
    IEnumerator<T>, IEnumerator, IDisposable
{
    private readonly T _value;
    private bool _canYield;

    object? IEnumerator.Current => _value;

    public T Current => _value;

    public SingleEnumerable(T value)
    {
        _value = value;
        _canYield = true;
    }

    IEnumerator IEnumerable.GetEnumerator() => this;
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => this;
    void IDisposable.Dispose() { /* Do Nothing */ }

    public bool MoveNext()
    {
        if (!_canYield)
            return false;
        _canYield = false;
        return true;
    }

    public void Reset() => _canYield = true;

    public SingleEnumerable<T> GetEnumerator() => this;
}
