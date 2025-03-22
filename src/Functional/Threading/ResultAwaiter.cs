// https://devblogs.microsoft.com/dotnet/how-async-await-really-works/

namespace ScrubJay.Functional.Threading;

[PublicAPI]
[StructLayout(LayoutKind.Auto)]
public readonly struct ResultAwaiter<T> :
    IEquatable<ResultAwaiter<T>>,
    ICriticalNotifyCompletion,
    INotifyCompletion
{
    public static bool operator ==(ResultAwaiter<T> left, ResultAwaiter<T> right) => left.Equals(right);

    public static bool operator !=(ResultAwaiter<T> left, ResultAwaiter<T> right) => !left.Equals(right);


    private readonly Result<T> _result;

    public bool IsCompleted => true;

    public ResultAwaiter(Result<T> result)
    {
        _result = result;
    }

    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T GetResult() => _result.OkOrThrow();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void OnCompleted(Action continuation) => continuation();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UnsafeOnCompleted(Action continuation) => continuation();

    public bool Equals(ResultAwaiter<T> other) => other._result == this._result;

    public override bool Equals(object? obj) => obj is ResultAwaiter<T> other && Equals(other);

    public override int GetHashCode() => _result.GetHashCode();

    public override string ToString() => $"await {_result}";
}
