namespace ScrubJay.Text.Rendering;

/// <summary>
///
/// </summary>
/// <typeparam name="T"></typeparam>
[PublicAPI]
public interface IRenderer<in T> : IRenderer
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="value"></param>
    void RenderValue(TextBuilder builder, T value);

#if !NETFRAMEWORK && !NETSTANDARD2_0
    bool IRenderer.CanRender(Type type) => type.Implements(typeof(T));

    void IRenderer.RenderObject(TextBuilder builder, object obj)
    {
        if (obj is T value)
        {
            RenderValue(builder, value);
        }
        else
        {
            throw Ex.Arg(obj, $"Object `{obj:@}` is not a {typeof(T):@} value");
        }
    }

#endif
}