namespace ScrubJay;

/// <summary>
/// Encapsulates a method that receives a <see cref="Span{T}"/> and returns a <typeparamref name="TResult"/>
/// </summary>
/// <param name="span">
/// <see cref="Span{T}"/>
/// </param>
/// <typeparam name="T">
/// The <see cref="Type"/> of items in <paramref name="span"/>
/// </typeparam>
/// <typeparam name="TResult">
/// The <see cref="Type"/> of return result
/// </typeparam>
public delegate TResult SpanFunc<T, out TResult>(Span<T> span);

public delegate void SpanAction<T>(Span<T> span);

public delegate TResult ReadOnlySpanFunc<T, out TResult>(ReadOnlySpan<T> span);

public delegate void ReadOnlySpanAction<T>(ReadOnlySpan<T> span);

public delegate void RefAction<T>(ref T value);

public delegate int TryWrite<T>(Span<T> destination);