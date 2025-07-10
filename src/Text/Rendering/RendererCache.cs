using ScrubJay.Collections.NonGeneric;

namespace ScrubJay.Text.Rendering;

public static class RendererCache
{
    private static readonly List<Renderer> _renderers;

    static RendererCache()
    {
        _renderers = AppDomain
            .CurrentDomain
            .GetAssemblies()
            .SelectMany(static assembly => Result.TryInvoke(assembly.GetTypes).OkOr([]))
            .Where(static type => type.Implements(typeof(Renderer<>)))
            .Where(static type => !type.IsAbstract)
            .SelectWhere(static type => Result.TryInvoke(type, static t => Activator.CreateInstance(t)))
            .OfType<Renderer>()
            .ToList();
    }

    // ReSharper disable once UnusedMethodReturnValue.Local
    private static TextBuilder DefaultRenderTo<T>(T? value, TextBuilder builder)
    {
        switch (value)
        {
            case null:
                return builder.Append("null");
            case DBNull:
                return builder.Append(nameof(DBNull));
            case bool b:
                return builder.IfAppend(b, bool.TrueString, bool.FalseString);
            case byte u8:
                return builder.Append("(byte)").Format(u8);
            case sbyte i8:
                return builder.Append("(sbyte)").Format(i8);
            case short i16:
                return builder.Append("(short)").Format(i16);
            case ushort u16:
                return builder.Append("(ushort)").Format(u16);
            case int i32:
                return builder.Format(i32);
            case uint u32:
                return builder.Format(u32).Append('U');
            case long i64:
                return builder.Format(i64).Append('L');
            case ulong u64:
                return builder.Format(u64).Append("UL");
            case float f32:
                return builder.Format(f32, "N1").Append('f');
            case double f64:
                return builder.Format(f64, "N1").Append('d');
            case decimal dec:
                return builder.Format(dec, "N1").Append('m');
            case TimeSpan ts:
                return builder.Format(ts, "g");
            case DateTime dt:
                return builder.Format(dt, "yyyy-MM-dd HH:mm:ss");
            case char ch:
                return builder.Append('\'').Append(ch).Append('\'');
            case string str:
                return builder.Append('"').Append(str).Append('"');
#if !NETSTANDARD2_0
            case ITuple tuple:
            {
                return builder
                    .Append('(')
                    .EnumerateAndDelimit(
                        Enumerable.Range(0, tuple.Length),
                        (tb, i) => RenderTo(tuple[i], tb),
                        ", ")
                    .Append(')');
            }
#endif
            case Array array:
            {
                var wrapper = new ArrayAdapterND<object?>(array);
                return builder.Append('[')
                    .EnumerateAndDelimit(wrapper,
                        static (tb, item) => RenderTo(item, tb),
                        ", ")
                    .Append(']');
            }
            case IEnumerable enumerable:
            {
                return builder.Append('{')
                    .EnumerateAndDelimit(enumerable.OfType<object?>(),
                        static (tb, item) => RenderTo(item, tb),
                        ", ")
                    .Append('}');
            }
            default:
            {
                return builder.Format<T>(value);
            }
        }
    }

    public static void RenderTo<T>(T? value, TextBuilder builder)
    {
        if (value is IRenderable)
        {
            ((IRenderable)value).RenderTo(builder);
            return;
        }

        Type valueType = value?.GetType() ?? typeof(T);

        if (_renderers.TryGetFirst(r => r.CanRender(valueType)).IsOk(out var renderer))
        {
            if (renderer.Is<Renderer<T>>(out var typedRenderer))
            {
                typedRenderer.RenderTo(value, builder);
                return;
            }

            Debugger.Break();
            throw new NotImplementedException();
        }

        DefaultRenderTo<T>(value, builder);
    }

    public static void RenderTo<T>(T[]? array, TextBuilder builder)
    {
        if (array is null)
        {
            builder.Append("`null`");
            return;
        }

        builder.Append('[')
            .EnumerateAndDelimit(array,
                static (tb, item) => RenderTo(item, tb),
                ", ")
            .Append(']');
    }

    public static void RenderTo<T>(scoped ReadOnlySpan<T> span, TextBuilder builder)
    {
        builder.Append('[')
            .EnumerateAndDelimit(span,
                static (tb, item) => RenderTo(item, tb),
                ", ")
            .Append(']');
    }

    public static void RenderTo<T>(scoped Span<T> span, TextBuilder builder)
    {
        builder.Append('[')
            .EnumerateAndDelimit(span,
                static (tb, item) => RenderTo(item, tb),
                ", ")
            .Append(']');
    }

    public static void RenderTo(scoped text text, TextBuilder builder)
        => builder.Append('"').Append(text).Append('"');
}