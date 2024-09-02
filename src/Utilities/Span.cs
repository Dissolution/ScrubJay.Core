using ScrubJay.Validation;

namespace ScrubJay.Utilities;

public static class Span
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryCopyTo<T>(scoped ReadOnlySpan<T> source, Span<T> destination)
    {
        return source.TryCopyTo(destination);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryCopyTo<T>(scoped ReadOnlySpan<T> source, T[]? destination)
    {
        if (destination is null)
            return false;
        return source.TryCopyTo(destination);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryCopyTo<T>(T[]? source, Span<T> destination)
    {
        if (source is null)
            return false;
        return new ReadOnlySpan<T>(source).TryCopyTo(destination);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryCopyTo<T>(T[]? source, T[]? destination)
    {
        if (source is null || destination is null)
            return false;
        return new ReadOnlySpan<T>(source).TryCopyTo(destination);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryCopyTo(string? str, Span<char> destination)
    {
        if (str is null)
            return false;
        return str.AsSpan().TryCopyTo(destination);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryCopyTo(string? str, char[]? destination)
    {
        if (str is null || destination is null)
            return false;
        return str.AsSpan().TryCopyTo(destination);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SelfCopy<T>(scoped Span<T> span, Range source, Range destination)
    {
        span[source].CopyTo(span[destination]);
    }
    

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Copy<T>(scoped ReadOnlySpan<T> source, Span<T> destination)
    {
        source.CopyTo(destination);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Copy<T>(scoped ReadOnlySpan<T> source, T[]? destination)
    {
        Validate.ThrowIfNull(destination);
        source.CopyTo(destination);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Copy<T>(T[]? source, Span<T> destination)
    {
        Validate.ThrowIfNull(source);
        new ReadOnlySpan<T>(source).CopyTo(destination);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Copy<T>(T[]? source, T[]? destination)
    {
        Validate.ThrowIfNull(source);
        Validate.ThrowIfNull(destination);
        new ReadOnlySpan<T>(source).CopyTo(destination);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Copy(string? str, Span<char> destination)
    {
        Validate.ThrowIfNull(str);
        str.AsSpan().CopyTo(destination);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Copy(string? str, char[]? destination)
    {
        Validate.ThrowIfNull(str);
        Validate.ThrowIfNull(destination);
        str.AsSpan().CopyTo(destination);
    }
}