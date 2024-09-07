#pragma warning disable CA1031

namespace ScrubJay;

[PublicAPI]
public static class Result
{
    /// <summary>
    /// Tries to dispose an <see cref="IDisposable"/> <paramref name="instance"/>
    /// </summary>
    /// <param name="instance">The <typeparamref name="T"/> instace to dispose</param>
    /// <typeparam name="T">The <see cref="Type"/> of instance to dispose</typeparam>
    /// <returns>
    /// A <see cref="Result{Unit,Exception}">Result</see>&lt;<see cref="Unit"/>, <see cref="Exception"/>&gt; describing the disposal 
    /// </returns>
    [HandlesResourceDisposal]
    public static Result<Unit, Exception> TryDispose<T>(T? instance)
        where T : IDisposable
    {
        if (instance is null)
            return new ArgumentNullException(nameof(instance));
        
        try
        {
            instance.Dispose();
            return Unit.Default;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    /// <summary>
    /// Try to invoke an <paramref name="action"/>
    /// </summary>
    /// <param name="action">The <see cref="Action"/> to invoke</param>
    /// <returns>
    /// A <see cref="Result{Unit,Exception}">Result</see>&lt;<see cref="Unit"/>, <see cref="Exception"/>&gt; describing the invocation
    /// </returns>
    public static Result<Unit, Exception> TryAction(Action? action)
    {
        if (action is null)
            return new ArgumentNullException(nameof(action));
        
        try
        {
            action.Invoke();
            return Unit.Default;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    /// <summary>
    /// Try to invoke an <paramref name="instance"/> <paramref name="action"/>
    /// </summary>
    /// <param name="instance">The instance to perform the action upon</param>
    /// <param name="action">The <see cref="Action"/> to invoke</param>
    /// <returns>
    /// A <see cref="Result{Unit,Exception}">Result</see>&lt;<see cref="Unit"/>, <see cref="Exception"/>&gt; describing the invocation
    /// </returns>
    public static Result<Unit, Exception> TryAction<TInstance>(
        [NotNullWhen(true)] TInstance? instance, 
        [NotNullWhen(true)] Action<TInstance>? action)
    {
        if (instance is null)
            return new ArgumentNullException(nameof(instance));
        if (action is null)
            return new ArgumentNullException(nameof(action));
        
        try
        {
            action.Invoke(instance);
            return Unit.Default;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    /// <summary>
    /// Try to invoke an <paramref name="func"/>
    /// </summary>
    /// <param name="func">The <see cref="Func{TResult}"/> to invoke</param>
    /// <returns>
    /// A <see cref="Result{TResult,Exception}">Result</see>&lt;<typeparamref name="TResult"/>, <see cref="Exception"/>&gt; describing the invocation
    /// </returns>
    public static Result<TResult, Exception> TryFunc<TResult>(Func<TResult>? func)
    {
        if (func is null)
            return new ArgumentNullException(nameof(func));
        
        try
        {
            return func.Invoke();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    /// <summary>
    /// Try to invoke an <paramref name="func"/>
    /// </summary>
    /// <param name="instance">The instance to perform the function upon</param>
    /// <param name="func">The <see cref="Func{TResult}"/> to invoke</param>
    /// <returns>
    /// A <see cref="Result{TResult,Exception}">Result</see>&lt;<typeparamref name="TResult"/>, <see cref="Exception"/>&gt; describing the invocation
    /// </returns>
    public static Result<TResult, Exception> TryFunc<TInstance, TResult>(
        [NotNullWhen(true)] TInstance? instance, 
        [NotNullWhen(true)] Func<TInstance, TResult>? func)
    {
        if (instance is null)
            return new ArgumentNullException(nameof(instance));
        if (func is null)
            return new ArgumentNullException(nameof(func));
        
        try
        {
            return func.Invoke(instance);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
}