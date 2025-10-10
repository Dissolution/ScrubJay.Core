using System.Collections.Concurrent;

namespace ScrubJay.Text.Scratch;

public delegate void RenderTo<in T>(TextBuilder builder, T? value)
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
;

public static class ScratchRenderer
{
    private static readonly ConcurrentBag<Delegate> _renderers = [];

    static ScratchRenderer()
    {
        RenderTo<Guid> renderGuid = static (builder, guid) => builder.Format(guid, "N");
        _renderers.Add(renderGuid);
    }

    public static void AddRenderer<T>(RenderTo<T> renderer)
    {
        _renderers.Add(renderer);
    }

    public static TextBuilder RenderTo<T>(TextBuilder builder, T? value)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        foreach (var renderer in _renderers)
        {
            if (renderer.Is<RenderTo<T>>(out var renderTo))
            {
                renderTo(builder, value);
                return builder;
            }
        }

        // no registered renderers can handle this value
        throw Ex.NotImplemented();
    }
}