using System.Reflection;

namespace ScrubJay.Text.Rendering;

internal sealed class RendererCacheComparer : Comparer<(int Priority, IRenderer Renderer)>
{
    public override int Compare((int Priority, IRenderer Renderer) x, (int Priority, IRenderer Renderer) y)
    {
        // High to Low, so compare y with x (opposite order)
        return y.Priority.CompareTo(x.Priority);
    }
}

[PublicAPI]
public static class Renderer
{



    private static readonly Lock _lock = new();
    private static readonly OrderedList<(int, IRenderer)> _renderers = new(new RendererCacheComparer(), newestFirst: true);
    private static readonly ConcurrentTypeMap<IRenderer?> _rendererMap = [];

    static Renderer()
    {
        var domain = AppDomain.CurrentDomain;
        domain.AssemblyLoad += DomainOnAssemblyLoad;

        domain.GetAssemblies()
            .Consume(LoadRenderers);
    }

    private static void LoadRenderers(Assembly assembly)
    {
        Type[] types;
        try
        {
            types = assembly.GetTypes();
        }
        catch (Exception)
        {
            // assemblies can be finicky, ignore all exceptions
            return;
        }

        var renderers = types
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
            .SelectWhere<Type, (IRenderer, int)>(type =>
            {
                if (Result.Try(type, Activator.CreateInstance).IsError(out var error, out var obj))
                    return error;

                if (obj is not IRenderer renderer)
                    return new InvalidOperationException();

                int priority = 0;
                var priorityAttribute = type.GetCustomAttribute<RendererPriorityAttribute>();
                if (priorityAttribute is not null)
                {
                    priority = priorityAttribute.Priority;
                }

                return Ok((renderer, priority));
            })
            .ToList();

        if (renderers.Count > 0)
        {
            lock (_lock)
            {
                foreach (var (renderer, priority) in renderers)
                {
                    _renderers.Add((priority, renderer));
                }
                _rendererMap.Clear();
            }
        }
    }

    private static void DomainOnAssemblyLoad(object? sender, AssemblyLoadEventArgs args)
    {
        LoadRenderers(args.LoadedAssembly);
    }

    internal static TextBuilder DefaultRenderValue<T>(TextBuilder builder, T value)
    {
        Debug.Assert(value is not null);
        return value switch
        {
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
            float f32 => builder.Format(f32, "G9").Append('f'),
            double f64 => builder.Format(f64, "G17").Append('d'),
            decimal dec => builder.Format(dec, "G").Append('m'),
            TimeSpan ts => builder.Format(ts, "g"),
            DateTime dt => builder.Format(dt, "yyyy-MM-dd HH:mm:ss"),
            DateTimeOffset dto => builder.Format(dto, "yyyy-MM-dd HH:mm:ss"),
            char ch => builder.Append('\'').Append(ch).Append('\''),
            string str => builder.Append('"').Append(str).Append('"'),
            _ => builder.Format<T>(value),
        };
    }

    internal static IRenderer? GetRenderer(Type type)
    {
        foreach (var (_, renderer) in _renderers)
        {
            if (renderer.CanRender(type))
                return renderer;
        }

        return null;
    }

    internal static IRenderer<T>? GetRenderer<T>()
    {
        return _rendererMap.GetOrAdd<T>(findRenderer) as IRenderer<T>;

        static IRenderer<T>? findRenderer(Type type)
        {
            foreach (var (_, renderer) in _renderers)
            {
                if (renderer is IRenderer<T> r && renderer.CanRender(type))
                    return r;
            }

            return null;
        }
    }




    public static TextBuilder RenderValue<T>(TextBuilder builder, T? value)
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

        // see if we have a direct IRenderer<T>
        IRenderer<T>? typedRenderer = GetRenderer<T>();
        if (typedRenderer is not null)
        {
            return typedRenderer.RenderValue(builder, value);
        }

        // see if we have something that can render this value
        IRenderer? renderer = GetRenderer(valueType);
        if (renderer is not null)
        {
            return renderer.RenderObject(builder, (object)value);
        }

        // fallback to default
        return DefaultRenderValue(builder, value);
    }

    public static bool CanRender<T>() => GetRenderer<T>() is not null;

    public static bool CanRender(Type type) => GetRenderer(type) is not null;
}