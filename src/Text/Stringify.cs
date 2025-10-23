using System.Reflection;
using System.Reflection.Emit;
using BF = System.Reflection.BindingFlags;

namespace ScrubJay.Text;

[PublicAPI]
public static class Stringify
{
#if NET9_0_OR_GREATER
    private static readonly ConcurrentTypeMap<Delegate> _toStringDelegates = [];

    private static MethodInfo? FindToString(Type type, BF flags)
    {
        var toStringMethod = type
            .GetMethods(flags)
            .Where(static method => method.Name == nameof(ToString) &&
                                    method.ReturnType == typeof(string) &&
                                    method.GetParameters().Length == 0)
            .FirstOrDefault();
        return toStringMethod;
    }

    internal static Func<T, string> CreateToString<T>()
        where T : allows ref struct
    {
        Type instanceType = typeof(T);
        MethodInfo? toStringMethod;

        // Enums
        if (instanceType.IsEnum)
        {
            // Enum instances do not have a special ToString, they use the common Enum.ToString()
            toStringMethod = FindToString(typeof(Enum), BF.Public | BF.NonPublic | BF.Instance | BF.DeclaredOnly);
        }
        else
        {
            // For all other types, first look for an instance method declared exactly on that type
            toStringMethod = FindToString(instanceType, BF.Public | BF.NonPublic | BF.Instance | BF.DeclaredOnly);

            // If this is a non-value, non-ref type, we can also scan higher
            if (toStringMethod is null && (!instanceType.IsRef && !instanceType.IsValueType))
            {
                toStringMethod = FindToString(instanceType, BF.Public | BF.NonPublic | BF.Instance);
            }
        }

        if (toStringMethod is null)
        {
            Debugger.Break();
            throw Ex.Invalid("Could not find any ToString");
        }

        // We have to emit a dynamic method,
        // as Expressions cannot handle ref structs
        var dyn = new DynamicMethod(
            name: Build($"{typeof(T):@}_ToString"),
            attributes: MethodAttributes.Public | MethodAttributes.Static,
            callingConvention: CallingConventions.Standard,
            returnType: typeof(string),
            parameterTypes: [instanceType],
            m: typeof(TextHelper).Module,
            skipVisibility: true);
        var gen = dyn.GetILGenerator();

        // first, load the instance

        // stack types
        if (instanceType.IsEnum ||
            instanceType.IsByRef ||
            instanceType.IsByRefLike ||
            instanceType.IsValueType)
        {
            // load a ref to this value
            gen.Emit(OpCodes.Ldarga_S, 0);
        }
        // heap types
        else if (instanceType.IsClass || instanceType.IsInterface)
        {
            // load this class
            gen.Emit(OpCodes.Ldarg_0);
        }
        else
        {
            Debugger.Break();
            throw Ex.Unreachable();
        }

        // second, call the ToString Method

        // enums + byref likes we can use Constrained
        if (instanceType.IsByRef ||
            instanceType.IsEnum ||
            instanceType.IsByRefLike)
        {
            gen.Emit(OpCodes.Constrained, instanceType);
            gen.Emit(OpCodes.Callvirt, toStringMethod);
        }
        // value types we can just call
        else if (instanceType.IsValueType)
        {
            gen.Emit(OpCodes.Call, toStringMethod);
        }
        // class types we callvir to account for overloads
        else
        {
            gen.Emit(OpCodes.Callvirt, toStringMethod);
        }

        // return
        gen.Emit(OpCodes.Ret);

        // create the function
        var func = dyn.CreateDelegate<Func<T, string>>();
        return func;
    }


    public static string ToString<T>(T? value)
        where T : allows ref struct
    {
        if (value is null)
            return string.Empty;
        var toString = _toStringDelegates.GetOrAdd<T>(CreateToString<T>()).ThrowIfNot<Func<T, string>>();
        return toString(value);
    }
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToString<T>(T? value)
    {
        return value?.ToString() ?? string.Empty;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToString<T>(scoped ReadOnlySpan<T> span)
    {
        return span.ToString();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToString<T>(scoped Span<T> span)
    {
        return span.ToString();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToString(scoped text text)
    {
        return text.AsString();
    }
#endif
}