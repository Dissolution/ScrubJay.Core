namespace ScrubJay.Extensions;

/// <summary>
/// Extensions methods on <see cref="Delegate"/>, <see cref="Action">Action</see>, and <see cref="Func{T}">Func</see>
/// </summary>
public static class DelegateExtensions
{
    /// <summary>
    /// Tries to invoke the <paramref name="action"/> and returns a <see cref="Result"/>
    /// </summary>
    /// <param name="action">The <see cref="Action"/> to <see cref="Action.Invoke"/></param>
    /// <returns>
    /// A successful <see cref="Result"/> if the <paramref name="action"/> invokes without throwing an <see cref="Exception"/>.
    /// A failed <see cref="Result"/> with the caught <see cref="Exception"/> attached if not.
    /// </returns>
    public static Result TryInvoke(
        [AllowNull, NotNullWhen(true)] this Action? action)
    {
        if (action is null)
        {
            return new ArgumentNullException(nameof(action));
        }

        try
        {
            action.Invoke();
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Error(ex);
        }
    }

    /// <summary>
    /// Tries to invoke the <paramref name="func"/>, setting <paramref name="value"/> and returning a <see cref="Result"/>
    /// </summary>
    /// <typeparam name="TValue">The <see cref="Type"/> of <paramref name="value"/> the <paramref name="func"/> produces</typeparam>
    /// <param name="func">The <see cref="Func{T}"/> to <see cref="Func{T}.Invoke"/></param>
    /// <param name="value">The result of invoking <paramref name="func"/> or <see langword="default{TResult}"/> on failure.</param>
    /// <returns>
    /// A successful <see cref="Result"/> if the <paramref name="func"/> invokes without throwing an <see cref="Exception"/>.
    /// A failed <see cref="Result"/> with the caught <see cref="Exception"/> attached if not.
    /// </returns>
    public static Result TryInvoke<TValue>(
        [AllowNull, NotNullWhen(true)] this Func<TValue>? func,
        [MaybeNullWhen(false)] out TValue value)
    {
        if (func is null)
        {
            value = default;
            return new ArgumentNullException(nameof(func));
        }

        try
        {
            value = func.Invoke();
            return Result.Ok();
        }
        catch (Exception ex)
        {
            value = default;
            return Result.Error(ex);
        }
    }

    /// <summary>
    /// Try to execute the given <paramref name="func"/> and return a <see cref="Result{T}"/>.
    /// </summary>
    /// <param name="func">The function to try to execute.</param>
    /// <returns>
    /// A passing <see cref="Result{T}"/> containing <paramref name="func"/>'s return value or
    /// a failing <see cref="Result{T}"/> containing the captured <see cref="Exception"/>.
    /// </returns>
    public static Result<TValue> TryInvoke<TValue>(
        [AllowNull, NotNullWhen(true)] this Func<TValue>? func)
    {
        if (func is null)
        {
            return new ArgumentNullException(nameof(func));
        }

        try
        {
            TValue value = func.Invoke();
            return Result<TValue>.Ok(value);
        }
        catch (Exception ex)
        {
            return Result<TValue>.Error(ex);
        }
    }

    /// <summary>
    /// Invokes a <paramref name="func"/> and returns its result <br/>
    /// If the function throws an <see cref="Exception"/>, a <paramref name="fallback"/> value will be returned instead
    /// </summary>
    /// <param name="func">The <see cref="Func{TResult}"/> to invoke</param>
    /// <param name="fallback">The value to return if <paramref name="func"/> throws an <see cref="Exception"/></param>
    /// <typeparam name="T">The <see cref="Type"/> of value to be returned</typeparam>
    /// <returns>The result of <paramref name="func"/> or <paramref name="fallback"/></returns>
    [return: NotNullIfNotNull(nameof(fallback))]
    public static T? InvokeOrDefault<T>(
        this Func<T>? func,
        T? fallback = default)
    {
        if (func is null)
        {
            return fallback;
        }

        try
        {
            return func();
        }
        catch
        {
            return fallback;
        }
    }

    public static Func<Nothing> ToFunc(this Action action) => () =>
    {
        action();
        return default;
    };

    public static Func<T1, Nothing> ToFunc<T1>(this Action<T1> action) => arg1 =>
    {
        action(arg1);
        return default;
    };

    public static Func<T1, T2, Nothing> ToFunc<T1, T2>(this Action<T1, T2> action) => (arg1, arg2) =>
    {
        action(arg1, arg2);
        return default;
    };

    public static Func<T1, T2, T3, Nothing> ToFunc<T1, T2, T3>(this Action<T1, T2, T3> action) => (arg1, arg2, arg3) =>
    {
        action(arg1, arg2, arg3);
        return default;
    };

    public static Func<T1, T2, T3, T4, Nothing> ToFunc<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action) => (arg1, arg2, arg3, arg4) =>
    {
        action(arg1, arg2, arg3, arg4);
        return default;
    };

    public static Func<T1, T2, T3, T4, T5, Nothing> ToFunc<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action) => (arg1, arg2, arg3, arg4, arg5) =>
    {
        action(arg1, arg2, arg3, arg4, arg5);
        return default;
    };

    public static Func<T1, T2, T3, T4, T5, T6, Nothing> ToFunc<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action) => (arg1, arg2, arg3, arg4, arg5, arg6) =>
    {
        action(arg1, arg2, arg3, arg4, arg5, arg6);
        return default;
    };

    public static Func<T1, T2, T3, T4, T5, T6, T7, Nothing> ToFunc<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action) => (arg1, arg2, arg3, arg4, arg5, arg6, arg7) =>
    {
        action(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        return default;
    };

    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, Nothing> ToFunc<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action) =>
        (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8) =>
        {
            action(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            return default;
        };

    public static Action ToAction<TResult>(this Func<TResult> func) => () => func();
   
    public static Action<T1> ToAction<T1, TResult>(this Func<T1, TResult> func) => arg1 => func(arg1);
   
    public static Action<T1, T2> ToAction<T1, T2, TResult>(this Func<T1, T2, TResult> func) => (arg1, arg2) => func(arg1, arg2);

    public static Action<T1, T2, T3> ToAction<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> func) => (arg1, arg2, arg3) => func(arg1, arg2, arg3);

    public static Action<T1, T2, T3, T4> ToAction<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, TResult> func) => (arg1, arg2, arg3, arg4) => func(arg1, arg2, arg3, arg4);

    public static Action<T1, T2, T3, T4, T5> ToAction<T1, T2, T3, T4, T5, TResult>(this Func<T1, T2, T3, T4, T5, TResult> func) => (arg1, arg2, arg3, arg4, arg5) => func(arg1, arg2, arg3, arg4, arg5);

    public static Action<T1, T2, T3, T4, T5, T6> ToAction<T1, T2, T3, T4, T5, T6, TResult>(this Func<T1, T2, T3, T4, T5, T6, TResult> func) =>
        (arg1, arg2, arg3, arg4, arg5, arg6) => func(arg1, arg2, arg3, arg4, arg5, arg6);

    public static Action<T1, T2, T3, T4, T5, T6, T7> ToAction<T1, T2, T3, T4, T5, T6, T7, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, TResult> func) =>
        (arg1, arg2, arg3, arg4, arg5, arg6, arg7) => func(arg1, arg2, arg3, arg4, arg5, arg6, arg7);

    public static Action<T1, T2, T3, T4, T5, T6, T7, T8> ToAction<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func) =>
        (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8) => func(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
}