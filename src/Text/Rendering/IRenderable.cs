namespace ScrubJay.Text.Rendering;

/// <summary>
/// Indicates that instances of this type can Render themselves to a <see cref="TextBuilder"/>
/// </summary>
[PublicAPI]
public interface IRenderable
{
    /// <summary>
    /// Renders this instance to <paramref name="builder"/>
    /// </summary>
    /// <param name="builder">
    /// The <see cref="TextBuilder"/> that a rendered representation of this instance will be written to
    /// </param>
    void RenderTo(TextBuilder builder);
}