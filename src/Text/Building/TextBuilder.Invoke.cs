namespace ScrubJay.Text;

partial class TextBuilder
{
#region Invoke(action)

    /// <summary>
    /// Invokes an <see cref="Action{TextBuilder}"/> with this <see cref="TextBuilder"/> instance.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TextBuilder Invoke(Action<TextBuilder>? build)
    {
        if (build is not null)
        {
            build(this);
        }

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TextBuilder Invoke<S>(S state, Action<TextBuilder, S>? buildState)
#if NET9_0_OR_GREATER
        where S : allows ref struct
#endif
    {
        if (buildState is not null)
        {
            buildState(this, state);
        }

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TextBuilder Invoke<S>(S state, Action<S, TextBuilder>? buildState)
#if NET9_0_OR_GREATER
        where S : allows ref struct
#endif
    {
        if (buildState is not null)
        {
            buildState(state, this);
        }

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TextBuilder Invoke<S>(Action<TextBuilder, S>? buildState, S state)
#if NET9_0_OR_GREATER
        where S : allows ref struct
#endif
    {
        if (buildState is not null)
        {
            buildState(this, state);
        }

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TextBuilder Invoke<S>(Action<S, TextBuilder>? buildState, S state)
#if NET9_0_OR_GREATER
        where S : allows ref struct
#endif
    {
        if (buildState is not null)
        {
            buildState(state, this);
        }

        return this;
    }

#endregion /Invoke(action)

#region Invoke(func)

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TextBuilder Invoke<R>(Func<TextBuilder, R>? buildOut)
#if NET9_0_OR_GREATER
        where R : allows ref struct
#endif

    {
        if (buildOut is not null)
        {
            _ = buildOut(this);
        }

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TextBuilder Invoke<R>(Func<TextBuilder, R>? buildOut, out R? result)
#if NET9_0_OR_GREATER
        where R : allows ref struct
#endif
    {
        if (buildOut is not null)
        {
            result = buildOut(this);
            return this;
        }

        result = default;
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TextBuilder Invoke<S, R>(S state, Func<TextBuilder, S, R>? buildState)
#if NET9_0_OR_GREATER
        where S : allows ref struct
        where R : allows ref struct
#endif
    {
        if (buildState is not null)
        {
            _ = buildState(this, state);
        }

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TextBuilder Invoke<S, R>(S state, Func<S, TextBuilder, R>? buildState)
#if NET9_0_OR_GREATER
        where S : allows ref struct
        where R : allows ref struct
#endif
    {
        if (buildState is not null)
        {
            _ = buildState(state, this);
        }

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TextBuilder Invoke<S, R>(Func<TextBuilder, S, R>? buildState, S state)
#if NET9_0_OR_GREATER
        where S : allows ref struct
        where R : allows ref struct
#endif
    {
        if (buildState is not null)
        {
            _ = buildState(this, state);
        }

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TextBuilder Invoke<S, R>(Func<S, TextBuilder, R>? buildState, S state)
#if NET9_0_OR_GREATER
        where S : allows ref struct
        where R : allows ref struct
#endif
    {
        if (buildState is not null)
        {
            _ = buildState(state, this);
        }

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TextBuilder Invoke<S, R>(S state, Func<TextBuilder, S, R>? buildStateOut, out R? result)
#if NET9_0_OR_GREATER
        where S : allows ref struct
        where R : allows ref struct
#endif
    {
        if (buildStateOut is not null)
        {
            result = buildStateOut(this, state);
            return this;
        }

        result = default;
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TextBuilder Invoke<S, R>(S state, Func<S, TextBuilder, R>? buildStateOut, out R? result)
#if NET9_0_OR_GREATER
        where S : allows ref struct
        where R : allows ref struct
#endif
    {
        if (buildStateOut is not null)
        {
            result = buildStateOut(state, this);
            return this;
        }

        result = default;
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TextBuilder Invoke<S, R>(Func<TextBuilder, S, R>? buildStateOut, S state, out R? result)
#if NET9_0_OR_GREATER
        where S : allows ref struct
        where R : allows ref struct
#endif
    {
        if (buildStateOut is not null)
        {
            result = buildStateOut(this, state);
            return this;
        }

        result = default;
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TextBuilder Invoke<S, R>(Func<S, TextBuilder, R>? buildStateOut, S state, out R? result)
#if NET9_0_OR_GREATER
        where S : allows ref struct
        where R : allows ref struct
#endif
    {
        if (buildStateOut is not null)
        {
            result = buildStateOut(state, this);
            return this;
        }

        result = default;
        return this;
    }

#endregion /Invoke(func)
}