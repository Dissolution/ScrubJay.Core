using System.Reflection;
using ScrubJay.Text.Rendering;

namespace ScrubJay.Comparison;

/// <summary>
/// A helper utility for equality comparison
/// </summary>
[PublicAPI]
public static class Equate
{
    /// <summary>
    /// A cache of T:IEqualityComparer&lt;T&gt; instances
    /// </summary>
    private static readonly ConcurrentTypeMap<object> _equalityComparers = new()
    {
        [typeof(object)] = ObjectComparer.Default,
    };

    private static object GetEqualityComparerInstance(Type type)
    {
        if (type.IsByRef
#if NETSTANDARD2_1 || NET8_0_OR_GREATER
            || type.IsByRefLike)
#else
           )
#endif
        {
            throw new NotImplementedException();
        }

        // Return EqualityComparer<T>.Default
        return typeof(EqualityComparer<>)
            .MakeGenericType(type)
            .GetMethod("get_Default", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase)
            .ThrowIfNull($"Could not find EqualityComparer<{type.Render()}>.get_Default method")
            .Invoke(null, null)
            .ThrowIfNot(typeof(IEqualityComparer<>).MakeGenericType(type));
    }

    public static IEqualityComparer<T> GetComparer<T>()
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        return _equalityComparers.GetOrAdd<T>(GetEqualityComparerInstance)
            .ThrowIfNot<IEqualityComparer<T>>();
    }

    public static IEqualityComparer GetComparer(Type type)
    {
        Throw.IfNull(type);
        return _equalityComparers.GetOrAdd(type, GetEqualityComparerInstance)
            .ThrowIfNot<IEqualityComparer>();
    }


    // private static Result<object> TryCreateDelegateEqualityComparer(Type type)
    // {
    //     //Debug.Assert(type.IsByRefLike);
    //
    //     // Special handling for Span<T>, ReadOnlySpan<T>
    //     if (type.IsGenericType)
    //     {
    //         var typeDef = type.GetGenericTypeDefinition();
    //         Debug.Assert(typeDef is not null);
    //         if (typeDef == typeof(ReadOnlySpan<>))
    //         {
    //             var method = typeof(MemoryExtensions)
    //                 .GetMethods(BindingFlags.Public | BindingFlags.Static)
    //                 .Where(method => method.Name == nameof(MemoryExtensions.SequenceEqual))
    //                 .Where(method =>
    //                 {
    //                     var mParams = method.GetParameters();
    //                     if (mParams.Length != 2)
    //                         return false;
    //                     var xType = mParams[0].ParameterType;
    //                     var yType = mParams[1].ParameterType;
    //
    //                     var xis = xType.GetGenericTypeDefinition() == typeDef;
    //                     var yis = yType.GetGenericTypeDefinition() == typeDef;
    //                     if (!xis || !yis)
    //                         return false;
    //
    //                     return true;
    //                 })
    //                 .OneOrDefault();
    //             Debug.Assert(method is not null);
    //             method = method!.MakeGenericMethod(type.GenericTypeArguments);
    //
    //             var fn = new LambdaBuilder(typeof(Fn<,,>), type, type, typeof(bool))
    //                 .ParamNames("x", "y")
    //                 .Body(b => b.Call(method, b.Parameters))
    //                 .TryCompile()
    //                 .OkOrThrow();
    //             //
    //             //
    //             //
    //             // var xParam = Expression.Parameter(type, "x");
    //             // var yParam = Expression.Parameter(type, "y");
    //             // var call = Expression.Call(null, method, xParam, yParam);
    //             // var fnType = typeof(Fn<,,>).MakeGenericType(type, type, typeof(bool));
    //             // var lambda = Expression.Lambda(fnType, call, xParam, yParam);
    //             // var fn = lambda.Compile();
    //
    //             var comparer =
    //                 typeof(FuncEqualityComparer<>)
    //                     .MakeGenericType(type)
    //                     .GetConstructor([fn.GetType(),])
    //                     .ThrowIfNull()
    //                     .Invoke([fn,])
    //                     .ThrowIfNull();
    //             return Ok(comparer);
    //         }
    //         else
    //         {
    //             Debugger.Break();
    //             throw new NotImplementedException();
    //         }
    //     }
    //     else
    //     {
    //         Debugger.Break();
    //         throw new NotImplementedException();
    //     }


    /// <summary>
    /// Creates an <see cref="IEqualityComparer{T}"/> that uses functions to determine equality
    /// </summary>
    /// <param name="equals">
    /// The <c>Func&lt;T?, T?, bool&gt;</c> that determines if two <typeparamref name="T"/> instances are equal
    /// </param>
    /// <param name="getHashCode">
    /// The <c>Func&lt;T?, int&gt;</c> that determines a <typeparamref name="T"/> instance's hash code
    /// </param>
    public static IEqualityComparer<T> CreateComparer<T>(Fn<T?, T?, bool> equals, Fn<T?, int> getHashCode)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
        => new FuncEqualityComparer<T>(equals, getHashCode);


    public static bool Values<T>(T? left, T? right)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
        => GetComparer<T>().Equals(left!, right!);

    public static bool Values<T>(T? left, T? right, IEqualityComparer<T>? comparer)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        if (comparer is null)
            return Values<T>(left, right);
        return comparer.Equals(left!, right!);
    }

    public static bool Objects(object? left, object? right)
        => ObjectComparer.Default.Equate(left, right);
}