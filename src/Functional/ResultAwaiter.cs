namespace ScrubJay.Functional;

[PublicAPI]
public struct ResultAwaiter<T> :
#if NET7_0_OR_GREATER
    IEqualityOperators<ResultAwaiter<T>, ResultAwaiter<T>, bool>,
#endif
    ICriticalNotifyCompletion,
    INotifyCompletion,
    IEquatable<ResultAwaiter<T>>
{
    public static bool operator ==(ResultAwaiter<T> left, ResultAwaiter<T> right)
        => left.Equals(right);

    public static bool operator !=(ResultAwaiter<T> left, ResultAwaiter<T> right)
        => !left.Equals(right);

    private readonly Result<T> _result;

    public ResultAwaiter(Result<T> result)
    {
        _result = result;
    }

    public bool IsCompleted => true;

    public T GetResult() => _result.OkOrThrow();

    public void OnCompleted(Action continuation)
    {
        continuation();
    }

    public void UnsafeOnCompleted(Action continuation)
    {
        continuation();
    }

    public bool Equals(ResultAwaiter<T> other)
    {
        return _result == other._result;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is ResultAwaiter<T> awaiter && Equals(awaiter);
    }

    public override int GetHashCode()
    {
        return _result.GetHashCode();
    }

    public override string ToString()
    {
        return _result.ToString();
    }
}