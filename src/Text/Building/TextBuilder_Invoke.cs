namespace ScrubJay.Text;

partial class TextBuilder
{
#region Invoke

    public TextBuilder Invoke(Action<TextBuilder>? buildText)
    {
        if (buildText is not null)
        {
            buildText(this);
        }

        return this;
    }

    public TextBuilder Invoke<S>(S state, Action<TextBuilder, S>? buildText)
#if NET9_0_OR_GREATER
        where S : allows ref struct
#endif
    {
        if (buildText is not null)
        {
            buildText(this, state);
        }

        return this;
    }

    public TextBuilder Invoke<S>(S state, Action<S, TextBuilder>? buildText)
#if NET9_0_OR_GREATER
        where S : allows ref struct
#endif
    {
        if (buildText is not null)
        {
            buildText(state, this);
        }

        return this;
    }

    public TextBuilder Invoke<R>(Func<TextBuilder, R>? buildText)
#if NET9_0_OR_GREATER
        where R : allows ref struct
#endif

    {
        if (buildText is not null)
        {
            _ = buildText.Invoke(this);
        }

        return this;
    }

    public TextBuilder Invoke<R>(Func<TextBuilder, R>? buildText, [MaybeNull] out R result)
#if NET9_0_OR_GREATER
        where R : allows ref struct
#endif
    {
        if (buildText is not null)
        {

            result = buildText.Invoke(this);
            return this;
        }

        result = default;
        return this;
    }

    public TextBuilder Invoke<S, R>(S state, Func<TextBuilder, S, R>? buildText)
#if NET9_0_OR_GREATER
        where S : allows ref struct
        where R : allows ref struct
#endif
    {
        if (buildText is not null)
        {
            _ = buildText.Invoke(this, state);
        }

        return this;
    }

    public TextBuilder Invoke<S, R>(S state, Func<TextBuilder, S, R>? buildText, [MaybeNull] out R result)
#if NET9_0_OR_GREATER
        where S : allows ref struct
        where R : allows ref struct
#endif
    {
        if (buildText is not null)
        {

            result = buildText.Invoke(this, state);
            return this;
        }

        result = default;
        return this;
    }

#endregion


}