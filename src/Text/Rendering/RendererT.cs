namespace ScrubJay.Text.Rendering;

[PublicAPI]
public abstract class Renderer<T> : IRenderer<T>, IRenderer
{
    public virtual bool CanRender(Type type)
    {
        return type.Implements<T>();
    }

    TextBuilder IRenderer.RenderObject(TextBuilder builder, object obj)
    {
        if (obj is T value)
        {
            return RenderValue(builder, value);
        }

        throw Ex.Arg(obj, $"Object `{obj:@}` is not a {typeof(T):@} value");
    }

    public abstract TextBuilder RenderValue(TextBuilder builder, T value);
}