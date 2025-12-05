namespace ScrubJay.Rendering;


[PublicAPI]
public interface IValueRenderer
{
    bool CanRender(Type valueType);
}

[PublicAPI]
public interface IValueRenderer<in T> : IValueRenderer
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
{
    void RenderTo(T value, TextBuilder builder);
}

[PublicAPI]
public abstract class ValueRendererBase<S, T> : IValueRenderer<T>, IHasDefault<S>
    where S : ValueRendererBase<S, T>, new()
{
    public static S Default { get; } = new S();

    public virtual bool CanRender(Type valueType)
    {
        return valueType.Implements<T>();
    }

    public abstract void RenderTo(T value, TextBuilder builder);
}