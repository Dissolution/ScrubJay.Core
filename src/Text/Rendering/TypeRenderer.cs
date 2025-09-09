#pragma warning disable IDE0058, CA2263, IDE0060

namespace ScrubJay.Text.Rendering;

/// <summary>
/// A helper for rendering <see cref="Type"/> names
/// </summary>
[PublicAPI]
public sealed class TypeRenderer : Renderer<Type>, IHasDefault<TypeRenderer>
{
    public static TypeRenderer Default { get; } = new();

    // https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/
    private static readonly TypeMap<string> _typeNameCache = new(capacity: 18)
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

    private static TextBuilder RenderType(TextBuilder builder, Type? type)
    {
        if (type is null)
            return builder;

        string? name;
        Type? underType;

        if (_typeNameCache.TryGetValue(type, out name))
            return builder.Append(name);

        // Nullable is `T?`
        underType = Nullable.GetUnderlyingType(type);
        if (underType is not null)
        {
            return RenderType(builder, underType).Append('?');
        }

        // Array is `T[,,,n]`
        if (type.IsArray)
        {
            underType = type.GetElementType()!;
            Debug.Assert(underType is not null);
            return RenderType(builder, underType)
                .Append('[')
                .RepeatAppend(type.GetArrayRank() - 1, ',')
                .Append(']');
        }

        // Pointers are `T*`
        if (type.IsPointer)
        {
            underType = type.GetElementType()!;
            Debug.Assert(underType is not null);
            return RenderType(builder, underType).Append('*');
        }

        // Refs are `ref T` (could also be `&T`)
        if (type.IsByRef)
        {
            underType = type.GetElementType()!;
            Debug.Assert(underType is not null);
            return RenderType(builder.Append("ref "), underType);
        }

        // Nested types we want to indicate their parent (but not generic parameters)
        if (type.IsNested && !type.IsGenericParameter)
        {
            RenderType(builder, type.DeclaringType).Append('.');
        }

        // If we are not generic or are an enum, we append the name only
        if (!type.IsGenericType || type.IsEnum)
            return builder.Append(type.Name);

        return RenderNameAndGenericTypes(builder, type.Name, type.GetGenericArguments());
    }

    private static TextBuilder RenderNameAndGenericTypes(TextBuilder builder, string? name, Type[]? genericTypes)
    {
        return builder.IfNotNull(name,
            static (tb, n) =>
            {
                int index = n.IndexOf('`');
                if (index >= 0)
                {
                    tb.Append(n.AsSpan(0, index));
                }
                else
                {
                    tb.Append(n);
                }
            },
            static tb => tb.Append("???")
        ).IfNotEmpty(genericTypes,
            static (tb, types) => tb
                .Append('<')
                .Delimit(", ", types, static (b, type) => RenderType(b, type))
                .Append('>'));
    }

    public override TextBuilder RenderTo(TextBuilder builder, Type? type)
        => RenderType(builder, type);
}