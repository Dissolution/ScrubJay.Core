namespace ScrubJay.Building;

/// <summary>
/// A base class implementation of the Builder Pattern
/// </summary>
/// <typeparam name="B">
/// The <see cref="Type"/> of the implementation of <see cref="BuilderBase{TBuilder}"/>
/// </typeparam>
[PublicAPI]
public abstract class BuilderBase<B> : IBuilder<B>
    where B : BuilderBase<B>
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
        _builder = (B)this;
    }
    
    /// <inheritdoc/>
    public virtual B Invoke(Action<B>? instanceAction)
    {
        instanceAction?.Invoke(_builder);
        return _builder;
    }

    /// <inheritdoc/>
    public virtual B Invoke(Func<B, B>? instanceFluentFunc)
    {
        // Throwaway what func returns, we always return our _builder
        _ = instanceFluentFunc?.Invoke(_builder);
        return _builder;
    }

    /// <inheritdoc/>
    public override string ToString() => typeof(B).NameOf();
}
