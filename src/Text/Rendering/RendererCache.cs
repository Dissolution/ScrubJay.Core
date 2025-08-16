namespace ScrubJay.Text.Rendering;

public static class RendererCache
{
    private static readonly List<IRenderer> _renderers;

    static RendererCache()
    {
        _renderers = AppDomain
            .CurrentDomain
            .GetAssemblies()
            .SelectMany(static assembly => Result.TryInvoke(assembly.GetTypes).OkOr([]))
            .Where(static type => type.Implements(typeof(IRenderer<>)))
            .Where(static type => !type.IsAbstract)
            .SelectWhere(static type => Result.TryInvoke(type, static t => Activator.CreateInstance(t)))
            .OfType<IRenderer>()
            .ToList();
    }

    public static TextBuilder FluentRender<T>(TextBuilder builder, T? value)
    {
        if (value is IRenderable)
        {
            ((IRenderable)value).RenderTo(builder);
            return builder;
        }

        Type valueType = value?.GetType() ?? typeof(T);

        if (_renderers.TryGetFirst(r => r.CanRender(valueType)).IsOk(out var renderer))
        {
            var typedRenderer = renderer as IRenderer<T>;

            if (typedRenderer is not null)
            {
                return typedRenderer.FluentRender(builder, value);
            }

            var tt = typeof(T);

            Debugger.Break();
            throw new NotImplementedException();
        }

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

    public static TextBuilder FluentRender<T>(TextBuilder builder, T[]? array)
    {
        if (array is null)
        {
            return builder.Append("`null`");
        }

        return builder.Append('[')
            .EnumerateAndDelimit(array,
                static (tb, item) => tb.Render(item),
                ", ")
            .Append(']');
    }

    public static TextBuilder FluentRender<T>(TextBuilder builder, scoped ReadOnlySpan<T> span)
    {
        return builder
            .Append('[')
            .EnumerateAndDelimit(span,
                static (tb, item) => tb.Render(item),
                ", ")
            .Append(']');
    }

    public static TextBuilder FluentRender<T>(TextBuilder builder, scoped Span<T> span)
    {
        return builder
            .Append('[')
            .EnumerateAndDelimit(span,
                static (tb, item) => tb.Render(item),
                ", ")
            .Append(']');
    }

    public static TextBuilder FluentRender(TextBuilder builder, scoped text text)
        => builder.Append('"').Append(text).Append('"');
}