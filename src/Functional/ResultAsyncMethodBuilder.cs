#pragma warning disable CA1000, CA1045, CA1815
#pragma warning disable IDE0060, IDE0251

namespace ScrubJay.Functional;

// https://devblogs.microsoft.com/dotnet/how-async-await-really-works/

/// <summary>
/// An <c>AsyncMethodBuilder</c> that works on <see cref="Result{T}"/>
/// </summary>
[PublicAPI]
[StructLayout(LayoutKind.Auto)]
public struct ResultAsyncMethodBuilder<T>
{
    /// <summary>
    /// Creates a new <see cref="ResultAsyncMethodBuilder{T}"/>
    /// </summary>
    public static ResultAsyncMethodBuilder<T> Create() => new();

    private static Action CreateCompletionAction<TStateMachine>(
        ref TStateMachine stateMachine)
        where TStateMachine : IAsyncStateMachine
    {
        Debugger.Break();
        var boxedStateMachine = stateMachine;
        return boxedStateMachine.MoveNext;
    }


    private Result<T> _result;

    /// <summary>
    /// Gets the <see cref="Task"/> this <see cref="ResultAsyncMethodBuilder{T}"/> is building
    /// </summary>
    public Result<T> Task
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _result;
    }

    // only implicit ctor


    public void Start<TStateMachine>(ref TStateMachine stateMachine)
        where TStateMachine : IAsyncStateMachine
        => stateMachine.MoveNext();

    public void SetResult(T result)
    {
        Debug.Assert(_result == default(Result<T>));
        _result = Result<T>.Ok(result);
    }

    public void SetException(Exception exception)
    {
        Debug.Assert(_result == default(Result<T>));
        _result = Result<T>.Error(exception);
    }


    public void AwaitOnCompleted<TAwaiter, TStateMachine>(
        ref TAwaiter awaiter,
        ref TStateMachine stateMachine)
        where TAwaiter : INotifyCompletion
        where TStateMachine : IAsyncStateMachine
    {
        Debugger.Break();
        var completionAction = CreateCompletionAction(ref stateMachine);
        awaiter.OnCompleted(completionAction);
    }

    public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(
        ref TAwaiter awaiter,
        ref TStateMachine stateMachine)
        where TAwaiter : ICriticalNotifyCompletion
        where TStateMachine : IAsyncStateMachine
    {
        Debugger.Break();
        var completionAction = CreateCompletionAction(ref stateMachine);
        awaiter.UnsafeOnCompleted(completionAction);
    }

    public void SetStateMachine(IAsyncStateMachine stateMachine)
    {
        Debugger.Break();
        throw new NotSupportedException();
    }
}
