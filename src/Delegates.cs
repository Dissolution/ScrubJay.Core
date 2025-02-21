// File name must match type name
#pragma warning disable MA0048, CA1005

// ReSharper disable InconsistentNaming
namespace ScrubJay;

public delegate void Act();

#if NET9_0_OR_GREATER
public delegate void Act<in T>(T arg)
    where T : allows ref struct;

public delegate void Act<in T1, in T2>(T1 arg1, T2 arg2)
    where T1 : allows ref struct
    where T2 : allows ref struct;

public delegate void Act<in T1, in T2, in T3>(T1 arg1, T2 arg2, T3 arg3)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct;

public delegate void Act<in T1, in T2, in T3, in T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct;

public delegate void Act<in T1, in T2, in T3, in T4, in T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct;

public delegate TReturn Fun<out TReturn>()
    where TReturn : allows ref struct;

public delegate TReturn Fun<in T1, out TReturn>(T1 arg1)
    where T1 : allows ref struct
    where TReturn : allows ref struct;

public delegate TReturn Fun<in T1, in T2, out TReturn>(T1 arg1, T2 arg2)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where TReturn : allows ref struct;

public delegate TReturn Fun<in T1, in T2, in T3, out TReturn>(T1 arg1, T2 arg2, T3 arg3)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where TReturn : allows ref struct;

public delegate TReturn Fun<in T1, in T2, in T3, in T4, out TReturn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where TReturn : allows ref struct;

public delegate TReturn Fun<in T1, in T2, in T3, in T4, in T5, out TReturn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct
    where TReturn : allows ref struct;

#else
public delegate void Act<in T1>(T1 arg1);
public delegate void Act<in T1, in T2>(T1 arg1, T2 arg2);
public delegate void Act<in T1, in T2, in T3>(T1 arg1, T2 arg2, T3 arg3);
public delegate void Act<in T1, in T2, in T3, in T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
public delegate void Act<in T1, in T2, in T3, in T4, in T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

public delegate TReturn Fun<out TReturn>();
public delegate TReturn Fun<in T1, out TReturn>(T1 arg1);
public delegate TReturn Fun<in T1, in T2, out TReturn>(T1 arg1, T2 arg2);
public delegate TReturn Fun<in T1, in T2, in T3, out TReturn>(T1 arg1, T2 arg2, T3 arg3);
public delegate TReturn Fun<in T1, in T2, in T3, in T4, out TReturn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
public delegate TReturn Fun<in T1, in T2, in T3, in T4, in T5, out TReturn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
#endif


/// <summary>
/// <see cref="Action{T}">Action</see>&lt;<see cref="ReadOnlySpan{T}"/>&gt;
/// </summary>
/// <typeparam name="T">
/// The <see cref="Type"/> of items in the <see cref="ReadOnlySpan{T}"/>
/// </typeparam>
/// <param name="span">
/// The <see cref="ReadOnlySpan{T}"/> argument for the <see cref="Action{T}"/>
/// </param>
public delegate void RSAct<T>(ReadOnlySpan<T> span);
public delegate void RSAct<TS, in T1>(ReadOnlySpan<TS> span, T1 arg1);
public delegate void RSAct<TS, in T1, in T2>(ReadOnlySpan<TS> span, T1 arg1, T2 arg2);
public delegate void RSAct<TS, in T1, in T2, in T3>(ReadOnlySpan<TS> span, T1 arg1, T2 arg2, T3 arg3);
public delegate void RSAct<TS, in T1, in T2, in T3, in T4>(ReadOnlySpan<TS> span, T1 arg1, T2 arg2, T3 arg3, T4 arg4);


public delegate void WSAct<T>(Span<T> span);
public delegate void WSAct<TS, in T1>(Span<TS> span, T1 arg1);
public delegate void WSAct<TS, in T1, in T2>(Span<TS> span, T1 arg1, T2 arg2);
public delegate void WSAct<TS, in T1, in T2, in T3>(Span<TS> span, T1 arg1, T2 arg2, T3 arg3);
public delegate void WSAct<TS, in T1, in T2, in T3, in T4>(Span<TS> span, T1 arg1, T2 arg2, T3 arg3, T4 arg4);


public delegate TResult RSFun<TS, out TResult>(ReadOnlySpan<TS> span);
public delegate TResult RSFun<TS, in T1, out TResult>(ReadOnlySpan<TS> span, T1 arg1);
public delegate TResult RSFun<TS, in T1, in T2, out TResult>(ReadOnlySpan<TS> span, T1 arg1, T2 arg2);
public delegate TResult RSFun<TS, in T1, in T2, in T3, out TResult>(ReadOnlySpan<TS> span, T1 arg1, T2 arg2, T3 arg3);
public delegate TResult RSFun<TS, in T1, in T2, in T3, in T4, out TResult>(ReadOnlySpan<TS> span, T1 arg1, T2 arg2, T3 arg3, T4 arg4);


public delegate TResult WSFun<TS, out TResult>(Span<TS> span);
public delegate TResult WSFun<TS, in T1, out TResult>(Span<TS> span, T1 arg1);
public delegate TResult WSFun<TS, in T1, in T2, out TResult>(Span<TS> span, T1 arg1, T2 arg2);
public delegate TResult WSFun<TS, in T1, in T2, in T3, out TResult>(Span<TS> span, T1 arg1, T2 arg2, T3 arg3);
public delegate TResult WSFun<TS, in T1, in T2, in T3, in T4, out TResult>(Span<TS> span, T1 arg1, T2 arg2, T3 arg3, T4 arg4);


public delegate ReadOnlySpan<TS> FunRS<TS>();
public delegate ReadOnlySpan<TS> FunRS<TS, in T1>(T1 arg1);
public delegate ReadOnlySpan<TS> FunRS<TS, in T1, in T2>(T1 arg1, T2 arg2);
public delegate ReadOnlySpan<TS> FunRS<TS, in T1, in T2, in T3>(T1 arg1, T2 arg2, T3 arg3);
public delegate ReadOnlySpan<TS> FunRS<TS, in T1, in T2, in T3, in T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);

public delegate Span<TS> FunWS<TS>();
public delegate Span<TS> FunWS<TS, in T1>(T1 arg1);
public delegate Span<TS> FunWS<TS, in T1, in T2>(T1 arg1, T2 arg2);
public delegate Span<TS> FunWS<TS, in T1, in T2, in T3>(T1 arg1, T2 arg2, T3 arg3);
public delegate Span<TS> FunWS<TS, in T1, in T2, in T3, in T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);


public delegate ReadOnlySpan<TSOut> RSFunRS<TSIn, TSOut>(ReadOnlySpan<TSIn> span);
public delegate ReadOnlySpan<TSOut> RSFunRS<TSIn, in T1, TSOut>(ReadOnlySpan<TSIn> span, T1 arg1);
public delegate ReadOnlySpan<TSOut> RSFunRS<TSIn, in T1, in T2, TSOut>(ReadOnlySpan<TSIn> span, T1 arg1, T2 arg2);
public delegate ReadOnlySpan<TSOut> RSFunRS<TSIn, in T1, in T2, in T3, TSOut>(ReadOnlySpan<TSIn> span, T1 arg1, T2 arg2, T3 arg3);
public delegate ReadOnlySpan<TSOut> RSFunRS<TSIn, in T1, in T2, in T3, in T4, TSOut>(ReadOnlySpan<TSIn> span, T1 arg1, T2 arg2, T3 arg3, T4 arg4);

public delegate ReadOnlySpan<TSOut> WSFunRS<TSIn, TSOut>(Span<TSIn> span);
public delegate ReadOnlySpan<TSOut> WSFunRS<TSIn, in T1, TSOut>(Span<TSIn> span, T1 arg1);
public delegate ReadOnlySpan<TSOut> WSFunRS<TSIn, in T1, in T2, TSOut>(Span<TSIn> span, T1 arg1, T2 arg2);
public delegate ReadOnlySpan<TSOut> WSFunRS<TSIn, in T1, in T2, in T3, TSOut>(Span<TSIn> span, T1 arg1, T2 arg2, T3 arg3);
public delegate ReadOnlySpan<TSOut> WSFunRS<TSIn, in T1, in T2, in T3, in T4, TSOut>(Span<TSIn> span, T1 arg1, T2 arg2, T3 arg3, T4 arg4);

public delegate Span<TSOut> WSFunWS<TSIn, TSOut>(Span<TSIn> span);
public delegate Span<TSOut> WSFunWS<TSIn, in T1, TSOut>(Span<TSIn> span, T1 arg1);
public delegate Span<TSOut> WSFunWS<TSIn, in T1, in T2, TSOut>(Span<TSIn> span, T1 arg1, T2 arg2);
public delegate Span<TSOut> WSFunWS<TSIn, in T1, in T2, in T3, TSOut>(Span<TSIn> span, T1 arg1, T2 arg2, T3 arg3);
public delegate Span<TSOut> WSFunWS<TSIn, in T1, in T2, in T3, in T4, TSOut>(Span<TSIn> span, T1 arg1, T2 arg2, T3 arg3, T4 arg4);

public delegate Span<TSOut> RSFunWS<TSIn, TSOut>(ReadOnlySpan<TSIn> span);
public delegate Span<TSOut> RSFunWS<TSIn, in T1, TSOut>(ReadOnlySpan<TSIn> span, T1 arg1);
public delegate Span<TSOut> RSFunWS<TSIn, in T1, in T2, TSOut>(ReadOnlySpan<TSIn> span, T1 arg1, T2 arg2);
public delegate Span<TSOut> RSFunWS<TSIn, in T1, in T2, in T3, TSOut>(ReadOnlySpan<TSIn> span, T1 arg1, T2 arg2, T3 arg3);
public delegate Span<TSOut> RSFunWS<TSIn, in T1, in T2, in T3, in T4, TSOut>(ReadOnlySpan<TSIn> span, T1 arg1, T2 arg2, T3 arg3, T4 arg4);

/// <summary>
/// A delegate that acts upon an item reference
/// </summary>
/// <typeparam name="T">
/// The <see cref="Type"/> of <paramref name="item"/> being referenced
/// </typeparam>
/// <param name="item">
/// The item being used by <c>ref</c>
/// </param>
[PublicAPI]
public delegate void RefItem<T>(ref T item);

/// <summary>
/// A delegate that acts upon an item reference and its index
/// </summary>
/// <typeparam name="T">
/// The <see cref="Type"/> of <paramref name="item"/> being referenced
/// </typeparam>
/// <param name="item">
/// The item being used by <c>ref</c>
/// </param>
/// <param name="index">
/// The index of the referenced item
/// </param>
[PublicAPI]
public delegate void RefItemAndIndex<T>(ref T item, int index);

/// <summary>
/// A delegate that represents access to an available, unwritten portion of a <see cref="Span{T}"/>
/// </summary>
/// <param name="buffer">
/// The available <see cref="Span{T}"/>
/// </param>
/// <returns>
/// The number of items added to <paramref name="buffer"/>
/// </returns>
[PublicAPI]
public delegate int UseAvailable<T>(Span<T> buffer);
