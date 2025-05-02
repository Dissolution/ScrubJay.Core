namespace ScrubJay.Text.Rendering;

[PublicAPI]
public abstract class Renderer<T> : IRenderer<T>
{
    public virtual bool CanRender(Type type) => type.Implements<T>();

    public abstract void RenderTo<B>(T? value, B textBuilder)
        where B : TextBuilderBase<B>;
}