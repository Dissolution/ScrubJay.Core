namespace ScrubJay.Text.Rendering;

[PublicAPI]
public abstract class Renderer<T> : IRenderer<T>, IRenderer
{
    public virtual bool CanRender(Type type)
    {
        return type.Implements(typeof(T));
    }

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