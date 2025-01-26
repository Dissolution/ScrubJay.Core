namespace ScrubJay.Collections;

/// <summary>
/// An <see cref="IEnumerator{T}"/> that returns a single value
/// </summary>
/// <typeparam name="T"></typeparam>
[PublicAPI]
[MustDisposeResource(false)]
public sealed class SingleValueEnumerator<T> : IEnumerator<T>, IEnumerator, IDisposable
{
    private readonly T _value;
    private bool _canYield;

    object? IEnumerator.Current => _value;
    public T Current => _value;

    public SingleValueEnumerator()
    {
        _value = default!;
        _canYield = false;
    }

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

    void IEnumerator.Reset() => throw new NotSupportedException();

    void IDisposable.Dispose() { /* Do Nothing */ }
}
