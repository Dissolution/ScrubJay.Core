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
    private static Action MoveNext<SM>(
        ref SM stateMachine)
        where SM : IAsyncStateMachine
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
    public void Start<SM>(ref SM stateMachine)
        where SM : IAsyncStateMachine
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
    public void AwaitOnCompleted<A, SM>(
        ref A awaiter,
        ref SM stateMachine)
        where A : INotifyCompletion
        where SM : IAsyncStateMachine
        => awaiter.OnCompleted(MoveNext(ref stateMachine));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AwaitUnsafeOnCompleted<A, SM>(
        ref A awaiter,
        ref SM stateMachine)
        where A : ICriticalNotifyCompletion
        where SM : IAsyncStateMachine
        => awaiter.UnsafeOnCompleted(MoveNext(ref stateMachine));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetStateMachine(IAsyncStateMachine stateMachine)
    {
        Debugger.Break();
        throw new NotSupportedException();
    }
}
