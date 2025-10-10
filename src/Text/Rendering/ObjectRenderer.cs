namespace ScrubJay.Text.Rendering;

[PublicAPI]
[RendererPriority(-100)]
public sealed class ObjectRenderer : Renderer<object>
{
    public override bool CanRender(Type type) => type == typeof(object);

    public override TextBuilder RenderValue(TextBuilder builder, object obj)
    {
        Type type = obj.GetType();
        // prevent infinite recursion
        IRenderer? renderer = null;
        if (type != typeof(object))
        {
            renderer = Renderer.GetRenderer(type);
        }

        if (renderer is not null)
        {
            return renderer.RenderObject(builder, obj);
        }

        return Renderer.DefaultRenderValue(builder, obj);
    }
}