namespace ScrubJay.Rendering;

[PublicAPI]
public interface IRenderable
{
    void RenderTo(TextBuilder builder);
}