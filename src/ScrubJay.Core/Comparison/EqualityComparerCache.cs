﻿// ReSharper disable InvokeAsExtensionMethod

#if !NETCOREAPP3_1_OR_GREATER
#pragma warning disable CS8604
#endif

using System.Reflection;
using ScrubJay.Collections;

namespace ScrubJay.Comparison;

public sealed class EqualityComparerCache : IEqualityComparer<object?>, IEqualityComparer
{
    private static readonly ConcurrentTypeMap<object> _equalityComparers = new();

    private static object FindEqualityComparer(Type type)
    {
        return typeof(EqualityComparer<>)
            .MakeGenericType(type)
            .GetProperty("Default", BindingFlags.Public | BindingFlags.Static)
            .ThrowIfNull()
            .GetValue(null)
            .ThrowIfNull();
    }

    public static IEqualityComparer Default(Type type)
    {
        return _equalityComparers
            .GetOrAdd(type, static t => FindEqualityComparer(t))
            .AsValid<IEqualityComparer>();
    }

    public static IEqualityComparer<T> Default<T>()
    {
        return _equalityComparers
            .GetOrAdd<T>(static _ => EqualityComparer<T>.Default)
            .AsValid<IEqualityComparer<T>>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals<T>(T? left, T? right)
    {
        return Default<T>().Equals(left, right);
    }

    public new static bool Equals(object? left, object? right)
    {
        if (ReferenceEquals(left, right)) 
            return true;
        if (left is not null)
        {
            return Default(left.GetType())
                .Equals(left, right);
        }
        return Default(right!.GetType())
            .Equals(right, left);
    }

    public static int GetHashCode(object? obj)
    {
        if (obj is null) return 0;
        return Default(obj.GetType()).GetHashCode(obj);
    }

    public static IEqualityComparer<T> Create<T>(Func<T?, T?, bool> equals, Func<T?, int> getHashCode)
        => new FuncEqualityComparer<T>(equals, getHashCode);

    public static IEqualityComparer<T> CreateKeyComparer<T>(Func<T, T, bool> equals, Func<T, int> getHashCode)
        => new FuncEqualityComparer<T>(equals!, getHashCode!);
    
    bool IEqualityComparer<object?>.Equals(object? x, object? y) => Equals(x, y);
    bool IEqualityComparer.Equals(object? x, object? y) => Equals(x, y);

    int IEqualityComparer<object?>.GetHashCode(object? obj) => GetHashCode(obj);
    int IEqualityComparer.GetHashCode(object? obj) => GetHashCode(obj);
}