namespace ScrubJay.Collections;

public sealed class EmptyEnumerator<T> : IEnumerator<T>, IEnumerator, IDisposable
{
    public static readonly EmptyEnumerator<T> Instance = new();

    object? IEnumerator.Current => default;
    public T Current => default!;

    public bool MoveNext() => false;

    public void Reset()
    {
    }

    public void Dispose()
    {
    }
}