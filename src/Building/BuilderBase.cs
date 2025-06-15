using ScrubJay.Text.Rendering;

namespace ScrubJay.Building;

/// <summary>
/// A base class implementation of the Builder Pattern
/// </summary>
/// <typeparam name="B">
/// The <see cref="Type"/> of the implementation of <see cref="BuilderBase{TBuilder}"/>
/// </typeparam>
[PublicAPI]
public abstract class BuilderBase<B> : IBuilder<B>
    where B : IBuilder<B>
{
    /// <summary>
    /// Stores a reference to this builder instance as a <typeparamref name="B"/>
    /// </summary>
    protected internal B _builder;

    /// <summary>
    /// Initiates a new <typeparamref name="B"/> instance by storing it in the <see cref="_builder"/> field
    /// </summary>
    protected BuilderBase()
    {
        _builder = (B)(IBuilder<B>)this;
    }

    public virtual B Invoke(Action<B>? instanceAction)
    {
        if (instanceAction is not null)
        {
            instanceAction(_builder);
        }
        return _builder;
    }

    public virtual B Invoke<S>(S state, Action<S, B>? stateInstanceAction)
    {
        if (stateInstanceAction is not null)
        {
            stateInstanceAction(state, _builder);
        }
        return _builder;
    }

    public virtual B Invoke<R>(Func<B, R>? instanceFunc)
    {
        if (instanceFunc is not null)
        {
            _ = instanceFunc(_builder);
        }
        return _builder;
    }

    public virtual B Invoke<S, R>(S state, Func<S, B, R>? instanceFunc)
    {
        if (instanceFunc is not null)
        {
            _ = instanceFunc(state, _builder);
        }
        return _builder;
    }

    /// <inheritdoc/>
    public override string ToString() => typeof(B).Render();
}
