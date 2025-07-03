namespace ScrubJay.Text.Rendering;

/// <summary>
/// Extensions on <see cref="IRenderable"/>
/// </summary>
[PublicAPI]
public static class RenderableExtensions
{
    public static string Render<R>(this R renderable,
        GenericTypeConstraint.AllowsRefStruct<R> _ = default)
        where R : IRenderable
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        using var builder = new TextBuilder();
        renderable.RenderTo(builder);
        return builder.ToString();
    }
}

[PublicAPI]
public static class GenericRenderableExtensions
{
    public static string Render<T>(this T? value)
    {
        using var builder = new TextBuilder();
        RendererCache.RenderTo<T>(value, builder);
        return builder.ToString();
    }

    public static string RenderType<T>(this T? value)
    {
        using var builder = new TextBuilder();
        Type valueType = value?.GetType() ?? typeof(T);
        TypeRenderer.Default.RenderTo(valueType, builder);
        return builder.ToString();
    }
}