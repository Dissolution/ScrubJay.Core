﻿#pragma warning disable CA1034, S2326, CA1711

// ReSharper disable UnusedTypeParameter
namespace ScrubJay.Utilities;

/// <summary>
/// Constraints are added to generic methods so that generic type constraints can co-exist without compiler error<br />
/// <br/>
/// Usually, if you had:<br />
/// <c>public T DoThing&lt;T&gt;(T value) where T : struct;</c><br />
/// <c>public T DoThing&lt;T&gt;(T value) where T : class;</c><br />
/// <br/>
/// The compiler will have an error: 'member with the same signature is already declared'<br />
/// <br />
/// You can use <see cref="GenericTypeConstraint"/> to fix it:<br />
/// <c>public static T DoThing&lt;T&gt;(T value, IsStruct&lt;T&gt; _ = default) where T : struct</c><br />
/// <c>public static T DoThing&lt;T&gt;(T value, IsClass&lt;T&gt; _ = default) where T : class</c><br />
/// <br/>
/// </summary>
[PublicAPI]
public static class GenericTypeConstraint
{
    public readonly struct Any<T>;


    /// <summary>
    /// Constrains <typeparamref name="T"/> to only non-<c>static</c> types that have a default constructor
    /// </summary>
    public readonly struct IsNew<T> where T : new();

    public readonly struct IsDisposable<T> where T : IDisposable;

    public readonly struct IsClass<T> where T : class;

    public readonly struct IsStruct<T> where T : struct;

    public readonly struct IsUnmanaged<T> where T : unmanaged;

    public readonly struct IsEquatable<T> where T : IEquatable<T>;

    /// <summary>
    /// Constrains <typeparamref name="T"/> to only <see cref="Type">Types</see> that implement <see cref="IComparable{T}"/>
    /// </summary>
    /// <typeparam name="T">
    /// <c>T : IComparable&lt;T&gt;</c>
    /// </typeparam>
    public readonly struct IsComparable<T> where T : IComparable<T>;

    public readonly struct IsDisposableNew<T> where T : IDisposable, new();

#if NET7_0_OR_GREATER
    public readonly struct IsSpanParsable<T> where T : ISpanParsable<T>;

    public readonly struct IsNumberBase<T> where T : INumberBase<T>;
#endif

    public readonly struct AllowsRefStruct<T>
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
        ;
}
