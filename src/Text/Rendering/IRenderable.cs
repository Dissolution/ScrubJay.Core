namespace ScrubJay.Text.Rendering;

/// <summary>
/// Indicates that instances of this type can Render themselves to a <see cref="TextBuilder"/>
/// </summary>
[PublicAPI]
public interface IRenderable
{
    /// <summary>
    /// Fluently render this instance to the given <see cref="TextBuilder"/>
    /// </summary>
    TextBuilder RenderTo(TextBuilder builder);
}