namespace ScrubJay.Extensions;

/// <summary>
/// Extensions on <see cref="Delegate"/> and <c>delegate</c>
/// </summary>
public static class DelegateExtensions
{
    public static Func<None> ToFunc(this Action action) => () =>
    {
        action();
        return default;
    };

    public static Func<T1, None> ToFunc<T1>(this Action<T1> action) => arg1 =>
    {
        action(arg1);
        return default;
    };

    public static Func<T1, T2, None> ToFunc<T1, T2>(this Action<T1, T2> action) => (arg1, arg2) =>
    {
        action(arg1, arg2);
        return default;
    };

    public static Func<T1, T2, T3, None> ToFunc<T1, T2, T3>(this Action<T1, T2, T3> action) => (arg1, arg2, arg3) =>
    {
        action(arg1, arg2, arg3);
        return default;
    };

    public static Func<T1, T2, T3, T4, None> ToFunc<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action) => (arg1, arg2, arg3, arg4) =>
    {
        action(arg1, arg2, arg3, arg4);
        return default;
    };

    public static Func<T1, T2, T3, T4, T5, None> ToFunc<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action) => (arg1, arg2, arg3, arg4, arg5) =>
    {
        action(arg1, arg2, arg3, arg4, arg5);
        return default;
    };

    public static Func<T1, T2, T3, T4, T5, T6, None> ToFunc<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action) => (arg1, arg2, arg3, arg4, arg5, arg6) =>
    {
        action(arg1, arg2, arg3, arg4, arg5, arg6);
        return default;
    };

    public static Func<T1, T2, T3, T4, T5, T6, T7, None> ToFunc<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action) => (arg1, arg2, arg3, arg4, arg5, arg6, arg7) =>
    {
        action(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        return default;
    };

    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, None> ToFunc<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action) =>
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