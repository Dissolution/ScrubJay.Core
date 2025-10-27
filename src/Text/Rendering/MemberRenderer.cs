using System.Reflection;

#pragma warning disable IDE0058, CA2263, IDE0060

namespace ScrubJay.Text.Rendering;

/// <summary>
/// A helper for rendering <see cref="Type"/> names
/// </summary>
[PublicAPI]
public static class TypeRenderer
{
    // https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/
    private static readonly TypeMap<string> _typeRenderCache = new(capacity: 18)
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

    internal static TextBuilder AppendName(this TextBuilder builder, MemberInfo? member)
    {
        if (member is null)
            return builder.Append("`member?");

        text name = member.Name;
        if (member.IsGeneric)
        {
            var i = name.LastIndexOf('`');
            if (i >= 0)
            {
                name = name[..i];
            }
        }

        return builder.Append(name);
    }

    internal static TextBuilder AppendGenericTypes(this TextBuilder builder, MemberInfo? member)
    {
        if (member is null)
            return builder;

        var genericTypes = member.GenericTypes();
        return builder
            .IfNotEmpty(genericTypes, static (tb, types) =>
                tb.Append('<')
                    .Delimit(", ", types, static (b, type) => b.Render(type))
                    .Append('>'));
    }


    public static void RenderTypeTo(TextBuilder builder, Type? type)
    {
        string? name;
        Type? underType;

        if (type is null)
        {
        }
        else if (_typeRenderCache.TryGetValue(type, out name))
        {
            // predefined simple type names

            builder.Write(name);
        }
        else if (Nullable.GetUnderlyingType(type).IsNotNull(out underType))
        {
            // Nullable<T> as `T?`

            RenderTypeTo(builder, underType);
            builder.Write('?');
        }
        else if (type.IsArray)
        {
            // Array as `T[,,,n]`

            underType = type.GetElementType()!;
            Debug.Assert(underType is not null);
            RenderTypeTo(builder, underType);
            builder
                .Append('[')
                .Repeat(type.GetArrayRank() - 1, ',')
                .Append(']');
        }
        else if (type.IsPointer)
        {
            // Pointers as `T*`

            underType = type.GetElementType()!;
            Debug.Assert(underType is not null);
            RenderTypeTo(builder, underType);
            builder.Write('*');
        }
        else if (type.IsRef)
        {
            // refs and by-refs as `ref T`

            underType = type.GetElementType()!;
            Debug.Assert(underType is not null);
            builder.Write("ref ");
            RenderTypeTo(builder, underType);
        }
        else if (type.IsNested && !type.IsGenericParameter)
        {
            // Nested types we want to indicate their parent (but not generic parameters)
            RenderTypeTo(builder, type.DeclaringType);
            builder.Write('.');
        }
        else if (!type.IsGenericType || type.IsEnum)
        {
            // If we are not generic or are an enum, we append the name only
            builder.Write(type.Name);
        }
        else
        {
            builder.AppendName(type)
                .AppendGenericTypes(type);
        }
    }
}