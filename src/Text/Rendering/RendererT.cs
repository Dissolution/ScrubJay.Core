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
            throw new ArgumentException(Build($"Object `{obj:@}` is not a {typeof(T):@} value"), nameof(obj));
        }
    }

    public abstract void RenderValue(TextBuilder builder, T value);
}