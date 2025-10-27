#if NET9_0_OR_GREATER
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using ScrubJay.Text.Rendering;
using BF = System.Reflection.BindingFlags;
#endif

namespace ScrubJay.Text;

public static partial class TextHelper
{
#if NET9_0_OR_GREATER

    // use a static class to contain the delegates!
    private static class Cache<T>
        where T : allows ref struct
    {
        internal static readonly Func<T, string> _toString = CreateToString();
        internal static readonly Func<T, string?, IFormatProvider?, string> _format = CreateFormat();

        private static MethodInfo? FindToString(Type type, BF flags)
        {
            var toStringMethod = type
                .GetMethods(flags)
                .Where(static method => method.Name == nameof(object.ToString) &&
                                        method.ReturnType == typeof(string) &&
                                        method.GetParameters().Length == 0)
                .FirstOrDefault();
            return toStringMethod;
        }

        private static MethodInfo? FindFormat(Type type, BF flags)
        {
            var formatMethod = type
                .GetMethods(flags)
                .Where(static method => method.Name == nameof(IFormattable.ToString) && method.ReturnType == typeof(string))
                .Where(static method =>
                {
                    var ps = method.GetParameters();
                    if (ps.Length != 2)
                        return false;
                    return ps[0].ParameterType == typeof(string) &&
                           ps[1].ParameterType == typeof(IFormatProvider);
                })
                .FirstOrDefault();
            return formatMethod;
        }

        private static Func<T, string> CreateToString()
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
                // fallback to describing the type
                return static _ => typeof(T).Render();
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

        private static Func<T, string?, IFormatProvider?, string> CreateFormat()
        {
            Type instanceType = typeof(T);
            MethodInfo? formatMethod;

            // Enums
            if (instanceType.IsEnum)
            {
                // Enum instances do not have a special ToString, they use the common Enum.ToString()
                formatMethod = FindFormat(typeof(Enum), BF.Public | BF.NonPublic | BF.Instance | BF.DeclaredOnly);
            }
            else
            {
                // For all other types, first look for an instance method declared exactly on that type
                formatMethod = FindFormat(instanceType, BF.Public | BF.NonPublic | BF.Instance | BF.DeclaredOnly);

                // If this is a non-value, non-ref type, we can also scan higher
                if (formatMethod is null && (!instanceType.IsRef && !instanceType.IsValueType))
                {
                    formatMethod = FindFormat(instanceType, BF.Public | BF.NonPublic | BF.Instance);
                }
            }

            if (formatMethod is null)
            {
                // fallback to describing the type
                return static (_, format, provider) =>
                {
                    if (provider is not null && provider.GetFormat(typeof(ICustomFormatter)) is ICustomFormatter formatter)
                    {
                        return formatter.Format(format, typeof(T), provider);
                    }

                    if (format == "@")
                    {
                        return typeof(T).Render();
                    }

                    return typeof(T).ToString();
                };
            }

            // We have to emit a dynamic method,
            // as Expressions cannot handle ref structs
            var dyn = new DynamicMethod(
                name: Build($"{typeof(T):@}_Format"),
                attributes: MethodAttributes.Public | MethodAttributes.Static,
                callingConvention: CallingConventions.Standard,
                returnType: typeof(string),
                parameterTypes: [instanceType, typeof(string), typeof(IFormatProvider)],
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

            // second, load the other args
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Ldarg_2);

            // finally, call the ToString Method

            // enums + byref likes we can use Constrained
            if (instanceType.IsByRef ||
                instanceType.IsEnum ||
                instanceType.IsByRefLike)
            {
                gen.Emit(OpCodes.Constrained, instanceType);
                gen.Emit(OpCodes.Callvirt, formatMethod);
            }
            // value types we can just call
            else if (instanceType.IsValueType)
            {
                gen.Emit(OpCodes.Call, formatMethod);
            }
            // class types we callvir to account for overloads
            else
            {
                gen.Emit(OpCodes.Callvirt, formatMethod);
            }

            // return
            gen.Emit(OpCodes.Ret);

            // create the function
            var func = dyn.CreateDelegate<Func<T, string?, IFormatProvider?, string>>();
            return func;
        }
    }

    public static string Stringify<T>(this T? value)
        where T : allows ref struct
    {
        if (value is null)
            return string.Empty;
        return Cache<T>._toString(value);
    }

    public static string Stringify<T>(this T? value, string? format, IFormatProvider? provider = null)
        where T : allows ref struct
    {
        if (value is null)
            return string.Empty;
        return Cache<T>._format(value, format, provider);
    }
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Stringify<T>(this T? value)
    {
        return value?.ToString() ?? string.Empty;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Stringify<T>(this T? value, string? format, IFormatProvider? provider = null)
    {
        if (value is null)
        {
            return string.Empty;
        }

        if (value is IFormattable)
        {
            return ((IFormattable)value).ToString(format, provider);
        }

        return value.ToString() ?? string.Empty;
    }

#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Stringify<T>(this scoped ReadOnlySpan<T> span)
    {
        return span.ToString();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Stringify<T>(this scoped Span<T> span)
    {
        return span.ToString();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Stringify(this scoped text text)
    {
        return text.AsString();
    }
}