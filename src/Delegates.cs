// File name must match type name

#pragma warning disable MA0048, CA1005

// ReSharper disable InconsistentNaming
namespace ScrubJay;

#if NET9_0_OR_GREATER

#region Act

[PublicAPI]
public delegate void Act();

[PublicAPI]
public delegate void Act<in T1>(T1 arg1)
    where T1 : allows ref struct;

[PublicAPI]
public delegate void Act<in T1, in T2>(T1 arg1, T2 arg2)
    where T1 : allows ref struct
    where T2 : allows ref struct;

[PublicAPI]
public delegate void Act<in T1, in T2, in T3>(T1 arg1, T2 arg2, T3 arg3)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct;

[PublicAPI]
public delegate void Act<in T1, in T2, in T3, in T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct;

[PublicAPI]
public delegate void Act<in T1, in T2, in T3, in T4, in T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct;

[PublicAPI]
public delegate void Act<in T1, in T2, in T3, in T4, in T5, in T6>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct
    where T6 : allows ref struct;

[PublicAPI]
public delegate void Act<in T1, in T2, in T3, in T4, in T5, in T6, in T7>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6,
    T7 arg7)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct
    where T6 : allows ref struct
    where T7 : allows ref struct;

[PublicAPI]
public delegate void Act<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5,
    T6 arg6, T7 arg7, T8 arg8)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct
    where T6 : allows ref struct
    where T7 : allows ref struct
    where T8 : allows ref struct;

#endregion

#region ActRef

[PublicAPI]
public delegate void ActRef<TR>(ref TR refArg)
    where TR : allows ref struct;

[PublicAPI]
public delegate void ActRef<TR, in T1>(ref TR refArg, T1 arg1)
    where TR : allows ref struct
    where T1 : allows ref struct;

[PublicAPI]
public delegate void ActRef<TR, in T1, in T2>(ref TR refArg, T1 arg1, T2 arg2)
    where TR : allows ref struct
    where T1 : allows ref struct
    where T2 : allows ref struct;

[PublicAPI]
public delegate void ActRef<TR, in T1, in T2, in T3>(ref TR refArg, T1 arg1, T2 arg2, T3 arg3)
    where TR : allows ref struct
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct;

[PublicAPI]
public delegate void ActRef<TR, in T1, in T2, in T3, in T4>(ref TR refArg, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    where TR : allows ref struct
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct;

[PublicAPI]
public delegate void ActRef<TR, in T1, in T2, in T3, in T4, in T5>(ref TR refArg, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
    where TR : allows ref struct
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct;

[PublicAPI]
public delegate void ActRef<TR, in T1, in T2, in T3, in T4, in T5, in T6>(ref TR refArg, T1 arg1, T2 arg2, T3 arg3, T4 arg4,
    T5 arg5, T6 arg6)
    where TR : allows ref struct
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct
    where T6 : allows ref struct;

[PublicAPI]
public delegate void ActRef<TR, in T1, in T2, in T3, in T4, in T5, in T6, in T7>(ref TR refArg, T1 arg1, T2 arg2, T3 arg3,
    T4 arg4, T5 arg5, T6 arg6, T7 arg7)
    where TR : allows ref struct
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct
    where T6 : allows ref struct
    where T7 : allows ref struct;

[PublicAPI]
public delegate void ActRef<TR, in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8>(ref TR refArg, T1 arg1, T2 arg2,
    T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
    where TR : allows ref struct
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct
    where T6 : allows ref struct
    where T7 : allows ref struct
    where T8 : allows ref struct;

#endregion

#region ActRSpan

[PublicAPI]
public delegate void ActRSpan<T>(ReadOnlySpan<T> span);

[PublicAPI]
public delegate void ActRSpan<T, in T1>(ReadOnlySpan<T> span, T1 arg1)
    where T1 : allows ref struct;

[PublicAPI]
public delegate void ActRSpan<T, in T1, in T2>(ReadOnlySpan<T> span, T1 arg1, T2 arg2)
    where T1 : allows ref struct
    where T2 : allows ref struct;

[PublicAPI]
public delegate void ActRSpan<T, in T1, in T2, in T3>(ReadOnlySpan<T> span, T1 arg1, T2 arg2, T3 arg3)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct;

[PublicAPI]
public delegate void ActRSpan<T, in T1, in T2, in T3, in T4>(ReadOnlySpan<T> span, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct;

[PublicAPI]
public delegate void ActRSpan<T, in T1, in T2, in T3, in T4, in T5>(ReadOnlySpan<T> span, T1 arg1, T2 arg2, T3 arg3, T4 arg4,
    T5 arg5)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct;

[PublicAPI]
public delegate void ActRSpan<T, in T1, in T2, in T3, in T4, in T5, in T6>(ReadOnlySpan<T> span, T1 arg1, T2 arg2, T3 arg3,
    T4 arg4, T5 arg5, T6 arg6)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct
    where T6 : allows ref struct;

[PublicAPI]
public delegate void ActRSpan<T, in T1, in T2, in T3, in T4, in T5, in T6, in T7>(ReadOnlySpan<T> span, T1 arg1, T2 arg2,
    T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct
    where T6 : allows ref struct
    where T7 : allows ref struct;

[PublicAPI]
public delegate void ActRSpan<T, in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8>(ReadOnlySpan<T> span, T1 arg1, T2 arg2,
    T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct
    where T6 : allows ref struct
    where T7 : allows ref struct
    where T8 : allows ref struct;

#endregion

#region ActSpan

[PublicAPI]
public delegate void ActSpan<T>(Span<T> span);

[PublicAPI]
public delegate void ActSpan<T, in T1>(Span<T> span, T1 arg1)
    where T1 : allows ref struct;

[PublicAPI]
public delegate void ActSpan<T, in T1, in T2>(Span<T> span, T1 arg1, T2 arg2)
    where T1 : allows ref struct
    where T2 : allows ref struct;

[PublicAPI]
public delegate void ActSpan<T, in T1, in T2, in T3>(Span<T> span, T1 arg1, T2 arg2, T3 arg3)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct;

[PublicAPI]
public delegate void ActSpan<T, in T1, in T2, in T3, in T4>(Span<T> span, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct;

[PublicAPI]
public delegate void ActSpan<T, in T1, in T2, in T3, in T4, in T5>(Span<T> span, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct;

[PublicAPI]
public delegate void ActSpan<T, in T1, in T2, in T3, in T4, in T5, in T6>(Span<T> span, T1 arg1, T2 arg2, T3 arg3, T4 arg4,
    T5 arg5, T6 arg6)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct
    where T6 : allows ref struct;

[PublicAPI]
public delegate void ActSpan<T, in T1, in T2, in T3, in T4, in T5, in T6, in T7>(Span<T> span, T1 arg1, T2 arg2, T3 arg3,
    T4 arg4, T5 arg5, T6 arg6, T7 arg7)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct
    where T6 : allows ref struct
    where T7 : allows ref struct;

[PublicAPI]
public delegate void ActSpan<T, in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8>(Span<T> span, T1 arg1, T2 arg2, T3 arg3,
    T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct
    where T6 : allows ref struct
    where T7 : allows ref struct
    where T8 : allows ref struct;

#endregion

#region Fn

[PublicAPI]
public delegate R Fn<out R>()
    where R : allows ref struct;

[PublicAPI]
public delegate R Fn<in T1, out R>(T1 arg1)
    where T1 : allows ref struct
    where R : allows ref struct;

[PublicAPI]
public delegate R Fn<in T1, in T2, out R>(T1 arg1, T2 arg2)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where R : allows ref struct;

[PublicAPI]
public delegate R Fn<in T1, in T2, in T3, out R>(T1 arg1, T2 arg2, T3 arg3)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where R : allows ref struct;

[PublicAPI]
public delegate R Fn<in T1, in T2, in T3, in T4, out R>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where R : allows ref struct;

[PublicAPI]
public delegate R Fn<in T1, in T2, in T3, in T4, in T5, out R>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct
    where R : allows ref struct;

[PublicAPI]
public delegate R Fn<in T1, in T2, in T3, in T4, in T5, in T6, out R>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct
    where T6 : allows ref struct
    where R : allows ref struct;

[PublicAPI]
public delegate R Fn<in T1, in T2, in T3, in T4, in T5, in T6, in T7, out R>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5,
    T6 arg6, T7 arg7)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct
    where T6 : allows ref struct
    where T7 : allows ref struct
    where R : allows ref struct;

[PublicAPI]
public delegate R Fn<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, out R>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5,
    T6 arg6, T7 arg7, T8 arg8)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct
    where T6 : allows ref struct
    where T7 : allows ref struct
    where T8 : allows ref struct
    where R : allows ref struct;

#endregion

#region FnRef

[PublicAPI]
public delegate R FnRef<TR, out R>(ref TR refArg)
    where TR : allows ref struct
    where R : allows ref struct;

[PublicAPI]
public delegate R FnRef<TR, in T1, out R>(ref TR refArg, T1 arg1)
    where TR : allows ref struct
    where T1 : allows ref struct
    where R : allows ref struct;

[PublicAPI]
public delegate R FnRef<TR, in T1, in T2, out R>(ref TR refArg, T1 arg1, T2 arg2)
    where TR : allows ref struct
    where T1 : allows ref struct
    where T2 : allows ref struct
    where R : allows ref struct;

[PublicAPI]
public delegate R FnRef<TR, in T1, in T2, in T3, out R>(ref TR refArg, T1 arg1, T2 arg2, T3 arg3)
    where TR : allows ref struct
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where R : allows ref struct;

[PublicAPI]
public delegate R FnRef<TR, in T1, in T2, in T3, in T4, out R>(ref TR refArg, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    where TR : allows ref struct
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where R : allows ref struct;

[PublicAPI]
public delegate R FnRef<TR, in T1, in T2, in T3, in T4, in T5, out R>(ref TR refArg, T1 arg1, T2 arg2, T3 arg3, T4 arg4,
    T5 arg5)
    where TR : allows ref struct
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct
    where R : allows ref struct;

[PublicAPI]
public delegate R FnRef<TR, in T1, in T2, in T3, in T4, in T5, in T6, out R>(ref TR refArg, T1 arg1, T2 arg2, T3 arg3, T4 arg4,
    T5 arg5, T6 arg6)
    where TR : allows ref struct
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct
    where T6 : allows ref struct
    where R : allows ref struct;

[PublicAPI]
public delegate R FnRef<TR, in T1, in T2, in T3, in T4, in T5, in T6, in T7, out R>(ref TR refArg, T1 arg1, T2 arg2, T3 arg3,
    T4 arg4, T5 arg5, T6 arg6, T7 arg7)
    where TR : allows ref struct
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct
    where T6 : allows ref struct
    where T7 : allows ref struct
    where R : allows ref struct;

[PublicAPI]
public delegate R FnRef<TR, in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, out R>(ref TR refArg, T1 arg1, T2 arg2,
    T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
    where TR : allows ref struct
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct
    where T6 : allows ref struct
    where T7 : allows ref struct
    where T8 : allows ref struct
    where R : allows ref struct;

#endregion

#region FnRSpan

[PublicAPI]
public delegate R FnRSpan<T, out R>(ReadOnlySpan<T> span)
    where R : allows ref struct;

[PublicAPI]
public delegate R FnRSpan<T, in T1, out R>(ReadOnlySpan<T> span, T1 arg1)
    where T1 : allows ref struct
    where R : allows ref struct;

[PublicAPI]
public delegate R FnRSpan<T, in T1, in T2, out R>(ReadOnlySpan<T> span, T1 arg1, T2 arg2)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where R : allows ref struct;

[PublicAPI]
public delegate R FnRSpan<T, in T1, in T2, in T3, out R>(ReadOnlySpan<T> span, T1 arg1, T2 arg2, T3 arg3)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where R : allows ref struct;

[PublicAPI]
public delegate R FnRSpan<T, in T1, in T2, in T3, in T4, out R>(ReadOnlySpan<T> span, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where R : allows ref struct;

[PublicAPI]
public delegate R FnRSpan<T, in T1, in T2, in T3, in T4, in T5, out R>(ReadOnlySpan<T> span, T1 arg1, T2 arg2, T3 arg3, T4 arg4,
    T5 arg5)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct
    where R : allows ref struct;

[PublicAPI]
public delegate R FnRSpan<T, in T1, in T2, in T3, in T4, in T5, in T6, out R>(ReadOnlySpan<T> span, T1 arg1, T2 arg2, T3 arg3,
    T4 arg4, T5 arg5, T6 arg6)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct
    where T6 : allows ref struct
    where R : allows ref struct;

[PublicAPI]
public delegate R FnRSpan<T, in T1, in T2, in T3, in T4, in T5, in T6, in T7, out R>(ReadOnlySpan<T> span, T1 arg1, T2 arg2,
    T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct
    where T6 : allows ref struct
    where T7 : allows ref struct
    where R : allows ref struct;

[PublicAPI]
public delegate R FnRSpan<T, in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, out R>(ReadOnlySpan<T> span, T1 arg1,
    T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct
    where T6 : allows ref struct
    where T7 : allows ref struct
    where T8 : allows ref struct
    where R : allows ref struct;

#endregion

#region RSpanFnRSpan

[PublicAPI]
public delegate ReadOnlySpan<R> RSpanFnRSpan<T, R>(ReadOnlySpan<T> span);

[PublicAPI]
public delegate ReadOnlySpan<R> RSpanFnRSpan<T, in T1, R>(ReadOnlySpan<T> span, T1 arg1)
    where T1 : allows ref struct;

[PublicAPI]
public delegate ReadOnlySpan<R> RSpanFnRSpan<T, in T1, in T2, R>(ReadOnlySpan<T> span, T1 arg1, T2 arg2)
    where T1 : allows ref struct
    where T2 : allows ref struct;

[PublicAPI]
public delegate ReadOnlySpan<R> RSpanFnRSpan<T, in T1, in T2, in T3, R>(ReadOnlySpan<T> span, T1 arg1, T2 arg2, T3 arg3)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct;

[PublicAPI]
public delegate ReadOnlySpan<R> RSpanFnRSpan<T, in T1, in T2, in T3, in T4, R>(ReadOnlySpan<T> span, T1 arg1, T2 arg2, T3 arg3,
    T4 arg4)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct;

[PublicAPI]
public delegate ReadOnlySpan<R> RSpanFnRSpan<T, in T1, in T2, in T3, in T4, in T5, R>(ReadOnlySpan<T> span, T1 arg1, T2 arg2,
    T3 arg3, T4 arg4, T5 arg5)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct;

[PublicAPI]
public delegate ReadOnlySpan<R> RSpanFnRSpan<T, in T1, in T2, in T3, in T4, in T5, in T6, R>(ReadOnlySpan<T> span, T1 arg1,
    T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct
    where T6 : allows ref struct;

[PublicAPI]
public delegate ReadOnlySpan<R> RSpanFnRSpan<T, in T1, in T2, in T3, in T4, in T5, in T6, in T7, R>(ReadOnlySpan<T> span,
    T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct
    where T6 : allows ref struct
    where T7 : allows ref struct;

[PublicAPI]
public delegate ReadOnlySpan<R> RSpanFnRSpan<T, in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, R>(ReadOnlySpan<T> span,
    T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct
    where T6 : allows ref struct
    where T7 : allows ref struct
    where T8 : allows ref struct;

#endregion

#region SpanFnRSpan

[PublicAPI]
public delegate Span<R> SpanFnRSpan<T, R>(ReadOnlySpan<T> span);

[PublicAPI]
public delegate Span<R> SpanFnRSpan<T, in T1, R>(ReadOnlySpan<T> span, T1 arg1)
    where T1 : allows ref struct;

[PublicAPI]
public delegate Span<R> SpanFnRSpan<T, in T1, in T2, R>(ReadOnlySpan<T> span, T1 arg1, T2 arg2)
    where T1 : allows ref struct
    where T2 : allows ref struct;

[PublicAPI]
public delegate Span<R> SpanFnRSpan<T, in T1, in T2, in T3, R>(ReadOnlySpan<T> span, T1 arg1, T2 arg2, T3 arg3)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct;

[PublicAPI]
public delegate Span<R> SpanFnRSpan<T, in T1, in T2, in T3, in T4, R>(ReadOnlySpan<T> span, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct;

[PublicAPI]
public delegate Span<R> SpanFnRSpan<T, in T1, in T2, in T3, in T4, in T5, R>(ReadOnlySpan<T> span, T1 arg1, T2 arg2, T3 arg3,
    T4 arg4, T5 arg5)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct;

[PublicAPI]
public delegate Span<R> SpanFnRSpan<T, in T1, in T2, in T3, in T4, in T5, in T6, R>(ReadOnlySpan<T> span, T1 arg1, T2 arg2,
    T3 arg3, T4 arg4, T5 arg5, T6 arg6)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct
    where T6 : allows ref struct;

[PublicAPI]
public delegate Span<R> SpanFnRSpan<T, in T1, in T2, in T3, in T4, in T5, in T6, in T7, R>(ReadOnlySpan<T> span, T1 arg1,
    T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct
    where T6 : allows ref struct
    where T7 : allows ref struct;

[PublicAPI]
public delegate Span<R> SpanFnRSpan<T, in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, R>(ReadOnlySpan<T> span, T1 arg1,
    T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct
    where T6 : allows ref struct
    where T7 : allows ref struct
    where T8 : allows ref struct;

#endregion

#region FnSpan

[PublicAPI]
public delegate R FnSpan<T, out R>(Span<T> span)
    where R : allows ref struct;

[PublicAPI]
public delegate R FnSpan<T, in T1, out R>(Span<T> span, T1 arg1)
    where T1 : allows ref struct
    where R : allows ref struct;

[PublicAPI]
public delegate R FnSpan<T, in T1, in T2, out R>(Span<T> span, T1 arg1, T2 arg2)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where R : allows ref struct;

[PublicAPI]
public delegate R FnSpan<T, in T1, in T2, in T3, out R>(Span<T> span, T1 arg1, T2 arg2, T3 arg3)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where R : allows ref struct;

[PublicAPI]
public delegate R FnSpan<T, in T1, in T2, in T3, in T4, out R>(Span<T> span, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where R : allows ref struct;

[PublicAPI]
public delegate R
    FnSpan<T, in T1, in T2, in T3, in T4, in T5, out R>(Span<T> span, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct
    where R : allows ref struct;

[PublicAPI]
public delegate R FnSpan<T, in T1, in T2, in T3, in T4, in T5, in T6, out R>(Span<T> span, T1 arg1, T2 arg2, T3 arg3, T4 arg4,
    T5 arg5, T6 arg6)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct
    where T6 : allows ref struct
    where R : allows ref struct;

[PublicAPI]
public delegate R FnSpan<T, in T1, in T2, in T3, in T4, in T5, in T6, in T7, out R>(Span<T> span, T1 arg1, T2 arg2, T3 arg3,
    T4 arg4, T5 arg5, T6 arg6, T7 arg7)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct
    where T6 : allows ref struct
    where T7 : allows ref struct
    where R : allows ref struct;

[PublicAPI]
public delegate R FnSpan<T, in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, out R>(Span<T> span, T1 arg1, T2 arg2,
    T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct
    where T6 : allows ref struct
    where T7 : allows ref struct
    where T8 : allows ref struct
    where R : allows ref struct;

#endregion

#region RSpanFnSpan

[PublicAPI]
public delegate ReadOnlySpan<R> RSpanFnSpan<T, R>(Span<T> span);

[PublicAPI]
public delegate ReadOnlySpan<R> RSpanFnSpan<T, in T1, R>(Span<T> span, T1 arg1)
    where T1 : allows ref struct;

[PublicAPI]
public delegate ReadOnlySpan<R> RSpanFnSpan<T, in T1, in T2, R>(Span<T> span, T1 arg1, T2 arg2)
    where T1 : allows ref struct
    where T2 : allows ref struct;

[PublicAPI]
public delegate ReadOnlySpan<R> RSpanFnSpan<T, in T1, in T2, in T3, R>(Span<T> span, T1 arg1, T2 arg2, T3 arg3)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct;

[PublicAPI]
public delegate ReadOnlySpan<R> RSpanFnSpan<T, in T1, in T2, in T3, in T4, R>(Span<T> span, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct;

[PublicAPI]
public delegate ReadOnlySpan<R> RSpanFnSpan<T, in T1, in T2, in T3, in T4, in T5, R>(Span<T> span, T1 arg1, T2 arg2, T3 arg3,
    T4 arg4, T5 arg5)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct;

[PublicAPI]
public delegate ReadOnlySpan<R> RSpanFnSpan<T, in T1, in T2, in T3, in T4, in T5, in T6, R>(Span<T> span, T1 arg1, T2 arg2,
    T3 arg3, T4 arg4, T5 arg5, T6 arg6)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct
    where T6 : allows ref struct;

[PublicAPI]
public delegate ReadOnlySpan<R> RSpanFnSpan<T, in T1, in T2, in T3, in T4, in T5, in T6, in T7, R>(Span<T> span, T1 arg1,
    T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct
    where T6 : allows ref struct
    where T7 : allows ref struct;

[PublicAPI]
public delegate ReadOnlySpan<R> RSpanFnSpan<T, in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, R>(Span<T> span, T1 arg1,
    T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct
    where T6 : allows ref struct
    where T7 : allows ref struct
    where T8 : allows ref struct;

#endregion

#region SpanFnSpan

[PublicAPI]
public delegate Span<R> SpanFnSpan<T, R>(Span<T> span);

[PublicAPI]
public delegate Span<R> SpanFnSpan<T, in T1, R>(Span<T> span, T1 arg1)
    where T1 : allows ref struct;

[PublicAPI]
public delegate Span<R> SpanFnSpan<T, in T1, in T2, R>(Span<T> span, T1 arg1, T2 arg2)
    where T1 : allows ref struct
    where T2 : allows ref struct;

[PublicAPI]
public delegate Span<R> SpanFnSpan<T, in T1, in T2, in T3, R>(Span<T> span, T1 arg1, T2 arg2, T3 arg3)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct;

[PublicAPI]
public delegate Span<R> SpanFnSpan<T, in T1, in T2, in T3, in T4, R>(Span<T> span, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct;

[PublicAPI]
public delegate Span<R> SpanFnSpan<T, in T1, in T2, in T3, in T4, in T5, R>(Span<T> span, T1 arg1, T2 arg2, T3 arg3, T4 arg4,
    T5 arg5)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct;

[PublicAPI]
public delegate Span<R> SpanFnSpan<T, in T1, in T2, in T3, in T4, in T5, in T6, R>(Span<T> span, T1 arg1, T2 arg2, T3 arg3,
    T4 arg4, T5 arg5, T6 arg6)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct
    where T6 : allows ref struct;

[PublicAPI]
public delegate Span<R> SpanFnSpan<T, in T1, in T2, in T3, in T4, in T5, in T6, in T7, R>(Span<T> span, T1 arg1, T2 arg2,
    T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct
    where T6 : allows ref struct
    where T7 : allows ref struct;

[PublicAPI]
public delegate Span<R> SpanFnSpan<T, in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, R>(Span<T> span, T1 arg1, T2 arg2,
    T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct
    where T6 : allows ref struct
    where T7 : allows ref struct
    where T8 : allows ref struct;

#endregion
#else

#region Act

[PublicAPI]
public delegate void Act();

[PublicAPI]
public delegate void Act<in T1>(T1 arg1);

[PublicAPI]
public delegate void Act<in T1, in T2>(T1 arg1, T2 arg2);

[PublicAPI]
public delegate void Act<in T1, in T2, in T3>(T1 arg1, T2 arg2, T3 arg3);

[PublicAPI]
public delegate void Act<in T1, in T2, in T3, in T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);

[PublicAPI]
public delegate void Act<in T1, in T2, in T3, in T4, in T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

[PublicAPI]
public delegate void Act<in T1, in T2, in T3, in T4, in T5, in T6>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);

[PublicAPI]
public delegate void Act<in T1, in T2, in T3, in T4, in T5, in T6, in T7>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6,
    T7 arg7);

[PublicAPI]
public delegate void Act<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5,
    T6 arg6, T7 arg7, T8 arg8);

#endregion

#region ActRef

[PublicAPI]
public delegate void ActRef<TR>(ref TR refArg);

[PublicAPI]
public delegate void ActRef<TR, in T1>(ref TR refArg, T1 arg1);

[PublicAPI]
public delegate void ActRef<TR, in T1, in T2>(ref TR refArg, T1 arg1, T2 arg2);

[PublicAPI]
public delegate void ActRef<TR, in T1, in T2, in T3>(ref TR refArg, T1 arg1, T2 arg2, T3 arg3);

[PublicAPI]
public delegate void ActRef<TR, in T1, in T2, in T3, in T4>(ref TR refArg, T1 arg1, T2 arg2, T3 arg3, T4 arg4);

[PublicAPI]
public delegate void ActRef<TR, in T1, in T2, in T3, in T4, in T5>(ref TR refArg, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

[PublicAPI]
public delegate void ActRef<TR, in T1, in T2, in T3, in T4, in T5, in T6>(ref TR refArg, T1 arg1, T2 arg2, T3 arg3, T4 arg4,
    T5 arg5, T6 arg6);

[PublicAPI]
public delegate void ActRef<TR, in T1, in T2, in T3, in T4, in T5, in T6, in T7>(ref TR refArg, T1 arg1, T2 arg2, T3 arg3,
    T4 arg4, T5 arg5, T6 arg6, T7 arg7);

[PublicAPI]
public delegate void ActRef<TR, in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8>(ref TR refArg, T1 arg1, T2 arg2,
    T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);

#endregion

#region ActRSpan

[PublicAPI]
public delegate void ActRSpan<T>(ReadOnlySpan<T> span);

[PublicAPI]
public delegate void ActRSpan<T, in T1>(ReadOnlySpan<T> span, T1 arg1);

[PublicAPI]
public delegate void ActRSpan<T, in T1, in T2>(ReadOnlySpan<T> span, T1 arg1, T2 arg2);

[PublicAPI]
public delegate void ActRSpan<T, in T1, in T2, in T3>(ReadOnlySpan<T> span, T1 arg1, T2 arg2, T3 arg3);

[PublicAPI]
public delegate void ActRSpan<T, in T1, in T2, in T3, in T4>(ReadOnlySpan<T> span, T1 arg1, T2 arg2, T3 arg3, T4 arg4);

[PublicAPI]
public delegate void ActRSpan<T, in T1, in T2, in T3, in T4, in T5>(ReadOnlySpan<T> span, T1 arg1, T2 arg2, T3 arg3, T4 arg4,
    T5 arg5);

[PublicAPI]
public delegate void ActRSpan<T, in T1, in T2, in T3, in T4, in T5, in T6>(ReadOnlySpan<T> span, T1 arg1, T2 arg2, T3 arg3,
    T4 arg4, T5 arg5, T6 arg6);

[PublicAPI]
public delegate void ActRSpan<T, in T1, in T2, in T3, in T4, in T5, in T6, in T7>(ReadOnlySpan<T> span, T1 arg1, T2 arg2,
    T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);

[PublicAPI]
public delegate void ActRSpan<T, in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8>(ReadOnlySpan<T> span, T1 arg1, T2 arg2,
    T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);

#endregion

#region ActSpan

[PublicAPI]
public delegate void ActSpan<T>(Span<T> span);

[PublicAPI]
public delegate void ActSpan<T, in T1>(Span<T> span, T1 arg1);

[PublicAPI]
public delegate void ActSpan<T, in T1, in T2>(Span<T> span, T1 arg1, T2 arg2);

[PublicAPI]
public delegate void ActSpan<T, in T1, in T2, in T3>(Span<T> span, T1 arg1, T2 arg2, T3 arg3);

[PublicAPI]
public delegate void ActSpan<T, in T1, in T2, in T3, in T4>(Span<T> span, T1 arg1, T2 arg2, T3 arg3, T4 arg4);

[PublicAPI]
public delegate void ActSpan<T, in T1, in T2, in T3, in T4, in T5>(Span<T> span, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

[PublicAPI]
public delegate void ActSpan<T, in T1, in T2, in T3, in T4, in T5, in T6>(Span<T> span, T1 arg1, T2 arg2, T3 arg3, T4 arg4,
    T5 arg5, T6 arg6);

[PublicAPI]
public delegate void ActSpan<T, in T1, in T2, in T3, in T4, in T5, in T6, in T7>(Span<T> span, T1 arg1, T2 arg2, T3 arg3,
    T4 arg4, T5 arg5, T6 arg6, T7 arg7);

[PublicAPI]
public delegate void ActSpan<T, in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8>(Span<T> span, T1 arg1, T2 arg2, T3 arg3,
    T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);

#endregion


#region Fn

[PublicAPI]
public delegate R Fn<out R>();

[PublicAPI]
public delegate R Fn<in T1, out R>(T1 arg1);

[PublicAPI]
public delegate R Fn<in T1, in T2, out R>(T1 arg1, T2 arg2);

[PublicAPI]
public delegate R Fn<in T1, in T2, in T3, out R>(T1 arg1, T2 arg2, T3 arg3);

[PublicAPI]
public delegate R Fn<in T1, in T2, in T3, in T4, out R>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);

[PublicAPI]
public delegate R Fn<in T1, in T2, in T3, in T4, in T5, out R>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

[PublicAPI]
public delegate R Fn<in T1, in T2, in T3, in T4, in T5, in T6, out R>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);

[PublicAPI]
public delegate R Fn<in T1, in T2, in T3, in T4, in T5, in T6, in T7, out R>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5,
    T6 arg6, T7 arg7);

[PublicAPI]
public delegate R Fn<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, out R>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5,
    T6 arg6, T7 arg7, T8 arg8);

#endregion

#region FnRef

[PublicAPI]
public delegate R FnRef<TR, out R>(ref TR refArg);

[PublicAPI]
public delegate R FnRef<TR, in T1, out R>(ref TR refArg, T1 arg1);

[PublicAPI]
public delegate R FnRef<TR, in T1, in T2, out R>(ref TR refArg, T1 arg1, T2 arg2);

[PublicAPI]
public delegate R FnRef<TR, in T1, in T2, in T3, out R>(ref TR refArg, T1 arg1, T2 arg2, T3 arg3);

[PublicAPI]
public delegate R FnRef<TR, in T1, in T2, in T3, in T4, out R>(ref TR refArg, T1 arg1, T2 arg2, T3 arg3, T4 arg4);

[PublicAPI]
public delegate R FnRef<TR, in T1, in T2, in T3, in T4, in T5, out R>(ref TR refArg, T1 arg1, T2 arg2, T3 arg3, T4 arg4,
    T5 arg5);

[PublicAPI]
public delegate R FnRef<TR, in T1, in T2, in T3, in T4, in T5, in T6, out R>(ref TR refArg, T1 arg1, T2 arg2, T3 arg3, T4 arg4,
    T5 arg5, T6 arg6);

[PublicAPI]
public delegate R FnRef<TR, in T1, in T2, in T3, in T4, in T5, in T6, in T7, out R>(ref TR refArg, T1 arg1, T2 arg2, T3 arg3,
    T4 arg4, T5 arg5, T6 arg6, T7 arg7);

[PublicAPI]
public delegate R FnRef<TR, in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, out R>(ref TR refArg, T1 arg1, T2 arg2,
    T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);

#endregion


#region FnRSpan

[PublicAPI]
public delegate R FnRSpan<T, out R>(ReadOnlySpan<T> span);

[PublicAPI]
public delegate R FnRSpan<T, in T1, out R>(ReadOnlySpan<T> span, T1 arg1);

[PublicAPI]
public delegate R FnRSpan<T, in T1, in T2, out R>(ReadOnlySpan<T> span, T1 arg1, T2 arg2);

[PublicAPI]
public delegate R FnRSpan<T, in T1, in T2, in T3, out R>(ReadOnlySpan<T> span, T1 arg1, T2 arg2, T3 arg3);

[PublicAPI]
public delegate R FnRSpan<T, in T1, in T2, in T3, in T4, out R>(ReadOnlySpan<T> span, T1 arg1, T2 arg2, T3 arg3, T4 arg4);

[PublicAPI]
public delegate R FnRSpan<T, in T1, in T2, in T3, in T4, in T5, out R>(ReadOnlySpan<T> span, T1 arg1, T2 arg2, T3 arg3, T4 arg4,
    T5 arg5);

[PublicAPI]
public delegate R FnRSpan<T, in T1, in T2, in T3, in T4, in T5, in T6, out R>(ReadOnlySpan<T> span, T1 arg1, T2 arg2, T3 arg3,
    T4 arg4, T5 arg5, T6 arg6);

[PublicAPI]
public delegate R FnRSpan<T, in T1, in T2, in T3, in T4, in T5, in T6, in T7, out R>(ReadOnlySpan<T> span, T1 arg1, T2 arg2,
    T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);

[PublicAPI]
public delegate R FnRSpan<T, in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, out R>(ReadOnlySpan<T> span, T1 arg1,
    T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);

#endregion

#region RSpanFnRSpan

[PublicAPI]
public delegate ReadOnlySpan<R> RSpanFnRSpan<T, R>(ReadOnlySpan<T> span);

[PublicAPI]
public delegate ReadOnlySpan<R> RSpanFnRSpan<T, in T1, R>(ReadOnlySpan<T> span, T1 arg1);

[PublicAPI]
public delegate ReadOnlySpan<R> RSpanFnRSpan<T, in T1, in T2, R>(ReadOnlySpan<T> span, T1 arg1, T2 arg2);

[PublicAPI]
public delegate ReadOnlySpan<R> RSpanFnRSpan<T, in T1, in T2, in T3, R>(ReadOnlySpan<T> span, T1 arg1, T2 arg2, T3 arg3);

[PublicAPI]
public delegate ReadOnlySpan<R> RSpanFnRSpan<T, in T1, in T2, in T3, in T4, R>(ReadOnlySpan<T> span, T1 arg1, T2 arg2, T3 arg3,
    T4 arg4);

[PublicAPI]
public delegate ReadOnlySpan<R> RSpanFnRSpan<T, in T1, in T2, in T3, in T4, in T5, R>(ReadOnlySpan<T> span, T1 arg1, T2 arg2,
    T3 arg3, T4 arg4, T5 arg5);

[PublicAPI]
public delegate ReadOnlySpan<R> RSpanFnRSpan<T, in T1, in T2, in T3, in T4, in T5, in T6, R>(ReadOnlySpan<T> span, T1 arg1,
    T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);

[PublicAPI]
public delegate ReadOnlySpan<R> RSpanFnRSpan<T, in T1, in T2, in T3, in T4, in T5, in T6, in T7, R>(ReadOnlySpan<T> span,
    T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);

[PublicAPI]
public delegate ReadOnlySpan<R> RSpanFnRSpan<T, in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, R>(ReadOnlySpan<T> span,
    T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);

#endregion

#region SpanFnRSpan

[PublicAPI]
public delegate Span<R> SpanFnRSpan<T, R>(ReadOnlySpan<T> span);

[PublicAPI]
public delegate Span<R> SpanFnRSpan<T, in T1, R>(ReadOnlySpan<T> span, T1 arg1);

[PublicAPI]
public delegate Span<R> SpanFnRSpan<T, in T1, in T2, R>(ReadOnlySpan<T> span, T1 arg1, T2 arg2);

[PublicAPI]
public delegate Span<R> SpanFnRSpan<T, in T1, in T2, in T3, R>(ReadOnlySpan<T> span, T1 arg1, T2 arg2, T3 arg3);

[PublicAPI]
public delegate Span<R> SpanFnRSpan<T, in T1, in T2, in T3, in T4, R>(ReadOnlySpan<T> span, T1 arg1, T2 arg2, T3 arg3, T4 arg4);

[PublicAPI]
public delegate Span<R> SpanFnRSpan<T, in T1, in T2, in T3, in T4, in T5, R>(ReadOnlySpan<T> span, T1 arg1, T2 arg2, T3 arg3,
    T4 arg4, T5 arg5);

[PublicAPI]
public delegate Span<R> SpanFnRSpan<T, in T1, in T2, in T3, in T4, in T5, in T6, R>(ReadOnlySpan<T> span, T1 arg1, T2 arg2,
    T3 arg3, T4 arg4, T5 arg5, T6 arg6);

[PublicAPI]
public delegate Span<R> SpanFnRSpan<T, in T1, in T2, in T3, in T4, in T5, in T6, in T7, R>(ReadOnlySpan<T> span, T1 arg1,
    T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);

[PublicAPI]
public delegate Span<R> SpanFnRSpan<T, in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, R>(ReadOnlySpan<T> span, T1 arg1,
    T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);

#endregion


#region FnSpan

[PublicAPI]
public delegate R FnSpan<T, out R>(Span<T> span);

[PublicAPI]
public delegate R FnSpan<T, in T1, out R>(Span<T> span, T1 arg1);

[PublicAPI]
public delegate R FnSpan<T, in T1, in T2, out R>(Span<T> span, T1 arg1, T2 arg2);

[PublicAPI]
public delegate R FnSpan<T, in T1, in T2, in T3, out R>(Span<T> span, T1 arg1, T2 arg2, T3 arg3);

[PublicAPI]
public delegate R FnSpan<T, in T1, in T2, in T3, in T4, out R>(Span<T> span, T1 arg1, T2 arg2, T3 arg3, T4 arg4);

[PublicAPI]
public delegate R
    FnSpan<T, in T1, in T2, in T3, in T4, in T5, out R>(Span<T> span, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

[PublicAPI]
public delegate R FnSpan<T, in T1, in T2, in T3, in T4, in T5, in T6, out R>(Span<T> span, T1 arg1, T2 arg2, T3 arg3, T4 arg4,
    T5 arg5, T6 arg6);

[PublicAPI]
public delegate R FnSpan<T, in T1, in T2, in T3, in T4, in T5, in T6, in T7, out R>(Span<T> span, T1 arg1, T2 arg2, T3 arg3,
    T4 arg4, T5 arg5, T6 arg6, T7 arg7);

[PublicAPI]
public delegate R FnSpan<T, in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, out R>(Span<T> span, T1 arg1, T2 arg2,
    T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);

#endregion

#region RSpanFnSpan

[PublicAPI]
public delegate ReadOnlySpan<R> RSpanFnSpan<T, R>(Span<T> span);

[PublicAPI]
public delegate ReadOnlySpan<R> RSpanFnSpan<T, in T1, R>(Span<T> span, T1 arg1);

[PublicAPI]
public delegate ReadOnlySpan<R> RSpanFnSpan<T, in T1, in T2, R>(Span<T> span, T1 arg1, T2 arg2);

[PublicAPI]
public delegate ReadOnlySpan<R> RSpanFnSpan<T, in T1, in T2, in T3, R>(Span<T> span, T1 arg1, T2 arg2, T3 arg3);

[PublicAPI]
public delegate ReadOnlySpan<R> RSpanFnSpan<T, in T1, in T2, in T3, in T4, R>(Span<T> span, T1 arg1, T2 arg2, T3 arg3, T4 arg4);

[PublicAPI]
public delegate ReadOnlySpan<R> RSpanFnSpan<T, in T1, in T2, in T3, in T4, in T5, R>(Span<T> span, T1 arg1, T2 arg2, T3 arg3,
    T4 arg4, T5 arg5);

[PublicAPI]
public delegate ReadOnlySpan<R> RSpanFnSpan<T, in T1, in T2, in T3, in T4, in T5, in T6, R>(Span<T> span, T1 arg1, T2 arg2,
    T3 arg3, T4 arg4, T5 arg5, T6 arg6);

[PublicAPI]
public delegate ReadOnlySpan<R> RSpanFnSpan<T, in T1, in T2, in T3, in T4, in T5, in T6, in T7, R>(Span<T> span, T1 arg1,
    T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);

[PublicAPI]
public delegate ReadOnlySpan<R> RSpanFnSpan<T, in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, R>(Span<T> span, T1 arg1,
    T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);

#endregion

#region SpanFnSpan

[PublicAPI]
public delegate Span<R> SpanFnSpan<T, R>(Span<T> span);

[PublicAPI]
public delegate Span<R> SpanFnSpan<T, in T1, R>(Span<T> span, T1 arg1);

[PublicAPI]
public delegate Span<R> SpanFnSpan<T, in T1, in T2, R>(Span<T> span, T1 arg1, T2 arg2);

[PublicAPI]
public delegate Span<R> SpanFnSpan<T, in T1, in T2, in T3, R>(Span<T> span, T1 arg1, T2 arg2, T3 arg3);

[PublicAPI]
public delegate Span<R> SpanFnSpan<T, in T1, in T2, in T3, in T4, R>(Span<T> span, T1 arg1, T2 arg2, T3 arg3, T4 arg4);

[PublicAPI]
public delegate Span<R> SpanFnSpan<T, in T1, in T2, in T3, in T4, in T5, R>(Span<T> span, T1 arg1, T2 arg2, T3 arg3, T4 arg4,
    T5 arg5);

[PublicAPI]
public delegate Span<R> SpanFnSpan<T, in T1, in T2, in T3, in T4, in T5, in T6, R>(Span<T> span, T1 arg1, T2 arg2, T3 arg3,
    T4 arg4, T5 arg5, T6 arg6);

[PublicAPI]
public delegate Span<R> SpanFnSpan<T, in T1, in T2, in T3, in T4, in T5, in T6, in T7, R>(Span<T> span, T1 arg1, T2 arg2,
    T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);

[PublicAPI]
public delegate Span<R> SpanFnSpan<T, in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, R>(Span<T> span, T1 arg1, T2 arg2,
    T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);

#endregion

#endif