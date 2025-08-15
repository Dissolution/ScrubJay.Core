namespace ScrubJay.Text.Rendering;

[PublicAPI]
public interface IRenderer
{
    bool CanRender(Type? type);
}

[PublicAPI]
public interface IRenderer<in T> : IRenderer
{
    TextBuilder FluentRender(TextBuilder builder, T? value);
}

[PublicAPI]
public abstract class Renderer<T> : IRenderer<T>, IRenderer
{
    public virtual bool CanRender(Type? type)
    {
        return type.Implements<T>();
    }

    public abstract TextBuilder FluentRender(TextBuilder builder, T? value);
}