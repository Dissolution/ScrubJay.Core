namespace ScrubJay.Functional;

public struct ResultAwaiter<T> : ICriticalNotifyCompletion, INotifyCompletion
{
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
}