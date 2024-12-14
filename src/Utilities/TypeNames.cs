#pragma warning disable IDE0058

using ScrubJay.Collections;
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

    private static void AppendTypeName(
        this ref InterpolatedText name,
        Type? type)
    {
        if (type is null)
        {
            name.AppendLiteral("null");
            return;
        }

        if (_typeNameCache.TryGetValue(type, out string? n))
        {
            name.AppendLiteral(n);
            return;
        }

        // Enum types are their NameFrom
        if (type.IsEnum)
        {
            name.AppendLiteral(type.Name);
            return;
        }

        // Nullable<T> => T?
        var underType = Nullable.GetUnderlyingType(type);
        if (underType is not null)
        {
            // c# Nullable alias
            name.AppendTypeName(underType);
            name.AppendLiteral("?");
            return;
        }

        // Pointer -> uType*
        if (type.IsPointer)
        {
            underType = type.GetElementType()!;
            name.AppendTypeName(underType);
            name.AppendLiteral("*");
            return;
        }

        // ByRef => ref type
        if (type.IsByRef)
        {
            underType = type.GetElementType()!;
            name.AppendLiteral("ref ");
            name.AppendTypeName(underType);
            return;
        }

        // Array => T[]
        if (type.IsArray)
        {
            underType = type.GetElementType()!;
            name.AppendTypeName(underType);
            name.AppendLiteral("[]");
            return;
        }

        // Nested Type?
        if (type is { IsNested: true, IsGenericParameter: false })
        {
            name.AppendTypeName(type.DeclaringType);
            name.AppendLiteral(".");
        }

        // If non-generic
        if (!type.IsGenericType)
        {
            // Just write the type name and we're done
            name.AppendLiteral(type.Name);
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
                name.AppendFormatted(typeName.Slice(0, i));
            }
            else
            {
                // Odd... use the name
                name.AppendFormatted(typeName);
            }

            // Add our generic types to finish
            var argTypes = type.GetGenericArguments();
            int argCount = argTypes.Length;
            Debug.Assert(argCount > 0);

            name.AppendLiteral("<");
            name.AppendTypeName(argTypes[0]);
            for (i = 1; i < argCount; i++)
            {
                name.AppendLiteral(", ");
                name.AppendTypeName(argTypes[i]);
            }

            name.AppendLiteral(">");
        }
    }

    private static string CreateTypeName(Type? type)
    {
        var text = new InterpolatedText(32, 1);
        AppendTypeName(ref text, type);
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
