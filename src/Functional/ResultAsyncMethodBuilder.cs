#pragma warning disable CA1000, CA1045, CA1815
#pragma warning disable IDE0060, IDE0251

namespace ScrubJay.Functional;

/// <summary>
/// An <c>AsyncMethodBuilder</c> that works on <see cref="Result{T}"/>
/// </summary>
/// <remarks>
/// <a href="https://devblogs.microsoft.com/dotnet/how-async-await-really-works/"/><br/>
/// <a href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/classes#15152-task-type-builder-pattern"/><br/>
/// </remarks>
[PublicAPI]
[StructLayout(LayoutKind.Auto)]
public struct ResultAsyncMethodBuilder<T>
{
    /// <summary>
    /// Creates a new <see cref="ResultAsyncMethodBuilder{T}"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ResultAsyncMethodBuilder<T> Create() => new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Action MoveNext<TStateMachine>(
        ref TStateMachine stateMachine)
        where TStateMachine : IAsyncStateMachine
    {
        // dereference
        var smInstance = stateMachine;
        return smInstance.MoveNext;
    }


    private Result<T> _result;

    /// <summary>
    /// Gets the <see cref="Result{T}"/> this <see cref="ResultAsyncMethodBuilder{T}"/> is building
    /// </summary>
    public Result<T> Task
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _result;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Start<TStateMachine>(ref TStateMachine stateMachine)
        where TStateMachine : IAsyncStateMachine
        => stateMachine.MoveNext();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetResult(T result)
    {
        Debug.Assert(_result == default(Result<T>));
        _result = Result<T>.Ok(result);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetException(Exception exception)
    {
        Debug.Assert(_result == default(Result<T>));
        _result = Result<T>.Error(exception);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AwaitOnCompleted<TAwaiter, TStateMachine>(
        ref TAwaiter awaiter,
        ref TStateMachine stateMachine)
        where TAwaiter : INotifyCompletion
        where TStateMachine : IAsyncStateMachine
        => awaiter.OnCompleted(MoveNext(ref stateMachine));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(
        ref TAwaiter awaiter,
        ref TStateMachine stateMachine)
        where TAwaiter : ICriticalNotifyCompletion
        where TStateMachine : IAsyncStateMachine
        => awaiter.UnsafeOnCompleted(MoveNext(ref stateMachine));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetStateMachine(IAsyncStateMachine stateMachine)
    {
        Debugger.Break();
        throw new NotSupportedException();
    }
}
