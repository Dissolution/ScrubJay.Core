using ScrubJay.Text.Rendering;

namespace ScrubJay.Text;

public partial class TextBuilder
{
    public TextBuilder Render<T>(T? value)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        Renderer.WriteTo(this, value);
        return this;
    }

#if !NET9_0_OR_GREATER
    public TextBuilder Render<T>(scoped ReadOnlySpan<T> span)
    {
        Renderer.RenderReadOnlySpanTo<T>(this, span);
        return this;
    }

    public TextBuilder Render<T>(scoped Span<T> span)
    {
        Renderer.RenderSpanTo<T>(this, span);
        return this;
    }

#endif


    public TextBuilder RenderLine<T>(T? value)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
        => Render<T>(value).NewLine();
}