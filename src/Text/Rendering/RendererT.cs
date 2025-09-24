namespace ScrubJay.Text.Rendering;

[PublicAPI]
public abstract class Renderer<T> : IRenderer<T>, IRenderer
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
{
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

    public abstract void RenderValue(TextBuilder builder, T value);
}