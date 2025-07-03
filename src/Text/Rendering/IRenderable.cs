namespace ScrubJay.Text.Rendering;

/// <summary>
/// Provides functionality for an instance to render itself to a <see cref="TextBuilder"/> instance
/// </summary>
[PublicAPI]
public interface IRenderable
{
    void RenderTo(TextBuilder builder);
}