namespace ScrubJay;

public static class Result
{
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
    
    public static Result<Unit, Exception> TryAction<TInstance>(
        [AllowNull, NotNullWhen(true)] TInstance? instance, 
        [AllowNull, NotNullWhen(true)] Action<TInstance>? instanceAction)
    {
        if (instance is null)
            return new ArgumentNullException(nameof(instance));
        if (instanceAction is null)
            return new ArgumentNullException(nameof(instanceAction));
        
        try
        {
            instanceAction.Invoke(instance);
            return Unit.Default;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
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
    public static Result<TResult, Exception> TryFunc<TInstance, TResult>(
        [AllowNull, NotNullWhen(true)] TInstance? instance, 
        [AllowNull, NotNullWhen(true)] Func<TInstance, TResult>? func)
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