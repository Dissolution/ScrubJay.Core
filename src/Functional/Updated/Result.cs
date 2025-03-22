// ReSharper disable InconsistentNaming
#pragma warning disable CA1031

namespace ScrubJay.Functional;

[PublicAPI]
public static class Result
{
    /// <summary>
    /// Try to invoke an <see cref="Action"/>
    /// </summary>
    /// <param name="action"></param>
    /// <returns>
    /// The <see cref="Result{T}"/> of the invocation
    /// </returns>
    public static Result<Unit> TryInvoke(Action? action)
    {
        if (action is null)
        {
            return new ArgumentNullException(nameof(action));
        }

        try
        {
            action.Invoke();
            return Ok(Unit());
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public static Result<Unit> TryInvoke<I>(
        [NotNullWhen(true)] I? instance,
        [NotNullWhen(true)] Action<I>? instanceAction)
    {
        if (instance is null)
            return new ArgumentNullException(nameof(instance));
        if (instanceAction is null)
            return new ArgumentNullException(nameof(instanceAction));

        try
        {
            instanceAction.Invoke(instance);
            return Ok(Unit());
        }
        catch (Exception ex)
        {
            return ex;
        }
    }


    public static Result<T> TryInvoke<T>(Fn<T>? func)
    {
        if (func is null)
            return new ArgumentNullException(nameof(func));

        try
        {
            return Ok(func.Invoke());
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public static Result<T> TryInvoke<I, T>(
        [NotNullWhen(true)] I? instance,
        [NotNullWhen(true)] Fn<I, T>? instanceFunc)
    {
        if (instance is null)
            return new ArgumentNullException(nameof(instance));
        if (instanceFunc is null)
            return new ArgumentNullException(nameof(instanceFunc));

        try
        {
            return Ok(instanceFunc.Invoke(instance));
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
}
