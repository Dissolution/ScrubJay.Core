#pragma warning disable IDE0058

using ScrubJay.Text;

namespace ScrubJay.Utilities;

/// <summary>
/// A helper for rendering <see cref="Type"/> names
/// </summary>
[PublicAPI]
public static class TypeNames
{
    // https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/
    private static readonly ConcurrentTypeMap<string> _typeNameCache = new()
    {
        // C# type aliases
        [typeof(bool)] = "bool",
        [typeof(char)] = "char",
        [typeof(sbyte)] = "sbyte",
        [typeof(byte)] = "byte",
        [typeof(short)] = "short",
        [typeof(ushort)] = "ushort",
        [typeof(int)] = "int",
        [typeof(uint)] = "uint",
        [typeof(long)] = "long",
        [typeof(ulong)] = "ulong",
        [typeof(float)] = "float",
        [typeof(double)] = "double",
        [typeof(decimal)] = "decimal",
        [typeof(string)] = "string",
        [typeof(object)] = "object",
        [typeof(void)] = "void",
        [typeof(nint)] = "nint",
        [typeof(nuint)] = "nuint",
    };

    internal static void WriteTypeName(
        this ref Buffer<char> name,
        Type? type)
    {
        if (type is null)
        {
            name.Write("null");
            return;
        }

        if (_typeNameCache.TryGetValue(type, out string? n))
        {
            name.Write(n);
            return;
        }

        // Enum types are their NameFrom
        if (type.IsEnum)
        {
            name.Write(type.Name);
            return;
        }

        // Nullable<T> => T?
        var underType = Nullable.GetUnderlyingType(type);
        if (underType is not null)
        {
            // c# Nullable alias
            name.WriteTypeName(underType);
            name.Write("?");
            return;
        }

        // Pointer -> uType*
        if (type.IsPointer)
        {
            underType = type.GetElementType()!;
            name.WriteTypeName(underType);
            name.Write("*");
            return;
        }

        // ByRef => ref type
        if (type.IsByRef)
        {
            underType = type.GetElementType()!;
            name.Write("ref ");
            name.WriteTypeName(underType);
            return;
        }

        // Array => T[]
        if (type.IsArray)
        {
            underType = type.GetElementType()!;
            name.WriteTypeName(underType);
            name.Write("[]");
            return;
        }

        // Nested Type?
        if (type is { IsNested: true, IsGenericParameter: false })
        {
            name.WriteTypeName(type.DeclaringType);
            name.Write(".");
        }

        // If non-generic
        if (!type.IsGenericType)
        {
            // Just write the type name and we're done
            name.Write(type.Name);
        }
        else
        {
            // Start processing type name
            var typeName = type.Name.AsSpan();

            /* The default NameFrom for a generic type is:
             * Thing<>   = Thing`1
             * Thing<,>  = Thing`2
             * Thing<,,> = Thing`3
             * ...
             */
            int i = typeName.IndexOf('`');
            if (i >= 0)
            {
                name.Write(typeName.Slice(0, i));
            }
            else
            {
                // Odd... use the name
                name.Write(typeName);
            }

            // Add our generic types to finish
            var argTypes = type.GetGenericArguments();
            int argCount = argTypes.Length;
            Debug.Assert(argCount > 0);

            name.Write("<");
            name.WriteTypeName(argTypes[0]);
            for (i = 1; i < argCount; i++)
            {
                name.Write(", ");
                name.WriteTypeName(argTypes[i]);
            }

            name.Write(">");
        }
    }

    private static string CreateTypeName(Type? type)
    {
        var text = new Buffer<char>();
        WriteTypeName(ref text, type);
        return text.ToStringAndDispose();
    }

    /// <summary>
    /// Gets the rendered name of this <see cref="Type"/>
    /// </summary>
    public static string NameOf(this Type? type)
    {
        if (type is null)
            return "null";
        return _typeNameCache.GetOrAdd(type, static t => CreateTypeName(t));
    }

    /// <summary>
    /// Gets the rendered name of a generic type
    /// </summary>
    public static string NameOf<T>() => _typeNameCache.GetOrAdd<T>(static t => CreateTypeName(t));
}
