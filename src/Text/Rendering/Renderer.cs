namespace ScrubJay.Text.Rendering;

[PublicAPI]
public interface IRenderable
{
    TextBuilder RenderTo(TextBuilder builder);
}

[PublicAPI]
public interface IRenderer
{
    public bool CanRender(Type type);

    public TextBuilder RenderTo(TextBuilder builder, object? obj);
}

[PublicAPI]
public abstract class Renderer : IRenderer
{
    public abstract bool CanRender(Type type);

    public abstract TextBuilder RenderTo(TextBuilder builder, object? obj);
}

[PublicAPI]
public interface IRenderer<in T> : IRenderer
{
    TextBuilder RenderTo(TextBuilder builder, T? value);

#if !NETFRAMEWORK && !NETSTANDARD2_0
    bool IRenderer.CanRender(Type type) => type.Implements<T>();

    TextBuilder IRenderer.RenderTo(TextBuilder builder, object? obj)
    {
        if (obj is T value)
        {
            return RenderTo(builder, value);
        }

        if (obj is null && typeof(T).CanContainNull())
        {
            return RenderTo(builder, default(T));
        }

        throw new ArgumentException($"Object `{obj}` is not a {typeof(T).Render()} value", nameof(obj));
    }

#endif
}

[PublicAPI]
public abstract class Renderer<T> : Renderer, IRenderer<T>
{
    public override bool CanRender(Type type)
    {
        return type.Implements<T>();
    }

    public override TextBuilder RenderTo(TextBuilder builder, object? obj)
    {
        if (obj is T value)
        {
            return RenderTo(builder, value);
        }

        if (obj is null && typeof(T).CanContainNull())
        {
            return RenderTo(builder, default(T));
        }

        throw new ArgumentException($"Object `{obj}` is not a {typeof(T).Render()} value", nameof(obj));
    }

    public abstract TextBuilder RenderTo(TextBuilder builder, T? value);
}