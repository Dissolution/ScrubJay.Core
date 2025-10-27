namespace ScrubJay.Text;

public partial class TextBuilder
{
    /// <summary>
    /// Creates a <c>new</c> <see cref="TextBuilder"/> instance
    /// </summary>
    public static TextBuilder New
    {
        [MustDisposeResource(true)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new();
    }

    public static string Build(Action<TextBuilder> buildText)
    {
        return New.Invoke(buildText).ToStringAndDispose();
    }

    public static string Build<R>(Func<TextBuilder, R> buildText)
#if NET9_0_OR_GREATER
        where R : allows ref struct
#endif
    {
        return New.Invoke(buildText).ToStringAndDispose();
    }

    public static string Build<S>(S state, Action<TextBuilder, S> buildText)
#if NET9_0_OR_GREATER
        where S : allows ref struct
#endif
    {
        using var builder = new TextBuilder();
        buildText(builder, state);
        return builder.ToString();
    }

    public static string Build<S, R>(S state, Func<TextBuilder, S, R>? buildStatefulText)
    {
        if (buildStatefulText is null)
            return string.Empty;
        using var builder = new TextBuilder();
        _ = buildStatefulText(builder, state);
        return builder.ToString();
    }

    public static string Build(InterpolatedTextBuilder interpolatedTextBuilder)
    {
        return interpolatedTextBuilder.ToStringAndDispose();
    }
}