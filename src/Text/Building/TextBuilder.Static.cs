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

    public static string Build(Action<TextBuilder>? build)
    {
        return New.Invoke(build).ToStringAndDispose();
    }

    public static string Build<R>(Func<TextBuilder, R>? buildOut)
#if NET9_0_OR_GREATER
        where R : allows ref struct
#endif
    {
        return New.Invoke(buildOut).ToStringAndDispose();
    }

    public static string Build<S>(S state, Action<TextBuilder, S>? buildState)
#if NET9_0_OR_GREATER
        where S : allows ref struct
#endif
    {
        return New.Invoke(state, buildState).ToStringAndDispose();
    }

    public static string Build<S, R>(S state, Func<TextBuilder, S, R>? buildStateOut)
    {
        if (buildStateOut is null)
            return string.Empty;
        return New.Invoke(state, buildStateOut).ToStringAndDispose();
    }

    public static string Build(InterpolatedTextBuilder interpolatedTextBuilder)
    {
        return interpolatedTextBuilder.ToStringAndClear();
    }
}