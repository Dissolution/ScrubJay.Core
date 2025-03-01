namespace ScrubJay.Functional;

[PublicAPI]
public interface ITryEnumerator<T> : IEnumerator<T>, IEnumerator, IDisposable
{
    Result<T, Exception> TryMoveNext();

#if !NETFRAMEWORK && !NETSTANDARD2_0
    bool IEnumerator.MoveNext() => TryMoveNext();
#endif
}
