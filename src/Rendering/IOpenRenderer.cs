namespace ScrubJay.Rendering;

[PublicAPI]
public interface IOpenRenderer
{
    bool CanRender<T>()
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    ;

    void RenderTo<T>(T value, TextBuilder builder)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    ;
}



[PublicAPI]
public interface IValueRenderer<in T>
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
{
    void RenderTo(T value, TextBuilder builder);
}