// ReSharper disable InconsistentNaming
namespace ScrubJay;

/// <summary>
/// <see cref="Action{T}">Action</see>&lt;<see cref="ReadOnlySpan{T}"/>&gt;
/// </summary>
/// <typeparam name="T">
/// The <see cref="Type"/> of items in the <see cref="ReadOnlySpan{T}"/>
/// </typeparam>
/// <param name="span">
/// The <see cref="ReadOnlySpan{T}"/> argument for the <see cref="Action{T}"/>
/// </param>
public delegate void RSpanAction<T>(ReadOnlySpan<T> span);
public delegate void RSpanAction<TS, in T1>(ReadOnlySpan<TS> span, T1 arg1);
public delegate void RSpanAction<TS, in T1, in T2>(ReadOnlySpan<TS> span, T1 arg1, T2 arg2);
public delegate void RSpanAction<TS, in T1, in T2, in T3>(ReadOnlySpan<TS> span, T1 arg1, T2 arg2, T3 arg3);
public delegate void RSpanAction<TS, in T1, in T2, in T3, in T4>(ReadOnlySpan<TS> span, T1 arg1, T2 arg2, T3 arg3, T4 arg4);


public delegate void WSpanAction<T>(Span<T> span);
public delegate void WSpanAction<TS, in T1>(Span<TS> span, T1 arg1);
public delegate void WSpanAction<TS, in T1, in T2>(Span<TS> span, T1 arg1, T2 arg2);
public delegate void WSpanAction<TS, in T1, in T2, in T3>(Span<TS> span, T1 arg1, T2 arg2, T3 arg3);
public delegate void WSpanAction<TS, in T1, in T2, in T3, in T4>(Span<TS> span, T1 arg1, T2 arg2, T3 arg3, T4 arg4);


public delegate TResult RSpanFunc<TS, out TResult>(ReadOnlySpan<TS> span);
public delegate TResult RSpanFunc<TS, in T1, out TResult>(ReadOnlySpan<TS> span, T1 arg1);
public delegate TResult RSpanFunc<TS, in T1, in T2, out TResult>(ReadOnlySpan<TS> span, T1 arg1, T2 arg2);
public delegate TResult RSpanFunc<TS, in T1, in T2, in T3, out TResult>(ReadOnlySpan<TS> span, T1 arg1, T2 arg2, T3 arg3);
public delegate TResult RSpanFunc<TS, in T1, in T2, in T3, in T4, out TResult>(ReadOnlySpan<TS> span, T1 arg1, T2 arg2, T3 arg3, T4 arg4);


public delegate TResult WSpanFunc<TS, out TResult>(Span<TS> span);
public delegate TResult WSpanFunc<TS, in T1, out TResult>(Span<TS> span, T1 arg1);
public delegate TResult WSpanFunc<TS, in T1, in T2, out TResult>(Span<TS> span, T1 arg1, T2 arg2);
public delegate TResult WSpanFunc<TS, in T1, in T2, in T3, out TResult>(Span<TS> span, T1 arg1, T2 arg2, T3 arg3);
public delegate TResult WSpanFunc<TS, in T1, in T2, in T3, in T4, out TResult>(Span<TS> span, T1 arg1, T2 arg2, T3 arg3, T4 arg4);


public delegate ReadOnlySpan<TS> FuncRSpan<TS>();
public delegate ReadOnlySpan<TS> FuncRSpan<TS, in T1>(T1 arg1);
public delegate ReadOnlySpan<TS> FuncRSpan<TS, in T1, in T2>(T1 arg1, T2 arg2);
public delegate ReadOnlySpan<TS> FuncRSpan<TS, in T1, in T2, in T3>(T1 arg1, T2 arg2, T3 arg3);
public delegate ReadOnlySpan<TS> FuncRSpan<TS, in T1, in T2, in T3, in T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);

public delegate Span<TS> FuncWSpan<TS>();
public delegate Span<TS> FuncWSpan<TS, in T1>(T1 arg1);
public delegate Span<TS> FuncWSpan<TS, in T1, in T2>(T1 arg1, T2 arg2);
public delegate Span<TS> FuncWSpan<TS, in T1, in T2, in T3>(T1 arg1, T2 arg2, T3 arg3);
public delegate Span<TS> FuncWSpan<TS, in T1, in T2, in T3, in T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);


public delegate ReadOnlySpan<TSOut> RSpanFuncRSpan<TSIn, TSOut>(ReadOnlySpan<TSIn> span);
public delegate ReadOnlySpan<TSOut> RSpanFuncRSpan<TSIn, in T1, TSOut>(ReadOnlySpan<TSIn> span, T1 arg1);
public delegate ReadOnlySpan<TSOut> RSpanFuncRSpan<TSIn, in T1, in T2, TSOut>(ReadOnlySpan<TSIn> span, T1 arg1, T2 arg2);
public delegate ReadOnlySpan<TSOut> RSpanFuncRSpan<TSIn, in T1, in T2, in T3, TSOut>(ReadOnlySpan<TSIn> span, T1 arg1, T2 arg2, T3 arg3);
public delegate ReadOnlySpan<TSOut> RSpanFuncRSpan<TSIn, in T1, in T2, in T3, in T4, TSOut>(ReadOnlySpan<TSIn> span, T1 arg1, T2 arg2, T3 arg3, T4 arg4);

public delegate ReadOnlySpan<TSOut> WSpanFuncRSpan<TSIn, TSOut>(Span<TSIn> span);
public delegate ReadOnlySpan<TSOut> WSpanFuncRSpan<TSIn, in T1, TSOut>(Span<TSIn> span, T1 arg1);
public delegate ReadOnlySpan<TSOut> WSpanFuncRSpan<TSIn, in T1, in T2, TSOut>(Span<TSIn> span, T1 arg1, T2 arg2);
public delegate ReadOnlySpan<TSOut> WSpanFuncRSpan<TSIn, in T1, in T2, in T3, TSOut>(Span<TSIn> span, T1 arg1, T2 arg2, T3 arg3);
public delegate ReadOnlySpan<TSOut> WSpanFuncRSpan<TSIn, in T1, in T2, in T3, in T4, TSOut>(Span<TSIn> span, T1 arg1, T2 arg2, T3 arg3, T4 arg4);

public delegate Span<TSOut> WSpanFuncWSpan<TSIn, TSOut>(Span<TSIn> span);
public delegate Span<TSOut> WSpanFuncWSpan<TSIn, in T1, TSOut>(Span<TSIn> span, T1 arg1);
public delegate Span<TSOut> WSpanFuncWSpan<TSIn, in T1, in T2, TSOut>(Span<TSIn> span, T1 arg1, T2 arg2);
public delegate Span<TSOut> WSpanFuncWSpan<TSIn, in T1, in T2, in T3, TSOut>(Span<TSIn> span, T1 arg1, T2 arg2, T3 arg3);
public delegate Span<TSOut> WSpanFuncWSpan<TSIn, in T1, in T2, in T3, in T4, TSOut>(Span<TSIn> span, T1 arg1, T2 arg2, T3 arg3, T4 arg4);

public delegate Span<TSOut> RSpanFuncWSpan<TSIn, TSOut>(ReadOnlySpan<TSIn> span);
public delegate Span<TSOut> RSpanFuncWSpan<TSIn, in T1, TSOut>(ReadOnlySpan<TSIn> span, T1 arg1);
public delegate Span<TSOut> RSpanFuncWSpan<TSIn, in T1, in T2, TSOut>(ReadOnlySpan<TSIn> span, T1 arg1, T2 arg2);
public delegate Span<TSOut> RSpanFuncWSpan<TSIn, in T1, in T2, in T3, TSOut>(ReadOnlySpan<TSIn> span, T1 arg1, T2 arg2, T3 arg3);
public delegate Span<TSOut> RSpanFuncWSpan<TSIn, in T1, in T2, in T3, in T4, TSOut>(ReadOnlySpan<TSIn> span, T1 arg1, T2 arg2, T3 arg3, T4 arg4);


/// <summary>
/// A delegate that acts upon upon an item reference
/// </summary>
/// <typeparam name="T">
/// The <see cref="Type"/> of <paramref name="item"/> being referenced
/// </typeparam>
/// <param name="item">
/// The item being used by <c>ref</c>
/// </param>
public delegate void RefItem<T>(ref T item);

/// <summary>
/// A delegate that represents access to an available, unwritten portion of a <see cref="Span{T}"/>
/// </summary>
/// <param name="emptyBuffer">
/// The available <see cref="Span{T}"/>
/// </param>
/// <returns>
/// The number of items added to <paramref name="emptyBuffer"/>
/// </returns>
public delegate int UseAvailable<T>(Span<T> emptyBuffer);

