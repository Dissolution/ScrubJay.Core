#pragma warning disable S2436

namespace ScrubJay.Extensions;

/// <summary>
/// Extensions on <see cref="Delegate"/> and <c>delegate</c>
/// </summary>
[PublicAPI]
public static class DelegateExtensions
{
    public static Func<Unit> ToUnitFunc(this Action action)
        => () =>
        {
            action();
            return Unit.Default;
        };

    public static Func<T1, Unit> ToUnitFunc<T1>(this Action<T1> action)
        => arg1 =>
        {
            action(arg1);
            return Unit.Default;
        };

    public static Func<T1, T2, Unit> ToUnitFunc<T1, T2>(this Action<T1, T2> action)
        => (arg1, arg2) =>
        {
            action(arg1, arg2);
            return Unit.Default;
        };

    public static Func<T1, T2, T3, Unit> ToUnitFunc<T1, T2, T3>(this Action<T1, T2, T3> action)
        => (arg1, arg2, arg3) =>
        {
            action(arg1, arg2, arg3);
            return Unit.Default;
        };

    public static Func<T1, T2, T3, T4, Unit> ToUnitFunc<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action)
        => (arg1, arg2, arg3, arg4) =>
        {
            action(arg1, arg2, arg3, arg4);
            return Unit.Default;
        };

    public static Func<T1, T2, T3, T4, T5, Unit> ToUnitFunc<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action)
        => (arg1, arg2, arg3, arg4, arg5) =>
        {
            action(arg1, arg2, arg3, arg4, arg5);
            return Unit.Default;
        };

    public static Func<T1, T2, T3, T4, T5, T6, Unit> ToUnitFunc<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action)
        => (arg1, arg2, arg3, arg4, arg5, arg6) =>
        {
            action(arg1, arg2, arg3, arg4, arg5, arg6);
            return Unit.Default;
        };

    public static Func<T1, T2, T3, T4, T5, T6, T7, Unit> ToUnitFunc<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action)
        => (arg1, arg2, arg3, arg4, arg5, arg6, arg7) =>
        {
            action(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            return Unit.Default;
        };

    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, Unit> ToUnitFunc<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action)
        => (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8) =>
        {
            action(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            return Unit.Default;
        };
}