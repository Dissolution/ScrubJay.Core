namespace ScrubJay.Text.Rendering;

public static class RendererCache
{
    private static readonly IRenderer[] _renderers;
    private static readonly ConcurrentTypeMap<IRenderer?> _rendererMap = [];

    internal static IReadOnlyList<IRenderer> Renderers => _renderers;
    internal static IReadOnlyDictionary<Type, IRenderer?> RendererMap => _rendererMap;

    static RendererCache()
    {
        _renderers = AppDomain
            .CurrentDomain
            // Get every assembly we can see
            .GetAssemblies()
            // Extract all their types (we use Result as dynamic and other secure assemblies can throw)
            .SelectMany(static assembly => Result.Try(assembly.GetTypes).OkOr([]))
            // Looking for renderers that we can instantiate
            .Where(static type =>
            {
                if (!type.Implements<IRenderer>())
                    return false;
                if (type.IsInterface || type.IsAbstract)
                    return false;
                if (type.IsGenericType)
                {
                    return false;
                }

                return true;
            })
            // Try to instantiate them (ignoring any that we cannot)
            .SelectWhere(static type => Result.Try(type, Activator.CreateInstance))
            // Cast to the base interface
            .OfType<IRenderer>()
            // and store
            .ToArray();
    }

    private static IRenderer<T>? GetRenderer<T>()
    {
        return _rendererMap.GetOrAdd<T>(findRenderer) as IRenderer<T>;

        static IRenderer<T>? findRenderer(Type type)
        {
            foreach (var renderer in _renderers)
            {
                if (renderer is IRenderer<T> typedRenderer &&
                    typedRenderer.CanRender(type))
                    return typedRenderer;
            }

            return null;
        }
    }

    private static TextBuilder DefaultRenderTo<T>(TextBuilder builder, T? value)
    {
        return value switch
        {
            null => builder.Append("null"),
            DBNull => builder.Append(nameof(DBNull)),
            bool b => builder.If(b, bool.TrueString, bool.FalseString),
            byte u8 => builder.Append("(byte)").Format(u8),
            sbyte i8 => builder.Append("(sbyte)").Format(i8),
            short i16 => builder.Append("(short)").Format(i16),
            ushort u16 => builder.Append("(ushort)").Format(u16),
            int i32 => builder.Format(i32),
            uint u32 => builder.Format(u32).Append('U'),
            long i64 => builder.Format(i64).Append('L'),
            ulong u64 => builder.Format(u64).Append("UL"),
            float f32 => builder.Format(f32, "N1").Append('f'),
            double f64 => builder.Format(f64, "N1").Append('d'),
            decimal dec => builder.Format(dec, "N1").Append('m'),
            TimeSpan ts => builder.Format(ts, "g"),
            DateTime dt => builder.Format(dt, "yyyy-MM-dd HH:mm:ss"),
            char ch => builder.Append('\'').Append(ch).Append('\''),
            string str => builder.Append('"').Append(str).Append('"'),
            _ => builder.Format<T>(value),
        };
    }


    public static TextBuilder RenderTo<T>(TextBuilder builder, T? value)
    {
        if (value is null)
            return builder;

        // If the value is Renderable, use its func
        if (value is IRenderable)
        {
            return ((IRenderable)value).RenderTo(builder);
        }

        // We're looking for something that can render T
        Type valueType = typeof(T);

#if DEBUG
        Type vt = value.GetType();
        if (valueType != typeof(Type) && valueType != vt)
            Debugger.Break();
#endif

        // see if we have a direct IRenderer<T>
        IRenderer<T>? typedRenderer = GetRenderer<T>();
        if (typedRenderer is not null)
        {
            typedRenderer.RenderValue(builder, value);
            return builder;
        }

        // see if we have something that can render this value
        foreach (var renderer in _renderers)
        {
            if (renderer.CanRender(valueType))
            {
                renderer.RenderObject(builder, (object)value!);
                return builder;
            }
        }

        return DefaultRenderTo(builder, value);
    }

    public static bool CanRender<T>() => GetRenderer<T>() is not null;

    public static bool CanRender(Type type) => _renderers.Any(r => r.CanRender(type));

#region Extensions

    public static string Render<T>(T? value)
    {
        return TextBuilder.New.Invoke(value, RenderTo).ToStringAndDispose();
    }

#endregion

    // }
    //
    // public static TextBuilder FluentRender<T>(TextBuilder builder, T[]? array)
    // {
    //     if (array is null)
    //     {
    //         return builder.Append("`null`");
    //     }
    //
    //     return builder.Append('[')
    //         .EnumerateAndDelimit(array,
    //             static (tb, item) => tb.Render(item),
    //             ", ")
    //         .Append(']');
    // }
    //
    // public static TextBuilder FluentRender<T>(TextBuilder builder, scoped ReadOnlySpan<T> span)
    // {
    //     return builder
    //         .Append('[')
    //         .EnumerateAndDelimit(span,
    //             static (tb, item) => tb.Render(item),
    //             ", ")
    //         .Append(']');
    // }
    //
    // public static TextBuilder FluentRender<T>(TextBuilder builder, scoped Span<T> span)
    // {
    //     return builder
    //         .Append('[')
    //         .EnumerateAndDelimit(span,
    //             static (tb, item) => tb.Render(item),
    //             ", ")
    //         .Append(']');
    // }
    //
    // public static TextBuilder FluentRender(TextBuilder builder, scoped text text)
    //     => builder.Append('"').Append(text).Append('"');
}