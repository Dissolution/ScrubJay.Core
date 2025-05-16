#pragma warning disable IDE0058, CA2263, IDE0060

namespace ScrubJay.Utilities;

/// <summary>
/// A helper for rendering <see cref="Type"/> names
/// </summary>
[PublicAPI]
public static class CSharpTypeNaming
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

    public static string NameOf(this Type? type)
    {
        if (type is null) return string.Empty;

        if (_typeNameCache.TryGetValue(type, out var name))
            return name;

        // Enum types are their Name
        if (type.IsEnum)
        {
            return type.Name;
        }

        using var text = TextBuilder.New;

        // Nullable<T> => T?
        var underType = Nullable.GetUnderlyingType(type);
        if (underType is not null)
        {
            // c# Nullable alias
            return text
                .Append(NameOf(underType))
                .Append('?')
                .ToString();
        }

        // Pointer -> uType*
        if (type.IsPointer)
        {
            // ptr alias
            underType = type.GetElementType()!;
            return text.Append(NameOf(underType))
                .Append('*')
                .ToString();
        }

        // ByRef => ref type
        if (type.IsByRef)
        {
            underType = type.GetElementType()!;
            return text.Append("ref ")
                .Append(NameOf(underType))
                .ToString();
        }

        // Array => T[]
        if (type.IsArray)
        {
            underType = type.GetElementType()!;
            return text.Append(NameOf(underType))
                .Append("[]")
                .ToString();
        }

        // Nested Type?
        if (type is { IsNested: true, IsGenericParameter: false })
        {
            text.Append(NameOf(type.DeclaringType))
                .Append('.');
        }

        // Start processing type name
        text typeName = type.Name.AsSpan();

        // If non-generic
        if (!type.IsGenericType)
        {
            // Just write the type name and we're done
            return text.Append(typeName).ToString();
        }

        /* The default Name for a generic type is:
         * Thing<>   = Thing`1
         * Thing<,>  = Thing`2
         * Thing<,,> = Thing`3
         * ...
         */
        int i = typeName.IndexOf('`');
        if (i >= 0)
        {
            text.Append(typeName[..i]);
        }
        else
        {
            Debugger.Break();
            // Odd... use the name
            text.Append(typeName);
        }

        // Add our generic types to finish
        var argTypes = type.GetGenericArguments();
        int argCount = argTypes.Length;
        Debug.Assert(argCount > 0);

        return text.Append('<')
            .EnumerateAndDelimit(argTypes,
                static (tb, argType) => tb.Append(NameOf(argType)),
                static tb => tb.Append(", "))
            .Append('>')
            .ToString();
    }


    /// <summary>
    /// Gets the rendered name of <typeparamref name="T"/>
    /// </summary>
    public static string NameOf<T>()
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
        => NameOf(typeof(T));

    /// <summary>
    /// Gets the rendered name of the <see cref="Type"/> of this <paramref name="instance"/>
    /// </summary>
    public static string NameOfType<TInstance>(this TInstance instance)
        where TInstance : struct
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        => NameOf<TInstance>();

    /// <summary>
    /// Gets the rendered name of the <see cref="Type"/> of this <paramref name="instance"/>
    /// </summary>
    public static string NameOfType(this object? instance)
        => NameOf(instance?.GetType());
}
