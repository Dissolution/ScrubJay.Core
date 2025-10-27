using System.Collections.Concurrent;
using System.Reflection;

namespace ScrubJay.Text.Rendering;

[PublicAPI]
public static class Renderer
{
    private static readonly ConcurrentDictionary<string, MethodInfo> _renderMethods = [];
    private static readonly TypeMap<Delegate> _renderers = [];

    static Renderer()
    {
        Add<bool>(static (tb, boolean) => tb.If(boolean, "true", "false"));

        Add<uint>(static (tb, u32) => tb.Format(u32).Write('U'));
        Add<long>(static (tb, i64) => tb.Format(i64).Write('L'));
        Add<ulong>(static (tb, u64) => tb.Format(u64).Write("UL"));
        Add<float>(static (tb, f32) =>
        {
            //tb.Format(f32, "N");
            //tb.Write('…');

            tb.Format(f32, "G9");

            tb.Append('f');
        });
        Add<double>(static (tb, f64) =>
        {
            //tb.Format(f64, "N");
            //tb.Write('…');

            tb.Format(f64, "G17");

            tb.Append('d');
        });
        Add<decimal>(static (tb, dec) => tb.Format(dec, "G").Append('m'));

        Add<TimeSpan>(static (tb, ts) => tb.Format(ts, "g"));
        Add<DateTime>(static (tb, dt) => tb.Format(dt, "yyyy-MM-dd HH:mm:ss"));
        Add<DateTimeOffset>(static (tb, dto) => tb.Format(dto, "yyyy-MM-dd HH:mm:ss"));

        Add<DBNull>(static (tb, _) => tb.Write(nameof(DBNull)));
        Add<Guid>(static (tb, guid) =>
        {
            var buffer = tb.Allocate(36);
#if NETFRAMEWORK || NETSTANDARD2_0
            string str = guid.ToString("N");
            Notsafe.Text.CopyBlock(str, buffer, 36);
#else
            guid.TryFormat(buffer, out _, format: "D");
#endif
            buffer.ForEach((ref char ch) =>
            {
                if (ch == 'a')
                    ch = 'A';
                else if (ch == 'b')
                    ch = 'B';
                else if (ch == 'c')
                    ch = 'C';
                else if (ch == 'd')
                    ch = 'D';
                else if (ch == 'e')
                    ch = 'E';
                else if (ch == 'f')
                    ch = 'F';
            });
        });

        Add<object>(RenderObjectTo);
        Add<ITuple>(RenderTupleTo);
        Add<Type>(TypeRenderer.RenderTypeTo);
    }

    public static void Add<T>(RenderTo<T> renderTo)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        _renderers.AddOrUpdate<T>(renderTo);
    }

    public static void Add<T>(IRenderer<T> renderer)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        Throw.IfNull(renderer);
        _renderers.AddOrUpdate<T>(renderer.RenderTo);
    }

    private static MethodInfo GetRenderMethod(string name, Type genericType)
    {
        return _renderMethods.GetOrAdd(name, findMethod);

        MethodInfo findMethod(string n) => typeof(Renderer)
            .GetMethod(n, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
            .ThrowIfNull($"Cannot find Renderer.{n} method");
    }

    private static RenderTo<T> RenderKnown<T>(
        string renderToMethodName,
        params Type[]? genericTypes)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        genericTypes ??= [typeof(T)];

        var renderMethod = GetRenderMethod(renderToMethodName, typeof(T));
        if (renderMethod.GetGenericArguments().Length != genericTypes.Length)
            throw Ex.Invalid();

        var exactMethod = renderMethod.MakeGenericMethod(genericTypes);
        return Delegate.CreateDelegate<RenderTo<T>>(exactMethod);
    }

    internal static void RenderEnumTo<E>(TextBuilder builder, E e)
        where E : struct, Enum
    {
        EnumInfo.RenderTo(builder, e);
    }

    internal static void RenderReadOnlySpanTo<T>(TextBuilder builder, scoped ReadOnlySpan<T> span)
    {
        builder.Append('[')
            .Delimit(", ", span, static (tb, value) => tb.Render(value))
            .Append(']');
    }

    internal static void RenderSpanTo<T>(TextBuilder builder, scoped Span<T> span)
    {
        builder.Append('[')
            .Delimit(", ", span, static (tb, value) => tb.Render(value))
            .Append(']');
    }

    internal static void RenderTupleTo(TextBuilder builder, ITuple tuple)
    {
        builder.Write('(');
        if (tuple.Length > 0)
        {
            builder.Render(tuple[0]);
            for (var i = 1; i < tuple.Length; i++)
            {
                builder.Append(", ").Render(tuple[i]);
            }
        }

        builder.Write(')');
    }

    internal static void RenderArrayTo<T>(TextBuilder builder, T[] array)
    {
        builder.Append('[')
            .Delimit(", ", array, static (tb, value) => tb.Render(value))
            .Append(']');
    }

    internal static void RenderEnumerableTo<T>(TextBuilder builder, IEnumerable<T> enumerable)
    {
        if (enumerable is IList<T> list)
        {
            builder
                .Append('[')
                .Delimit(", ", list, static (tb, value) => tb.Render(value))
                .Append(']');
        }
        else if (enumerable is ICollection<T> collection)
        {
            builder
                .Append('(')
                .Delimit(", ", collection, static (tb, value) => tb.Render(value))
                .Append(')');
        }
        else
        {
            builder
                .Append('{')
                .Delimit(", ", enumerable, static (tb, value) => tb.Render(value))
                .Append('}');
        }
    }

    internal static void RenderDictionaryTo<K, V>(TextBuilder builder, IDictionary<K, V> dictionary)
    {
        builder.Append('{')
            .Delimit(", ", dictionary,
                static (tb, pair) => tb.Render(pair.Key).Append(": ").Render(pair.Value))
            .Append('}');
    }

    internal static void RenderRenderableTo<R>(TextBuilder builder, R renderable)
        where R : IRenderable
    {
        renderable.RenderTo(builder);
    }

    internal static void RenderObjectTo(TextBuilder builder, object obj)
    {
        var type = obj.GetType();
        if (type == typeof(object))
        {
            builder.Append("(object)");
            return;
        }

        Debugger.Break();
        typeof(Renderer)
            .GetMethod(nameof(WriteTo), BindingFlags.NonPublic | BindingFlags.Static)
            .ThrowIfNull()
            .MakeGenericMethod(type)
            .Invoke(null, [builder, obj]);
        Debugger.Break();
    }

    private static RenderTo<T>? FindRenderTo<T>()
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        if (_renderers.TryGetValue<T>(out var del))
        {
            if (del.Is<RenderTo<T>>(out var renderTo))
            {
                return renderTo;
            }
            else
            {
                return null;
            }
        }

        var type = typeof(T);

        if (type.IsEnum)
        {
            return RenderKnown<T>(nameof(RenderEnumTo));
        }

        if (type.IsArray)
        {
            var elementType = type.GetElementType().ThrowIfNull();
            return RenderKnown<T>(nameof(RenderArrayTo), elementType);
        }

        if (type.Implements<IRenderable>())
        {
            return RenderKnown<T>(nameof(RenderRenderableTo), type);
        }

        if (type.Implements(typeof(ReadOnlySpan<>)))
        {
            return RenderKnown<T>(nameof(RenderReadOnlySpanTo), type.GenericTypeArguments);
        }

        if (type.Implements(typeof(Span<>)))
        {
            return RenderKnown<T>(nameof(RenderReadOnlySpanTo), type.GenericTypeArguments);
        }

        if (type.Implements(typeof(IEnumerable<>)))
        {
            return RenderKnown<T>(nameof(RenderReadOnlySpanTo), type.GenericTypeArguments);
        }

        if (type.Implements(typeof(IDictionary<,>)))
        {
            return RenderKnown<T>(nameof(RenderDictionaryTo), type.GenericTypeArguments);
        }

        return null;
    }

    internal static void WriteTo<T>(TextBuilder builder, T? value)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        if (value is null)
        {
            builder.Append("`null");
            return;
        }

        var renderTo = FindRenderTo<T>();
        if (renderTo is not null)
        {
            renderTo(builder, value);
            return;
        }

        // fallback to Append
        builder.Append<T>(value);
    }
}