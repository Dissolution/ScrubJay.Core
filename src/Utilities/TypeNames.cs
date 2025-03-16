#pragma warning disable IDE0058

using ScrubJay.Collections;

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

    internal static void WriteTypeName(this ref TextBuffer buffer, Type? type)
    {
        if (type is null)
        {
            buffer.Write("null");
            return;
        }

        if (_typeNameCache.TryGetValue(type, out var name))
        {
            buffer.Write(name);
            return;
        }

        // Enum types are their Name
        if (type.IsEnum)
        {
            buffer.Write(type.Name);
            return;
        }

        // Nullable<T> => T?
        var underType = Nullable.GetUnderlyingType(type);
        if (underType is not null)
        {
            // c# Nullable alias
            buffer.WriteTypeName(underType);
            buffer.Write("?");
            return;
        }

        // Pointer -> uType*
        if (type.IsPointer)
        {
            underType = type.GetElementType()!;
            buffer.WriteTypeName(underType);
            buffer.Write("*");
            return;
        }

        // ByRef => ref type
        if (type.IsByRef)
        {
            underType = type.GetElementType()!;
            buffer.Write("ref ");
            buffer.WriteTypeName(underType);
            return;
        }

#if !NETFRAMEWORK && !NETSTANDARD2_0
        // allows ref struct, ???
        if (type.IsByRefLike)
        {
            Debugger.Break();
            // continue
        }
#endif

        // Array => T[]
        if (type.IsArray)
        {
            underType = type.GetElementType()!;
            buffer.WriteTypeName(underType);
            buffer.Write("[]");
            return;
        }

        // Nested Type?
        if (type is { IsNested: true, IsGenericParameter: false })
        {
            buffer.WriteTypeName(type.DeclaringType);
            buffer.Write(".");
        }

        // If non-generic
        if (!type.IsGenericType)
        {
            // Just write the type name and we're done
            buffer.Write(type.Name);
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
                buffer.Write(typeName.Slice(0, i));
            }
            else
            {
                Debugger.Break();
                // Odd... use the name
                buffer.Write(typeName);
            }

            // Add our generic types to finish
            var argTypes = type.GetGenericArguments();
            int argCount = argTypes.Length;
            Debug.Assert(argCount > 0);

            buffer.Write("<");
            buffer.WriteTypeName(argTypes[0]);
            for (i = 1; i < argCount; i++)
            {
                buffer.Write(", ");
                buffer.WriteTypeName(argTypes[i]);
            }

            buffer.Write(">");
        }
    }

    private static string CreateTypeName(Type? type)
    {
        var text = new TextBuffer();
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
    public static string NameOf<T>()
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
        => _typeNameCache.GetOrAdd<T>(static t => CreateTypeName(t));
}
