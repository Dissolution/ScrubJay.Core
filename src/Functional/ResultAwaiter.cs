// Override equals and operator equals on value types
#pragma warning disable CA1815

namespace ScrubJay.Functional;

/// <summary>
///
/// </summary>
/// <typeparam name="T"></typeparam>
/// <remarks>
/// <a href="https://devblogs.microsoft.com/dotnet/how-async-await-really-works/"/><br/>
/// <a href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/classes#15151-general"/><br/>
/// </remarks>
[PublicAPI]
[StructLayout(LayoutKind.Auto)]
public readonly struct ResultAwaiter<T> :
    ICriticalNotifyCompletion,
    INotifyCompletion
{
    private readonly Result<T> _result;

    public bool IsCompleted
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
}
