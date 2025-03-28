// File name must match type name

#pragma warning disable MA0048, CA1005

// ReSharper disable InconsistentNaming
namespace ScrubJay;

#if NET9_0_OR_GREATER
public delegate void Act();
public delegate void Act<in T1>(T1 arg1)
    where T1 : allows ref struct;

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

public delegate void Act<in T1, in T2, in T3, in T4, in T5, in T6>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct
    where T6 : allows ref struct;

public delegate void Act<in T1, in T2, in T3, in T4, in T5, in T6, in T7>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct
    where T6 : allows ref struct
    where T7 : allows ref struct;

public delegate void Act<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8>(
    T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct
    where T6 : allows ref struct
    where T7 : allows ref struct
    where T8 : allows ref struct;


public delegate TReturn Fn<out TReturn>()
    where TReturn : allows ref struct;

public delegate TReturn Fn<in T1, out TReturn>(T1 arg1)
    where T1 : allows ref struct
    where TReturn : allows ref struct;

public delegate TReturn Fn<in T1, in T2, out TReturn>(T1 arg1, T2 arg2)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where TReturn : allows ref struct;

public delegate TReturn Fn<in T1, in T2, in T3, out TReturn>(T1 arg1, T2 arg2, T3 arg3)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where TReturn : allows ref struct;

public delegate TReturn Fn<in T1, in T2, in T3, in T4, out TReturn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where TReturn : allows ref struct;

public delegate TReturn Fn<in T1, in T2, in T3, in T4, in T5, out TReturn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct
    where TReturn : allows ref struct;

public delegate TReturn Fn<in T1, in T2, in T3, in T4, in T5, in T6, out TReturn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct
    where T6 : allows ref struct
    where TReturn : allows ref struct;

public delegate TReturn Fn<in T1, in T2, in T3, in T4, in T5, in T6, in T7, out TReturn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct
    where T6 : allows ref struct
    where T7 : allows ref struct
    where TReturn : allows ref struct;

public delegate TReturn Fn<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, out TReturn>(
    T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct
    where T6 : allows ref struct
    where T7 : allows ref struct
    where T8 : allows ref struct
    where TReturn : allows ref struct;
#else

public delegate void Act();
public delegate void Act<in T1>(T1 arg1);

public delegate void Act<in T1, in T2>(T1 arg1, T2 arg2);

public delegate void Act<in T1, in T2, in T3>(T1 arg1, T2 arg2, T3 arg3);

public delegate void Act<in T1, in T2, in T3, in T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);

public delegate void Act<in T1, in T2, in T3, in T4, in T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

public delegate void Act<in T1, in T2, in T3, in T4, in T5, in T6>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);

public delegate void Act<in T1, in T2, in T3, in T4, in T5, in T6, in T7>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);

public delegate void Act<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8>(
    T1 arg1,
    T2 arg2,
    T3 arg3,
    T4 arg4,
    T5 arg5,
    T6 arg6,
    T7 arg7,
    T8 arg8);

public delegate TReturn Fn<out TReturn>();
public delegate TReturn Fn<in T1, out TReturn>(T1 arg1);
public delegate TReturn Fn<in T1, in T2, out TReturn>(T1 arg1, T2 arg2);
public delegate TReturn Fn<in T1, in T2, in T3, out TReturn>(T1 arg1, T2 arg2, T3 arg3);
public delegate TReturn Fn<in T1, in T2, in T3, in T4, out TReturn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
public delegate TReturn Fn<in T1, in T2, in T3, in T4, in T5, out TReturn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
public delegate TReturn Fn<in T1, in T2, in T3, in T4, in T5, in T6, out TReturn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);

public delegate TReturn Fn<in T1, in T2, in T3, in T4, in T5, in T6, in T7, out TReturn>(
    T1 arg1,
    T2 arg2,
    T3 arg3,
    T4 arg4,
    T5 arg5,
    T6 arg6,
    T7 arg7);

public delegate TReturn Fn<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, out TReturn>(
    T1 arg1,
    T2 arg2,
    T3 arg3,
    T4 arg4,
    T5 arg5,
    T6 arg6,
    T7 arg7,
    T8 arg8);
#endif


/// <summary>
/// Support for <see cref="Span{T}"/> and <see cref="ReadOnlySpan{T}"/> in Action and Functions
/// </summary>
/// <remarks>
/// Naming scheme (regex):<br/>
/// `(ROS|S|)?(Action|Func)(ROS|S|)?`<br/>
/// - group 1<br/>
///   - ROS indicates a return type of ReadOnlySpan&lt;R&gt;<br/>
///   - S indicates a return type of Span&lt;R&gt;<br/>
///   - _ indicates a return type of R<br/>
/// - group 2<br/>
///   - Action indicates that there is no return type<br/>
///   - Func indicates there is<br/>
/// - group 3<br/>
///   - ROS indicates the first arg is a ReadOnlySpan&lt;T1&gt;<br/>
///   - S indicates the first arg is a Span&lt;T1&gt;<br/>
///   - _ indicates the first arg is a T1<br/>
/// </remarks>
public static class SpanDelegates
{

    public delegate void ActionROS<T1>(ReadOnlySpan<T1> arg1);
    public delegate void ActionROS<T1, in T2>(ReadOnlySpan<T1> arg1, T2 arg2);
    public delegate void ActionROS<T1, in T2, in T3>(ReadOnlySpan<T1> arg1, T2 arg2, T3 arg3);
    public delegate void ActionROS<T1, in T2, in T3, in T4>(ReadOnlySpan<T1> arg1, T2 arg2, T3 arg3, T4 arg4);
    public delegate void ActionROS<T1, in T2, in T3, in T4, in T5>(ReadOnlySpan<T1> arg1, T2 arg2, T3 arg3, T4 arg4, T4 arg5);

    public delegate void ActionS<T1>(Span<T1> arg1);
    public delegate void ActionS<T1, in T2>(Span<T1> arg1, T2 arg2);
    public delegate void ActionS<T1, in T2, in T3>(Span<T1> arg1, T2 arg2, T3 arg3);
    public delegate void ActionS<T1, in T2, in T3, in T4>(Span<T1> arg1, T2 arg2, T3 arg3, T4 arg4);
    public delegate void ActionS<T1, in T2, in T3, in T4, in T5>(Span<T1> arg1, T2 arg2, T3 arg3, T4 arg4, T4 arg5);

    public delegate R FuncROS<T1, out R>(ReadOnlySpan<T1> arg1);
    public delegate R FuncROS<T1, in T2, out R>(ReadOnlySpan<T1> arg1, T2 arg2);
    public delegate R FuncROS<T1, in T2, in T3, out R>(ReadOnlySpan<T1> arg1, T2 arg2, T3 arg3);
    public delegate R FuncROS<T1, in T2, in T3, in T4, out R>(ReadOnlySpan<T1> arg1, T2 arg2, T3 arg3, T4 arg4);
    public delegate R FuncROS<T1, in T2, in T3, in T4, in T5, out R>(ReadOnlySpan<T1> arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

    public delegate R FuncS<T1, out R>(Span<T1> arg1);
    public delegate R FuncS<T1, in T2, out R>(Span<T1> arg1, T2 arg2);
    public delegate R FuncS<T1, in T2, in T3, out R>(Span<T1> arg1, T2 arg2, T3 arg3);
    public delegate R FuncS<T1, in T2, in T3, in T4, out R>(Span<T1> arg1, T2 arg2, T3 arg3, T4 arg4);
    public delegate R FuncS<T1, in T2, in T3, in T4, in T5, out R>(Span<T1> arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

    public delegate ReadOnlySpan<R> ROSFunc<R>();
    public delegate ReadOnlySpan<R> ROSFunc<in T1, R>(T1 arg1);
    public delegate ReadOnlySpan<R> ROSFunc<in T1, in T2, R>(T1 arg1, T2 arg2);
    public delegate ReadOnlySpan<R> ROSFunc<in T1, in T2, in T3, R>(T1 arg1, T2 arg2, T3 arg3);
    public delegate ReadOnlySpan<R> ROSFunc<in T1, in T2, in T3, in T4, R>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    public delegate ReadOnlySpan<R> ROSFunc<in T1, in T2, in T3, in T4, in T5, R>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

    public delegate Span<R> SFunc<R>();
    public delegate Span<R> SFunc<in T1, R>(T1 arg1);
    public delegate Span<R> SFunc<in T1, in T2, R>(T1 arg1, T2 arg2);
    public delegate Span<R> SFunc<in T1, in T2, in T3, R>(T1 arg1, T2 arg2, T3 arg3);
    public delegate Span<R> SFunc<in T1, in T2, in T3, in T4, R>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    public delegate Span<R> SFunc<in T1, in T2, in T3, in T4, in T5, R>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);


    public delegate ReadOnlySpan<R> ROSFuncROS<T1, R>(ReadOnlySpan<T1> arg1);
    public delegate ReadOnlySpan<R> ROSFuncROS<T1, in T2, R>(ReadOnlySpan<T1> arg1, T2 arg2);
    public delegate ReadOnlySpan<R> ROSFuncROS<T1, in T2, in T3, R>(ReadOnlySpan<T1> arg1, T2 arg2, T3 arg3);
    public delegate ReadOnlySpan<R> ROSFuncROS<T1, in T2, in T3, in T4, R>(ReadOnlySpan<T1> arg1, T2 arg2, T3 arg3, T4 arg4);
    public delegate ReadOnlySpan<R> ROSFuncROS<T1, in T2, in T3, in T4, in T5, R>(ReadOnlySpan<T1> arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

    public delegate ReadOnlySpan<R> ROSFuncS<T1, R>(Span<T1> arg1);
    public delegate ReadOnlySpan<R> ROSFuncS<T1, in T2, R>(Span<T1> arg1, T2 arg2);
    public delegate ReadOnlySpan<R> ROSFuncS<T1, in T2, in T3, R>(Span<T1> arg1, T2 arg2, T3 arg3);
    public delegate ReadOnlySpan<R> ROSFuncS<T1, in T2, in T3, in T4, R>(Span<T1> arg1, T2 arg2, T3 arg3, T4 arg4);
    public delegate ReadOnlySpan<R> ROSFuncS<T1, in T2, in T3, in T4, in T5, R>(Span<T1> arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

    public delegate Span<R> SFuncROS<T1, R>(ReadOnlySpan<T1> arg1);
    public delegate Span<R> SFuncROS<T1, in T2, R>(ReadOnlySpan<T1> arg1, T2 arg2);
    public delegate Span<R> SFuncROS<T1, in T2, in T3, R>(ReadOnlySpan<T1> arg1, T2 arg2, T3 arg3);
    public delegate Span<R> SFuncROS<T1, in T2, in T3, in T4, R>(ReadOnlySpan<T1> arg1, T2 arg2, T3 arg3, T4 arg4);
    public delegate Span<R> SFuncROS<T1, in T2, in T3, in T4, in T5, R>(ReadOnlySpan<T1> arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

    public delegate Span<R> SFuncS<T1, R>(Span<T1> arg1);
    public delegate Span<R> SFuncS<T1, in T2, R>(Span<T1> arg1, T2 arg2);
    public delegate Span<R> SFuncS<T1, in T2, in T3, R>(Span<T1> arg1, T2 arg2, T3 arg3);
    public delegate Span<R> SFuncS<T1, in T2, in T3, in T4, R>(Span<T1> arg1, T2 arg2, T3 arg3, T4 arg4);
    public delegate Span<R> SFuncS<T1, in T2, in T3, in T4, in T5, R>(Span<T1> arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
}

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
