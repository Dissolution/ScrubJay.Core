namespace ScrubJay.Collections;

/// <summary>
/// An <see cref="IEnumerator{T}"/> that yields a single value
/// </summary>
[PublicAPI]
[MustDisposeResource(false)]
public sealed class SingleValueEnumerator<T> : IEnumerator<T>, IEnumerator, IDisposable
{
    private readonly T _value;
    private bool _canYield;

    object? IEnumerator.Current => _value;
    public T Current => _value;

    public SingleValueEnumerator(T value)
    {
        _value = value;
        _canYield = true;
    }

    public bool MoveNext()
    {
        if (!_canYield)
            return false;
        _canYield = false;
        return true;
    }

    public void Reset() => _canYield = true;

    void IDisposable.Dispose() { /* Do Nothing */ }
}
