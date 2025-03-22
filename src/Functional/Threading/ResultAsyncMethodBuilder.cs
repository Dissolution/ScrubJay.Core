#pragma warning disable CA1000, CA1045, CA1815
#pragma warning disable IDE0060, IDE0251

namespace ScrubJay.Functional.Threading;

// https://devblogs.microsoft.com/dotnet/how-async-await-really-works/

[PublicAPI]
[StructLayout(LayoutKind.Auto)]
public struct ResultAsyncMethodBuilder<T>
{
    public static ResultAsyncMethodBuilder<T> Create() => new();

    private static Action CreateCompletionAction<TStateMachine>(
        ref TStateMachine stateMachine)
        where TStateMachine : IAsyncStateMachine
    {
        Debugger.Break();
        var boxedStateMachine = stateMachine;
        return boxedStateMachine.MoveNext;
    }


    // ^static   instance_v

    private Result<T> _result;

    public Result<T> Task
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _result;
    }

    // no ctor


    public void Start<TStateMachine>(ref TStateMachine stateMachine)
        where TStateMachine : IAsyncStateMachine
    {
#if NET5_0_OR_GREATER
        ExecutionContext? ctx = ExecutionContext.Capture();
        try
        {
            stateMachine.MoveNext();
        }
        finally
        {
            if (ctx is not null)
            {
                ExecutionContext.Restore(ctx);
            }
        }
#else
        stateMachine.MoveNext();
#endif
    }

    public void SetResult(T result)
    {
        Debug.Assert(_result == default(Result<T, Exception>));
        _result = Result<T>.Ok(result);
    }

    public void SetException(Exception exception)
    {
        Debug.Assert(_result == default(Result<T, Exception>));
        _result = Result<T>.Error(exception);
    }


    public void AwaitOnCompleted<TAwaiter, TStateMachine>(
        ref TAwaiter awaiter,
        ref TStateMachine stateMachine)
        where TAwaiter : INotifyCompletion
        where TStateMachine : IAsyncStateMachine
    {
        var completionAction = CreateCompletionAction(ref stateMachine);
        awaiter.OnCompleted(completionAction);
    }

    public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(
        ref TAwaiter awaiter,
        ref TStateMachine stateMachine)
        where TAwaiter : ICriticalNotifyCompletion
        where TStateMachine : IAsyncStateMachine
    {
        var completionAction = CreateCompletionAction(ref stateMachine);
        awaiter.UnsafeOnCompleted(completionAction);
    }

    public void SetStateMachine(IAsyncStateMachine stateMachine)
    {
        // Do we get here?
        Debugger.Break();
        throw new InvalidOperationException();
    }
}
