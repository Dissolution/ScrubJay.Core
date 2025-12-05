namespace ScrubJay.Text;

public partial class TextBuilder
{
    public TextBuilder Render<T>(T? value)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        Renderer.RenderTo<T>(value, this);
        return this;
    }

#if !NET9_0_OR_GREATER
    public TextBuilder Render<T>(scoped ReadOnlySpan<T> span)
    {
        Renderer.RenderReadOnlySpanTo<T>(span, this);
        return this;
    }

    public TextBuilder Render<T>(scoped Span<T> span)
    {
        Renderer.RenderSpanTo<T>(span, this);
        return this;
    }

#endif


    public TextBuilder RenderLine<T>(T? value)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
        => Render<T>(value).NewLine();
}