namespace ScrubJay.Text.Rendering;

/// <summary>
///
/// </summary>
/// <typeparam name="T"></typeparam>
[PublicAPI]
public interface IRenderer<in T> : IRenderer
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="value"></param>
    void RenderValue(TextBuilder builder, T value);

#if !NETFRAMEWORK && !NETSTANDARD2_0
    bool IRenderer.CanRender(Type type) => type.Implements(typeof(T));

    void IRenderer.RenderObject(TextBuilder builder, object obj)
    {
        if (obj is T value)
        {
            RenderValue(builder, value);
        }
        else
        {
            throw new ArgumentException(Build($"Object `{obj:@}` is not a {typeof(T):@} value"), nameof(obj));
        }
    }

#endif
}