#if NET8_0_OR_GREATER
using System.Collections.Frozen;
#elif !NETSTANDARD
using System.Collections.Immutable;
#endif

namespace ScrubJay.Rendering;

public sealed class TypeRenderer : IValueRenderer<Type>, IHasDefault<TypeRenderer>
{
    public static TypeRenderer Default { get; } = new();

    // C# type aliases
    private static readonly ConcurrentTypeMap<string> _typeRenders = new()
    {
        [typeof(byte)] = "byte",
        [typeof(sbyte)] = "sbyte",
        [typeof(short)] = "short",
        [typeof(ushort)] = "ushort",
        [typeof(int)] = "int",
        [typeof(uint)] = "uint",
        [typeof(long)] = "long",
        [typeof(ulong)] = "ulong",
        [typeof(nint)] = "nint",
        [typeof(nuint)] = "nuint",
        [typeof(float)] = "float",
        [typeof(double)] = "double",
        [typeof(decimal)] = "decimal",
        [typeof(bool)] = "bool",
        [typeof(char)] = "char",
        [typeof(string)] = "string",
        [typeof(object)] = "object",
        [typeof(void)] = "void",
        [typeof(Tuple)] = "()",
        [typeof(ValueTuple)] = "()",
    };

    #if NET8_0_OR_GREATER
    private static readonly FrozenSet<Type> _tupleGenericTypeDefinitions;

    static TypeRenderer()
    {
        _tupleGenericTypeDefinitions =
            new HashSet<Type>
            {
                typeof(ValueTuple<>),
                typeof(ValueTuple<,>),
                typeof(ValueTuple<,,>),
                typeof(ValueTuple<,,,>),
                typeof(ValueTuple<,,,,>),
                typeof(ValueTuple<,,,,,>),
                typeof(ValueTuple<,,,,,,>),
                typeof(ValueTuple<,,,,,,,>),
                typeof(Tuple<>),
                typeof(Tuple<,>),
                typeof(Tuple<,,>),
                typeof(Tuple<,,,>),
                typeof(Tuple<,,,,>),
                typeof(Tuple<,,,,,>),
                typeof(Tuple<,,,,,,>),
                typeof(Tuple<,,,,,,,>),
            }.ToFrozenSet();
    }
    #elif !NETSTANDARD
    private static readonly ImmutableHashSet<Type> _tupleGenericTypeDefinitions;

    static TypeRenderer()
    {
        _tupleGenericTypeDefinitions =
            new HashSet<Type>
            {
                typeof(ValueTuple<>),
                typeof(ValueTuple<,>),
                typeof(ValueTuple<,,>),
                typeof(ValueTuple<,,,>),
                typeof(ValueTuple<,,,,>),
                typeof(ValueTuple<,,,,,>),
                typeof(ValueTuple<,,,,,,>),
                typeof(ValueTuple<,,,,,,,>),
                typeof(Tuple<>),
                typeof(Tuple<,>),
                typeof(Tuple<,,>),
                typeof(Tuple<,,,>),
                typeof(Tuple<,,,,>),
                typeof(Tuple<,,,,,>),
                typeof(Tuple<,,,,,,>),
                typeof(Tuple<,,,,,,,>),
            }.ToImmutableHashSet();
    }
    #else
    private static readonly HashSet<Type> _tupleGenericTypeDefinitions;

    static TypeRenderer()
    {
        _tupleGenericTypeDefinitions =
        [
            typeof(ValueTuple<>),
            typeof(ValueTuple<,>),
            typeof(ValueTuple<,,>),
            typeof(ValueTuple<,,,>),
            typeof(ValueTuple<,,,,>),
            typeof(ValueTuple<,,,,,>),
            typeof(ValueTuple<,,,,,,>),
            typeof(ValueTuple<,,,,,,,>),
            typeof(Tuple<>),
            typeof(Tuple<,>),
            typeof(Tuple<,,>),
            typeof(Tuple<,,,>),
            typeof(Tuple<,,,,>),
            typeof(Tuple<,,,,,>),
            typeof(Tuple<,,,,,,>),
            typeof(Tuple<,,,,,,,>),
        ];
    }
    #endif

    public void RenderTo(Type type, TextBuilder builder)
    {
        if (_typeRenders.TryGetValue(type, out var rendering))
        {
            builder.Write(rendering);
            return;
        }

        Type? underType;

        if (type.IsByRef)
        {
            // Reference Types
            // display as `type&`

            Debug.Assert(type.GetGenericArguments().Length == 0);
            underType = type.GetElementType()!;
            Debug.Assert(underType is not null);
            RenderTo(underType!, builder);
            builder.Write('&');
            return;
        }

        if (type.IsPointer)
        {
            // Pointers:
            // display as `type*`

            Debug.Assert(type.GetGenericArguments().Length == 0);
            underType = type.GetElementType()!;
            Debug.Assert(underType is not null);
            RenderTo(underType!, builder);
            builder.Write('*');
            return;
        }

        if (type.IsArray)
        {
            int rank = type.GetArrayRank();
            Debug.Assert(type.GetGenericArguments().Length == 0);
            underType = type.GetElementType()!;
            Debug.Assert(underType is not null);

            RenderTo(underType!, builder);
            builder
                .Append('[')
                .Repeat(rank - 1, ',')
                .Append(']');
            return;
        }

        // Gather any generic types
        Span<Type> genericTypes = type.GetGenericArguments();

        if (type.IsGenericType)
        {
            var genericTypeDefinition = type.GetGenericTypeDefinition();

            if (genericTypeDefinition == typeof(Nullable<>))
            {
                Debug.Assert(genericTypes.Length >= 1);
                underType = genericTypes[0];
                genericTypes = genericTypes[1..];
                // Nullable<struct>:
                // display as `type?`
                RenderTo(underType, builder);
                builder.Write('?');
                return;
            }

            if (_tupleGenericTypeDefinitions.Contains(genericTypeDefinition))
            {
                builder.Append('(')
                    .Delimit(", ", genericTypes, (tb, gt) => RenderTo(gt, tb))
                    .Append(')');

                return;
            }
        }

        if (type.IsNested && !type.IsGenericParameter)
        {
            var declaringType = type.DeclaringType.ThrowIfNull();
            var declaringTypeGenericTypes = declaringType.GetGenericArguments();
            if (declaringTypeGenericTypes.Length > 0)
            {
                if (Sequence.StartsWith(genericTypes, declaringTypeGenericTypes, LooseTypeComparer.Default))
                {
                    genericTypes = genericTypes[declaringTypeGenericTypes.Length..];
                }
                else
                {
                    Debugger.Break();
                }

            }

            RenderTo(declaringType, builder);
            builder.Write('.');
        }

        if (genericTypes.Length > 0)
        {
            Debug.Assert(type.IsGeneric);

            // name might have a ` in it
            int i = type.Name.IndexOf('`');
            if (i >= 0)
            {
                builder.Append(type.Name.AsSpan(0, i));
            }
            else
            {
                Debugger.Break();
                builder.Append(type.Name);
            }

            builder.Append('<')
                .Delimit(", ", genericTypes, (tb, gt) => RenderTo(gt, tb))
                .Append('>');
        }
        else
        {
            Debug.Assert(genericTypes.Length == 0);
            builder.Append(type.Name);
        }

        return;
    }

    public string Render(Type type)
    {
        using var builder = new TextBuilder();
        RenderTo(type, builder);
        return builder.ToString();
    }

    /*

    private sealed class TypeRenderingBuilder
    {
        private readonly TextBuilder _builder;
        private readonly Type _type;
        private readonly Type[] _genericTypes;
        private int _genericTypesOffset;

        public TypeRenderingBuilder(TextBuilder builder, Type type)
        {
            _builder = new();
            _type = type;
            _genericTypes = type.GetGenericArguments();
            _genericTypesOffset = 0;
        }

        private Type NextGenericType()
        {
            if (_genericTypesOffset >= _genericTypes.Length)
                Debugger.Break();

            Type type = _genericTypes[_genericTypesOffset];
            _genericTypesOffset++;
            return type;
        }

        private ReadOnlySpan<Type> NextGenericTypes(int count)
        {
            ReadOnlySpan<Type> types = _genericTypes.AsSpan(_genericTypesOffset, count);
            _genericTypesOffset += count;
            return types;
        }

        private TextBuilder InnerAppendType(Type type)
        {
            if (_typeRenders.TryGetValue(type, out var rendering))
            {
                return _builder.Append(rendering);
            }

            if (type.IsByRef)
            {
                // T&
                type = type.GetElementType()!;
                return InnerAppendType(type).Append('&');
            }

            if (type.IsPointer)
            {
                // T*
                type = type.GetElementType()!;
                return InnerAppendType(type).Append('*');
            }

            if (type.IsArray)
            {
                // T[] .. T[,,,,]
                int rank = type.GetArrayRank();
                type = NextGenericType();
                return InnerAppendType(type)
                    .Append('[')
                    .Repeat(rank - 1, ',')
                    .Append(']');
            }

            // Use this to check for Nullable<> and Tuples
            // but only as a shortcut
            // we know how generic we are based on genericTypes
            if (type.IsGenericType)
            {
                var genericTypeDefinition = type.GetGenericTypeDefinition();

                // Nullable<T> -> "T?"
                // note: Nullable.GetUnderlyingType() ultimately takes this path
                if (genericTypeDefinition == typeof(Nullable<>))
                {
                    type = NextGenericType();
                    return InnerAppendType(type).Append('?');
                }

                // Tuples!
                if (genericTypeDefinition == typeof(ValueTuple<>)
                    genericTypeDefinition == typeof(ValueTuple<,>)
                    genericTypeDefinition == typeof(ValueTuple<,,>)
                    genericTypeDefinition == typeof(ValueTuple<,,,>)
                    genericTypeDefinition == typeof(ValueTuple<,,,,>)
                    genericTypeDefinition == typeof(ValueTuple<,,,,,>)
                    genericTypeDefinition == typeof(ValueTuple<,,,,,,>)
                    genericTypeDefinition == typeof(ValueTuple<,,,,,,,>)
                    genericTypeDefinition == typeof(Tuple<>)
                    genericTypeDefinition == typeof(Tuple<,>)
                    genericTypeDefinition == typeof(Tuple<,,>)
                    genericTypeDefinition == typeof(Tuple<,,,>)
                    genericTypeDefinition == typeof(Tuple<,,,,>)
                    genericTypeDefinition == typeof(Tuple<,,,,,>)
                    genericTypeDefinition == typeof(Tuple<,,,,,,>)
                    genericTypeDefinition == typeof(Tuple<,,,,,,,>)
                {
                    var tupleGenericTypes = type.GetGenericArguments();
                    _genericTypesOffset += tupleGenericTypes.Length;
                    return _builder.Append('(')
                        .Delimit(", ", tupleGenericTypes, (_,t) => InnerAppendType(t))
                        .Append(')');
                }
            }

            // Nested Type?
            if (type.IsNested && !type.IsGenericParameter)
            {
                InnerAppendType(type.DeclaringType!)
                    .Write('.');
            }

            // generics have a ` in their name
            int i = type.Name.IndexOf('`');
            if (i == -1)
            {
                // simple type
                return _builder.Append(type.Name);
            }

            _builder.Append(type.Name.AsSpan(0, i))
                .Write('<');
#if NETSTANDARD2_0
            int gaCount = int.Parse(type.Name.Substring(i + 1));
#else
            int gaCount = int.Parse(type.Name.AsSpan(i+1));
#endif
            var currentGenericTypes = NextGenericTypes(gaCount);
            return _builder.Delimit(", ", currentGenericTypes, (_, t) => InnerAppendType(t))
                .Append('>');
        }


        public void Build()
        {
            InnerAppendType(_type);
        }
    }

    private static string CreateRendering(Type type, TextBuilder builder)
    {
        var typeRenderingBuilder = new TypeRenderingBuilder(builder, type);
        typeRenderingBuilder.Build();

    }

    public void RenderTo(Type type, TextBuilder builder)
    {
        string render = _typeRenders.GetOrAdd(type, static (t, tb) =>
        {
            var builder = new TypeRenderingBuilder(tb, t);
            builder.Build();
        }, builder);
        builder.Write(render);

        return _typeRenders.GetOrAdd(type, static t =>
        {
            var genericTypes = t.GetGenericArguments();
            using var builder = new TypeRenderingBuilder(genericTypes);
            builder.Write(t);
            return builder.ToString();
        });
    }
    */
}