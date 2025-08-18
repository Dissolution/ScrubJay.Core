namespace ScrubJay.Text.Rendering;

public static class RendererCache
{
    private static readonly List<IRenderer> _renderers;
    private static readonly ConcurrentTypeMap<IRenderer?> _rendererMap = [];

    static RendererCache()
    {
        _renderers = AppDomain
            .CurrentDomain
            .GetAssemblies()
            .SelectMany(static assembly => Result.TryInvoke(assembly.GetTypes).OkOr([]))
            .Where(static type => type.Implements<IRenderer>())
            .Where(static type => !type.IsAbstract && !type.IsGenericType)
            .SelectWhere(static type => Result.TryInvoke(type, static t => Activator.CreateInstance(t)))
            .OfType<IRenderer>()
            .ToList();
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
            bool b => builder.IfAppend(b, bool.TrueString, bool.FalseString),
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
        // If the value is Renderable, use its func
        if (value is IRenderable)
        {
            return ((IRenderable)value).RenderTo(builder);
        }

        // We're looking for something that can render T
        Type valueType = typeof(T);
        Type vt = value?.GetType() ?? typeof(void);
        if (vt != valueType)
            Debugger.Break();

        // see if we have a direct IRenderer<T>
        IRenderer<T>? typedRenderer = GetRenderer<T>();
        if (typedRenderer is not null)
        {
            Debugger.Break();
            return typedRenderer.RenderTo(builder, value);
        }

        // see if we have something that can render this value
        foreach (var renderer in _renderers)
        {
            if (renderer.CanRender(valueType))
            {
                Debugger.Break();
                return renderer.RenderTo(builder, (object?)value);
            }
        }

        Debugger.Break();
        return DefaultRenderTo(builder, value);
    }

#region Extensions

#if NET9_0_OR_GREATER
    public static string Render<R>(this R renderable,
        GenericTypeConstraint.AllowsRefStruct<R> _ = default)
        where R : IRenderable
        , allows ref struct
    {
        using var builder = new TextBuilder();
        renderable.RenderTo(builder);
        return builder.ToString();
    }
#endif

    public static string Render<T>(this T? value)
    {
        var builder = new TextBuilder();
        _ = RenderTo(builder, value);
        return builder.ToStringAndDispose();
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