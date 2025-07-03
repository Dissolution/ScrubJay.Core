namespace ScrubJay.Text.Rendering;

public abstract class Renderer
{
    public abstract bool CanRender(Type type);
}

public abstract class Renderer<T> : Renderer
{
    public override bool CanRender(Type type) => type.Implements<T>();

    public abstract void RenderTo(T? value, TextBuilder builder);
}