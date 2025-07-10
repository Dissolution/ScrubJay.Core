// ReSharper disable InvokeAsExtensionMethod

using System.Reflection;
using ScrubJay.Expressions;
using ScrubJay.Text.Rendering;

namespace ScrubJay.Comparison;

/// <summary>
/// A helper for all Equality Comparison
/// </summary>
[PublicAPI]
public static class Equate
{
    private static readonly ConcurrentTypeMap<object?> _equalityComparers = new()
    {
        [typeof(object)] = ObjectComparer.Default,
    };


    private static Result<object> TryFindEqualityComparer(Type? type)
    {
        if (type is null)
            return new ArgumentNullException(nameof(type));

        if (type.IsByRef)
        {
            var elementType = type.GetElementType();
            Debug.Assert(elementType is not null);
            return TryFindEqualityComparer(elementType);
        }

#if !NETFRAMEWORK && !NETSTANDARD2_0
        if (type.IsByRefLike)
        {
            // allows ref struct
            return TryCreateDelegateEqualityComparer(type);
        }
#endif

        var defaultEqualityComparer = typeof(EqualityComparer<>)
            .MakeGenericType(type)
            .GetMethod("get_Default", BindingFlags.NonPublic | BindingFlags.Static)
            .ThrowIfNull($"Could not find 'get_Default' method on EqualityComparer<{type.Render()}>")
            .Invoke(null, null)
            .ThrowIfNot<IEqualityComparer>();
        return Ok<object>(defaultEqualityComparer);
    }

    private static Result<IEqualityComparer<T>> TryFindEqualityComparer<T>()
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        var result = TryFindEqualityComparer(typeof(T));
        if (result.IsOkWithError(out var ok, out var error))
        {
            if (ok is IEqualityComparer<T> equalityComparer)
                return Ok(equalityComparer);
            return new InvalidOperationException("Could not get Generic Equality Comparer");
        }
        return error;
    }

    private static Result<object> TryCreateDelegateEqualityComparer(Type type)
    {
        //Debug.Assert(type.IsByRefLike);

        // Special handling for Span<T>, ReadOnlySpan<T>
        if (type.IsGenericType)
        {
            var typeDef = type.GetGenericTypeDefinition();
            Debug.Assert(typeDef is not null);
            if (typeDef == typeof(ReadOnlySpan<>))
            {
                var method = typeof(MemoryExtensions)
                    .GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .Where(method => method.Name == nameof(MemoryExtensions.SequenceEqual))
                    .Where(method =>
                    {
                        var mParams = method.GetParameters();
                        if (mParams.Length != 2)
                            return false;
                        var xType = mParams[0].ParameterType;
                        var yType = mParams[1].ParameterType;

                        var xis = xType.GetGenericTypeDefinition() == typeDef;
                        var yis = yType.GetGenericTypeDefinition() == typeDef;
                        if (!xis || !yis)
                            return false;

                        return true;
                    })
                    .OneOrDefault();
                Debug.Assert(method is not null);
                method = method!.MakeGenericMethod(type.GenericTypeArguments);

                var fn = new LambdaBuilder(typeof(Fn<,,>), type, type, typeof(bool))
                    .ParamNames("x", "y")
                    .Body(b => b.Call(method, b.Parameters))
                    .TryCompile()
                    .OkOrThrow();
                //
                //
                //
                // var xParam = Expression.Parameter(type, "x");
                // var yParam = Expression.Parameter(type, "y");
                // var call = Expression.Call(null, method, xParam, yParam);
                // var fnType = typeof(Fn<,,>).MakeGenericType(type, type, typeof(bool));
                // var lambda = Expression.Lambda(fnType, call, xParam, yParam);
                // var fn = lambda.Compile();

                var comparer =
                    typeof(DelegatedEqualityComparer<>)
                        .MakeGenericType(type)
                        .GetConstructor([fn.GetType(),])
                        .ThrowIfNull()
                        .Invoke([fn,])
                        .ThrowIfNull();
                return Ok(comparer);
            }
            else
            {
                Debugger.Break();
                throw new NotImplementedException();
            }
        }
        else
        {
            Debugger.Break();
            throw new NotImplementedException();
        }
    }


    public static IEqualityComparer GetEqualityComparer(Type type)
    {
        var comparer = _equalityComparers
            .GetOrAdd(type, static t => TryFindEqualityComparer(t).OkOr(null!))
            .Is<IEqualityComparer>();
        return comparer.SomeOrThrow();
    }

    public static IEqualityComparer<T> GetEqualityComparer<T>()
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        var comparer = _equalityComparers
            .GetOrAdd(typeof(T), static _ => TryFindEqualityComparer<T>().OkOrDefault())
            .Is<IEqualityComparer<T>>();
        return comparer.SomeOrThrow();
    }

    public static IEqualityComparer<T> GetEqualityComparerFor<T>(T? _)
        => GetEqualityComparer<T>();


    public static IEqualityComparer<T> CreateEqualityComparer<T>(Fn<T?, T?, bool> equals, Fn<T?, int> getHashCode)
        => new DelegatedEqualityComparer<T>(equals, getHashCode);


#if NET9_0_OR_GREATER
    public static bool RefValues<T>(T? left, T? right)
        where T : allows ref struct
        => GetEqualityComparer<T>().Equals(left!, right!);
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Values<T>(T? left, T? right) => EqualityComparer<T>.Default.Equals(left!, right!);


    public static bool Values<T>(T? left, T? right, IEqualityComparer<T>? comparer)
    {
        if (comparer is null)
            return Values<T>(left, right);
        return comparer.Equals(left!, right!);
    }

    public static bool EquatableValues<T>(T? left, T? right)
        where T : IEquatable<T>
    {
        if (left is null)
        {
            if (right is null)
                return true;
            return right.Equals(left!);
        }

        return left.Equals(right!);
    }

    public static bool EquatableValues<L, R>(L? left, R? right)
        where L : IEquatable<R>
    {
        if (left is null)
            return right is null;
        return left.Equals(right!);
    }


    public static bool Objects(object? left, object? right) => ObjectComparer.Default.Equals(left, right);


#region Type

    /// <summary>
    /// Are the <see cref="System.Type"/> parameters <paramref name="left"/> and <paramref name="right"/> equal?
    /// </summary>
    public static bool Types(Type? left, Type? right) => left == right;

    /// <summary>
    /// Are the generic <see cref="System.Type">Types</see> <typeparamref name="TL"/> and <typeparamref name="TR"/> equal?
    /// </summary>
    public static bool TypesOf<TL, TR>(TL? _ = default, TR? __ = default) => typeof(TL) == typeof(TR);

    /// <summary>
    /// Are the <see cref="System.Type"/>s of <paramref name="left"/> and <paramref name="right"/> equal?
    /// </summary>
    public static bool TypesOf(object? left, object? right) => left?.GetType() == right?.GetType();

#endregion
}
