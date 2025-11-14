namespace ScrubJay.Rendering;

[PublicAPI]
public delegate void ValueRenderer<in T>(T value, TextBuilder builder)
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
    ;