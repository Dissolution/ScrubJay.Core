// ReSharper disable InconsistentNaming
namespace ScrubJay;

/// <summary>
/// An <see cref="Action{T}"/> where <typeparamref name="S"/> is a <see cref="ReadOnlySpan{T}"/>
/// </summary>
/// <typeparam name="S">
/// The <see cref="Type"/> of items in the <see cref="ReadOnlySpan{T}"/>
/// </typeparam>
/// <param name="span">
/// The <see cref="ReadOnlySpan{T}"/> argument for this action
/// </param>
public delegate void RSpanAction<S>(ReadOnlySpan<S> span);


/// <summary>
/// A <see cref="Func{TResult}"/> that produces a <see cref="ReadOnlySpan{T}"/>
/// </summary>
/// <typeparam name="S">
/// The <see cref="Type"/> of items in the returned <see cref="ReadOnlySpan{T}"/>
/// </typeparam>
public delegate ReadOnlySpan<S> FuncRSpan<S>();

/// <summary>
/// A <see cref="Func{T, TResult}"/> that produces a <see cref="ReadOnlySpan{T}"/>
/// </summary>
/// <typeparam name="S">
/// The <see cref="Type"/> of items in the returned <see cref="ReadOnlySpan{T}"/>
/// </typeparam>
/// <typeparam name="T">
/// The <see cref="Type"/> of the argument of this function
/// </typeparam>
/// <param name="arg">
/// The <typeparamref name="T"/> argument for this function
/// </param>
public delegate ReadOnlySpan<S> FuncRSpan<in T, S>(T arg);

/// <summary>
/// A <see cref="Func{SIn, SOut}"/> that takes and produces <see cref="ReadOnlySpan{T}"/>s
/// </summary>
/// <typeparam name="SIn">
/// The <see cref="Type"/> of items in the input <see cref="ReadOnlySpan{SIn}"/>
/// </typeparam>
/// <typeparam name="SOut">
/// The <see cref="Type"/> of items in the output <see cref="ReadOnlySpan{SOut}"/>
/// </typeparam>
/// <param name="span">
/// The input <see cref="ReadOnlySpan{SIn}"/> for this function
/// </param>
/// <returns>
/// An output <see cref="ReadOnlySpan{SOut}"/>
/// </returns>
public delegate ReadOnlySpan<SOut> RSpanFuncRSpan<SIn, SOut>(ReadOnlySpan<SIn> span);

/// <summary>
/// A <see cref="Func{S, TResult}"/> that takes a <see cref="ReadOnlySpan{T}"/> and produces a <typeparamref name="TResult"/>
/// </summary>
/// <typeparam name="S">
/// The <see cref="Type"/> of items in the input <see cref="ReadOnlySpan{SIn}"/>
/// </typeparam>
/// <typeparam name="TResult">
/// The <see cref="Type"/> of the result value from this function
/// </typeparam>
/// <param name="span">
/// The input <see cref="ReadOnlySpan{SIn}"/> for this function
/// </param>
/// <returns>
/// An output <typeparamref name="TResult"/> value
/// </returns>
public delegate TResult RSpanFunc<S, out TResult>(ReadOnlySpan<S> span);