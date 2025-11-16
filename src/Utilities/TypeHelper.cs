// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ScrubJay.Utilities;

public static class TypeHelper
{
    // C# type aliases
    private static readonly ConcurrentTypeMap<string> _displays = new()
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
    };

    static TypeHelper()
    {
    }

    public static void Add(Type type, string display)
    {
        _displays.AddOrUpdate(type, display);
    }

    public static void Add<T>(string display)
    {
        _displays.AddOrUpdate<T>(display);
    }

    private sealed class TypeDisplayBuilder : IDisposable
    {
        private TextBuilder _builder;
        private Type[] _genericTypes;
        private int _genericTypesOffset;

        public TypeDisplayBuilder(Type[] genericTypes)
        {
            _builder = new();
            _genericTypes = genericTypes;
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

        public TextBuilder Write(Type type)
        {
            if (_displays.TryGetValue(type, out var display))
            {
                return _builder.Append(display);
            }

            if (type.IsByRef)
            {
                // T&
                type = type.GetElementType()!;
                return Write(type).Append('&');
            }

            if (type.IsPointer)
            {
                // T*
                type = type.GetElementType()!;
                return Write(type).Append('*');
            }

            if (type.IsArray)
            {
                // T[] .. T[,,,,]
                int rank = type.GetArrayRank();
                type = NextGenericType();
                return Write(type)
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
                    return Write(type).Append('?');
                }

                // Tuples!
                if (genericTypeDefinition == typeof(ValueTuple<>) ||
                    genericTypeDefinition == typeof(ValueTuple<,>) ||
                    genericTypeDefinition == typeof(ValueTuple<,,>) ||
                    genericTypeDefinition == typeof(ValueTuple<,,,>) ||
                    genericTypeDefinition == typeof(ValueTuple<,,,,>) ||
                    genericTypeDefinition == typeof(ValueTuple<,,,,,>) ||
                    genericTypeDefinition == typeof(ValueTuple<,,,,,,>) ||
                    genericTypeDefinition == typeof(ValueTuple<,,,,,,,>) ||
                    genericTypeDefinition == typeof(Tuple<>) ||
                    genericTypeDefinition == typeof(Tuple<,>) ||
                    genericTypeDefinition == typeof(Tuple<,,>) ||
                    genericTypeDefinition == typeof(Tuple<,,,>) ||
                    genericTypeDefinition == typeof(Tuple<,,,,>) ||
                    genericTypeDefinition == typeof(Tuple<,,,,,>) ||
                    genericTypeDefinition == typeof(Tuple<,,,,,,>) ||
                    genericTypeDefinition == typeof(Tuple<,,,,,,,>))
                {
                    var tupleGenericTypes = type.GetGenericArguments();
                    _genericTypesOffset += tupleGenericTypes.Length;
                    return _builder.Append('(')
                        .Delimit(", ", tupleGenericTypes, (_,t) => Write(t))
                        .Append(')');
                }
            }

            // Nested Type?
            if (type.IsNested && !type.IsGenericParameter)
            {
                Write(type.DeclaringType!)
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
            return _builder.Delimit(", ", currentGenericTypes, (_, t) => Write(t))
                .Append('>');
        }

        public void Dispose()
        {
            _builder.Dispose();
        }

        public override string ToString()
        {
            return _builder.ToString();
        }
    }

    public static string Display(this Type type)
    {
        return _displays.GetOrAdd(type, static t =>
        {
            var genericTypes = t.GetGenericArguments();
            using var builder = new TypeDisplayBuilder(genericTypes);
            builder.Write(t);
            return builder.ToString();
        });
    }
}