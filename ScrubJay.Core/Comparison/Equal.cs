// ReSharper disable InvokeAsExtensionMethod

namespace ScrubJay.Comparison;

public class Equal
{
    public static bool Objects(object? left, object? right)
    {
        return EqualityComparerCache.Equals(left, right);
    }
    
        
    /// <summary>
    /// Are the <paramref name="left"/> and <paramref name="right"/> <see cref="object">objects</see> equal?
    /// </summary>
    public static bool Object(object? left, object? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

#region Sequence

#if NET6_0_OR_GREATER
    public static bool Sequence<T>(T[]? left, T[]? right)
    {
        return MemoryExtensions.SequenceEqual<T>(left, right);
    }

    public static bool Sequence<T>(T[]? left, Span<T> right)
    {
        return MemoryExtensions.SequenceEqual<T>(left, right);
    }

    public static bool Sequence<T>(T[]? left, ReadOnlySpan<T> right)
    {
        return MemoryExtensions.SequenceEqual<T>(left, right);
    }

    public static bool Sequence<T>(Span<T> left, T[]? right)
    {
        return MemoryExtensions.SequenceEqual<T>(left, right);
    }

    public static bool Sequence<T>(Span<T> left, Span<T> right)
    {
        return MemoryExtensions.SequenceEqual<T>(left, right);
    }

    public static bool Sequence<T>(Span<T> left, ReadOnlySpan<T> right)
    {
        return MemoryExtensions.SequenceEqual<T>(left, right);
    }

    public static bool Sequence<T>(ReadOnlySpan<T> left, T[]? right)
    {
        return MemoryExtensions.SequenceEqual<T>(left, right);
    }

    public static bool Sequence<T>(ReadOnlySpan<T> left, Span<T> right)
    {
        return MemoryExtensions.SequenceEqual<T>(left, right);
    }

    public static bool Sequence<T>(ReadOnlySpan<T> left, ReadOnlySpan<T> right)
    {
        return MemoryExtensions.SequenceEqual<T>(left, right);
    }
#else
    public static bool Sequence<T>(T[]? left, T[]? right)
    {
        return SpanExtensions.SequenceEqual<T>(left, right);
    }
    public static bool Sequence<T>(T[]? left, Span<T> right)
    {
        return SpanExtensions.SequenceEqual<T>(left, right);
    }
    public static bool Sequence<T>(T[]? left, ReadOnlySpan<T> right)
    {
        return SpanExtensions.SequenceEqual<T>(left, right);
    }
    public static bool Sequence<T>(Span<T> left, T[]? right)
    {
        return SpanExtensions.SequenceEqual<T>(left, right);
    }
    public static bool Sequence<T>(Span<T> left, Span<T> right)
    {
        return SpanExtensions.SequenceEqual<T>(left, right);
    }
    public static bool Sequence<T>(Span<T> left, ReadOnlySpan<T> right)
    {
        return SpanExtensions.SequenceEqual<T>(left, right);
    }
    public static bool Sequence<T>(ReadOnlySpan<T> left, T[]? right)
    {
        return ReadOnlySpanExtensions.SequenceEqual<T>(left, right);
    }
    public static bool Sequence<T>(ReadOnlySpan<T> left, Span<T> right)
    {
        return ReadOnlySpanExtensions.SequenceEqual<T>(left, right);
    }
    public static bool Sequence<T>(ReadOnlySpan<T> left, ReadOnlySpan<T> right)
    {
        return ReadOnlySpanExtensions.SequenceEqual<T>(left, right);
    }
#endif

#endregion

#region Text

    public static bool Text(string? left, string? right)
    {
        return string.Equals(left, right);
    }

    public static bool Text(string? left, ReadOnlySpan<char> right)
    {
        return MemoryExtensions.SequenceEqual<char>(left.AsSpan(), right);
    }

    public static bool Text(string? left, char[]? right)
    {
        return MemoryExtensions.SequenceEqual<char>(left.AsSpan(), right);
    }

    public static bool Text(ReadOnlySpan<char> left, string? right)
    {
        return MemoryExtensions.SequenceEqual<char>(left, right.AsSpan());
    }

    public static bool Text(ReadOnlySpan<char> left, ReadOnlySpan<char> right)
    {
        return MemoryExtensions.SequenceEqual<char>(left, right);
    }

    public static bool Text(ReadOnlySpan<char> left, char[]? right)
    {
        return MemoryExtensions.SequenceEqual<char>(left, right);
    }

    public static bool Text(char[]? left, string? right)
    {
        return MemoryExtensions.SequenceEqual<char>(left, right.AsSpan());
    }

    public static bool Text(char[]? left, ReadOnlySpan<char> right)
    {
        return MemoryExtensions.SequenceEqual<char>(left, right);
    }

    public static bool Text(char[]? left, char[]? right)
    {
        return MemoryExtensions.SequenceEqual<char>(left, right);
    }


    public static bool Text(string? left, string? right, StringComparison comparison)
    {
        return string.Equals(left, right, comparison);
    }

    public static bool Text(string? left, ReadOnlySpan<char> right, StringComparison comparison)
    {
        return MemoryExtensions.Equals(left.AsSpan(), right, comparison);
    }

    public static bool Text(string? left, char[]? right, StringComparison comparison)
    {
        return MemoryExtensions.Equals(left.AsSpan(), right, comparison);
    }

    public static bool Text(ReadOnlySpan<char> left, string? right, StringComparison comparison)
    {
        return MemoryExtensions.Equals(left, right.AsSpan(), comparison);
    }

    public static bool Text(ReadOnlySpan<char> left, ReadOnlySpan<char> right, StringComparison comparison)
    {
        return MemoryExtensions.Equals(left, right, comparison);
    }

    public static bool Text(ReadOnlySpan<char> left, char[]? right, StringComparison comparison)
    {
        return MemoryExtensions.Equals(left, right, comparison);
    }

    public static bool Text(char[]? left, string? right, StringComparison comparison)
    {
        return MemoryExtensions.Equals(left, right.AsSpan(), comparison);
    }

    public static bool Text(char[]? left, ReadOnlySpan<char> right, StringComparison comparison)
    {
        return MemoryExtensions.Equals(left, right, comparison);
    }

    public static bool Text(char[]? left, char[]? right, StringComparison comparison)
    {
        return MemoryExtensions.Equals(left, right, comparison);
    }

#endregion
    
    #region Type

    
    /// <summary>
    /// Are the <paramref name="left"/> and <paramref name="right"/> <see cref="System.Type"/>s exactly the same? 
    /// </summary>
    public static bool Type(Type? left, Type? right) => left == right;

    /// <summary>
    /// Are the <see cref="System.Type"/>s of <paramref name="left"/> and <paramref name="right"/> exactly the same?
    /// </summary>
    public static bool Type<TL, TR>(TL? left, TR? right) => typeof(TL) == typeof(TR);

    /// <summary>
    /// Are the <see cref="System.Type"/>s of <paramref name="left"/> and <paramref name="right"/> exactly the same?
    /// </summary>
    public static bool Type(object? left, object? right) => left?.GetType() == right?.GetType();
    #endregion
    
    /// <summary>
    /// Are the <paramref name="left"/> and <paramref name="right"/> values equal?
    /// </summary>
    /// <remarks>
    /// As determined by <see cref="EqualityComparer{T}"/>.<see cref="EqualityComparer{T}.Default"/>.Equals
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Default<T>(T? left, T? right) 
        => EqualityComparer<T>.Default.Equals(left!, right!);
    
    /// <summary>
    /// Safely calls <paramref name="left"/>.Equals(<paramref name="right"/>)
    /// with <c>null</c> checking
    /// </summary>
    public static bool Safe<TL, TR>(TL? left, TR? right)
        where TL : IEquatable<TR>
    {
        if (left is null)
            return right is null;
        return left.Equals(right!);
    }

}