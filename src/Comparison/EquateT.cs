using System.Reflection;
using ScrubJay.Collections;
#if !NETFRAMEWORK && !NETSTANDARD2_0
using System.Linq.Expressions;
using ScrubJay.Functional.Linq;
#endif

namespace ScrubJay.Comparison;

public partial class Equate2
{
    private static readonly ConcurrentTypeMap<object?> _equalityComparers = new()
    {
        [typeof(object)] = ObjectComparer.Default,
    };

#if !NETFRAMEWORK && !NETSTANDARD2_0
    private static Result<object, Exception> TryCreateARSEqualityComparer(Type type)
    {
        Debug.Assert(type.IsByRefLike);

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

                var xParam = Expression.Parameter(type, "x");
                var yParam = Expression.Parameter(type, "y");
                var call = Expression.Call(null, method, xParam, yParam);
                var fnType = typeof(Fn<,,>).MakeGenericType([type, type, typeof(bool)]);
                var lambda = Expression.Lambda(fnType, call, xParam, yParam);
                var fn = lambda.Compile();

                var comparer =
                    typeof(DelegatedEqualityComparer<>)
                        .MakeGenericType(type)
                        .GetConstructor([fnType])
                        .ThrowIfNull()
                        .Invoke([fn])
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
#endif

    protected static Result<object, Exception> TryCreateEqualityComparer(Type? type)
    {
        if (type is null)
            return new ArgumentNullException(nameof(type));

        if (type.IsByRef)
        {
            var elementType = type.GetElementType();
            Debug.Assert(elementType is not null);
            return TryCreateEqualityComparer(elementType);
        }

#if !NETFRAMEWORK && !NETSTANDARD2_0
        if (type.IsByRefLike)
        {
            // allows ref struct
            return TryCreateARSEqualityComparer(type);
        }
#endif

        var defaultEqualityComparer = typeof(EqualityComparer<>)
            .MakeGenericType(type)
            .GetMethod("get_Default", BindingFlags.NonPublic | BindingFlags.Static)
            .ThrowIfNull($"Could not find 'get_Default' method on EqualityComparer<{type.NameOf()}>")
            .Invoke(null, null)
            .ThrowIfNot<IEqualityComparer>();
        return Ok<object>(defaultEqualityComparer);
    }

    protected static Result<IEqualityComparer<T>, Exception> TryCreateEqualityComparer<T>()
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        var result = TryCreateEqualityComparer(typeof(T));
        if (result.HasOkOrError(out var ok, out var error))
        {
            if (ok is IEqualityComparer<T> equalityComparer)
                return Ok(equalityComparer);
            return new InvalidOperationException("Could not get Generic Equality Comparer");
        }
        return error;
    }

    public static IEqualityComparer GetEqualityComparer(Type type)
    {
        var comparer = _equalityComparers
            .GetOrAdd(type, static t => TryCreateEqualityComparer(t).OkOr(null!))
            .As<IEqualityComparer>();
        return comparer.SomeOrThrow();
    }

    public static IEqualityComparer<T> GetEqualityComparer<T>()
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        var comparer = _equalityComparers
            .GetOrAdd(typeof(T), static _ => TryCreateEqualityComparer<T>().OkOr(null!))
            .As<IEqualityComparer<T>>();
        return comparer.SomeOrThrow();
    }
}

public class Equate<T> : Equate2
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
{
    public static bool Values([AllowNull] T x, [AllowNull] T y)
    {
        return GetEqualityComparer<T>().Equals(x!, y!);
    }
}
