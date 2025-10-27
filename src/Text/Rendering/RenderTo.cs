namespace ScrubJay.Text.Rendering;

[PublicAPI]
public delegate void RenderTo<in T>(TextBuilder builder, T value)
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
;