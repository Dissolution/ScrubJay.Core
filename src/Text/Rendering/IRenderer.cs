namespace ScrubJay.Text.Rendering;

/// <summary>
///
/// </summary>
[PublicAPI]
public interface IRenderer
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    bool CanRender(Type type);

    /// <summary>
    ///
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="obj"></param>
    TextBuilder RenderObject(TextBuilder builder, object obj);
}