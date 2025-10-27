namespace ScrubJay.Text.Rendering;

[PublicAPI]
public interface IRenderer<in T>
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
{
    void RenderTo(TextBuilder builder, T value);
}

[PublicAPI]
public interface IRenderable
{
    void RenderTo(TextBuilder builder);
}