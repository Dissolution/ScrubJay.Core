using System.Reflection;
using System.Reflection.Emit;

namespace ScrubJay.Utilities;

public static class Any
{
#if NET9_0_OR_GREATER
    internal static D EmitDelegate<D>(string name, Action<ILGenerator> emissions)
        where D : Delegate
    {
        // the invoke method of the delegate contains the 'true' signature of the delegate
        var invokeMethod = typeof(D)
            .GetMethod("Invoke", BindingFlags.Public | BindingFlags.Instance)
            .ThrowIfNull();

        DynamicMethod method = new DynamicMethod(name,
            MethodAttributes.Public | MethodAttributes.Static,
            CallingConventions.Standard,
            invokeMethod.ReturnType,
            invokeMethod.GetParameters().ConvertAll(static p => p.ParameterType),
            typeof(Any<>).Module,
            true);

        emissions(method.GetILGenerator());
        return method.CreateDelegate(typeof(D)).ThrowIfNot<D>();
    }
#endif

    public static bool Equals<T>(T? value, object? obj)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
        => Any<T>.Equals(value, obj);

    public static bool Equals<T>(T? value, T? other)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
        => Any<T>.Equals(value, other);

    public static bool Equals<T>(scoped ReadOnlySpan<T> left, ReadOnlySpan<T> right)
    {
        int len = left.Length;
        if (right.Length != len)
            return false;
        for (var i = 0; i < len; i++)
        {
            if (!Any<T>.Equals(left[i], right[i]))
                return false;
        }

        return true;
    }

    public static int GetHashCode<T>(T? value)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
        => Any<T>.GetHashCode(value);

    public static int GetHashCode<T>(scoped ReadOnlySpan<T> span)
    {
        return Hasher.HashMany<T>(span);
    }

    public static string ToString<T>(T? value)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
        => Any<T>.ToString(value);

    public static string ToString<T>(scoped ReadOnlySpan<T> span)
    {
        return TextBuilder
            .New
            .Append($"ReadOnlySpan<{typeof(T):@}>(")
            .Delimit<T>(", ", span, static (tb, item) => tb.Write(Any<T>.ToString(item)))
            .Append(')')
            .ToStringAndDispose();
    }

    public static string ToString<T>(scoped Span<T> span)
    {
        return TextBuilder
            .New
            .Append($"Span<{typeof(T):@}>(")
            .Delimit(", ", span, static (tb, item) => tb.Write(Any<T>.ToString(item)))
            .Append(')')
            .ToStringAndDispose();
    }

    public static string Format<T>(T? value, string? format = null, IFormatProvider? provider = null)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
        => Any<T>.ToString(value, format, provider);

    [return: NotNullIfNotNull(nameof(value))]
    public static Type? GetType<T>(T? value)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
        => Any<T>.GetType(value);
}
