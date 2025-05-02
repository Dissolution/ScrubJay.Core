namespace ScrubJay.Text.Rendering;

[PublicAPI]
public interface IRenderer
{
    bool CanRender(Type type);
}

[PublicAPI]
public interface IRenderer<in T> :  IRenderer
{
    void RenderTo<B>(T? value, B textBuilder)
        where B : TextBuilderBase<B>;
}