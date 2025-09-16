#if NET9_0_OR_GREATER
using System.Reflection;
using System.Reflection.Emit;
using ScrubJay.Text.Rendering;
#endif

namespace ScrubJay.Core.Utilities;

public static class Any<T>
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
{
#if NET9_0_OR_GREATER
    private static readonly Func<T, object?, bool> _equalsObject;
    private static readonly Func<T, T?, bool> _equals;
    private static readonly Func<T, int> _getHashCode;
    private static readonly Func<T, string> _toString;
    private static readonly Func<T, string?, IFormatProvider?, string> _format;
    private static readonly Func<T, Type> _getType;

    static Any()
    {
        var type = typeof(T);
        var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (type.IsByRef || type.IsByRefLike)
        {
            methods = methods.Where(m => m.DeclaringType == type).ToArray();
        }

        // Equals(object) -> bool
        // Equals(T) -> bool
        var equalsMethods = methods
            .Where(static method => method.Name == nameof(Equals) && method.ReturnType == typeof(bool))
            .ToList();

        var equalsObjectMethod = equalsMethods.FirstOrDefault(method =>
        {
            var mp = method.GetParameters();
            return mp.Length == 1 && mp[0].ParameterType == typeof(object);
        });

        if (equalsObjectMethod is not null)
        {
            _equalsObject = EmitDelegate<Func<T, object?, bool>>(
                $"{type.Render()}_Equals_Object",
                gen => EmitLoadCall(gen, equalsObjectMethod, 1));
        }
        else
        {
            _equalsObject = static (_, _) => false;
        }

        var equalsMethod = equalsMethods.FirstOrDefault(method =>
        {
            var mp = method.GetParameters();
            return mp.Length == 1 && mp[0].ParameterType == type;
        });

        if (equalsMethod is not null)
        {
            _equals = EmitDelegate<Func<T, T?, bool>>(
                $"{type.Render()}_Equals",
                gen => EmitLoadCall(gen, equalsMethod, 1));
        }
        else
        {
            _equals = static (_, _) => false;
        }

        // GetHashCode
        var getHashCodeMethod = methods
            .Where(static method => method.Name == nameof(GetHashCode) && method.ReturnType == typeof(int))
            .FirstOrDefault();

        if (getHashCodeMethod is not null)
        {
            _getHashCode = EmitDelegate<Func<T, int>>(
                $"{type.Render()}_GetHashCode",
                gen => EmitLoadCall(gen, getHashCodeMethod, 0));
        }
        else
        {
            _getHashCode = static _ => 0;
        }

        // ToString + ToString(format, provider)
        var toStringMethods = methods
            .Where(static method => method.Name == nameof(ToString) && method.ReturnType == typeof(string))
            .ToList();

        // ToString()
        var toStringMethod = toStringMethods.FirstOrDefault(static method => method.GetParameters().Length == 0);

        if (toStringMethod is not null)
        {
            _toString = EmitDelegate<Func<T, string>>(
                $"{type.Render()}_ToString",
                gen => EmitLoadCall(gen, toStringMethod, 0));
        }
        else
        {
            _toString = FallbackToString;
        }

        // ToString(string? format, IFormatProvider? provider)
        var formatMethod = toStringMethods.FirstOrDefault(static method =>
        {
            var mp = method.GetParameters();
            if (mp.Length != 2) return false;
            if (mp[0].ParameterType != typeof(string))
                return false;
            if (mp[1].ParameterType != typeof(IFormatProvider))
                return false;
            return true;
        });

        if (formatMethod is not null)
        {
            _format = EmitDelegate<Func<T, string?, IFormatProvider?, string>>(
                $"{type.Render()}_ToString_Format_Provider",
                gen => EmitLoadCall(gen, formatMethod, 2));
        }
        else
        {
            _format = (value, _, _) => _toString(value);
        }

        // GetType
        var getTypeMethod =
            methods.FirstOrDefault(static method => method.Name == nameof(GetType) && method.ReturnType == typeof(Type));

        if (getTypeMethod is not null)
        {
            _getType = EmitDelegate<Func<T, Type>>(
                $"{type.Render()}_GetType",
                gen => EmitLoadCall(gen, getTypeMethod, 0));
        }
        else
        {
            _getType = static _ => typeof(T);
        }
    }

    private static D EmitDelegate<D>(string name, Action<ILGenerator> emissions)
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

    private static void EmitLoadCall(ILGenerator generator, MethodInfo method, int args)
    {
        var type = typeof(T);

        // load value types as a ref
        if (type.IsValueType || type.IsByRef || type.IsByRefLike)
        {
            generator.Emit(OpCodes.Ldarga, 0);
        }
        else
        {
            generator.Emit(OpCodes.Ldarg_0);
        }

        var mp = method.GetParameters();
        var mpc = mp.Length;
        var mps = method.IsStatic;

        if (args != mpc)
            Debugger.Break();

        // load any additional args
        for (int a = 1; a <= args; a++)
        {
            generator.Emit(OpCodes.Ldarg, a);
        }

        // special call handling for special types
        if (type.IsByRef || type.IsByRefLike)
        {
            generator.Emit(OpCodes.Constrained, type);
        }
        else if (type.IsValueType)
        {
            generator.Emit(OpCodes.Constrained, type);
        }
        else if (type.IsClass || type.IsInterface)
        {
            // generator.Emit(OpCodes.Ldind_Ref);
        }
        else
        {
            Debugger.Break();
            throw new NotImplementedException();
        }

        // call the method
        generator.Emit(OpCodes.Callvirt, method);
        // return
        generator.Emit(OpCodes.Ret);
    }

    private static string FallbackToString(T value)
    {
        var type = typeof(T);
        if (type.IsValueType || type.IsByRef || type.IsByRefLike)
        {
            var sz = Notsafe.SizeOf<T>();
            Span<byte> buffer = stackalloc byte[sz];
            Notsafe.Bytes.Write(value, buffer);
            string str =  Convert.ToHexString(buffer);
            Debugger.Break();
            return str;
        }
        else
        {
            unsafe
            {
                IntPtr ptr = new(Notsafe.InAsVoidPtr(in value));
                string str = ptr.ToString();
                Debugger.Break();
                return str;
            }
        }
    }
#endif


    public static bool Equals(T? value, object? obj)
    {
        if (value is null)
            return obj is null;
#if !NET9_0_OR_GREATER
        return value.Equals(obj);
#else
        return _equalsObject(value, obj);
#endif
    }

    public static bool Equals(T? value, T? other)
    {
        if (value is null)
            return other is null;
#if !NET9_0_OR_GREATER
        if (value is IEquatable<T>)
        {
            return ((IEquatable<T>)value).Equals(other!);
        }
        else
        {
            return value.Equals(other);
        }
#else
        return _equals(value, other);
#endif
    }

    public static int GetHashCode(T? value)
    {
        if (value is null)
            return Hasher.NullHash;

#if !NET9_0_OR_GREATER
        return value.GetHashCode();
#else
        return _getHashCode(value);
#endif
    }

    public static string ToString(T? value)
    {
        if (value is null)
            return string.Empty;

#if !NET9_0_OR_GREATER
        return value.ToString() ?? string.Empty;
#else
        return _toString(value);
#endif
    }

    public static string ToString(T? value, string? format, IFormatProvider? provider = default)
    {
        if (value is null)
            return string.Empty;

#if !NET9_0_OR_GREATER
        return value.ToString() ?? string.Empty;
#else
        return _format(value, format, provider);
#endif
    }

    [return: NotNullIfNotNull(nameof(value))]
    public static Type? GetType(T? value)
    {
        if (value is null)
            return null;

#if !NET9_0_OR_GREATER
        return value.GetType();
#else
        return _getType(value);
#endif
    }
}