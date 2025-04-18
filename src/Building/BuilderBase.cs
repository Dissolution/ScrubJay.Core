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
    protected internal B _builder;

    protected BuilderBase()
    {
        _builder = (B)this;
    }

    /// <summary>
    /// Invoke an action on this <typeparamref name="B"/>
    /// </summary>
    /// <param name="build">
    /// The <see cref="Action{T}"/> to invoke on this <typeparamref name="B"/>
    /// </param>
    /// <returns>
    /// This <typeparamref name="B"/> instance after invoking <paramref name="build"/>
    /// </returns>
    public virtual B Invoke(Action<B>? build)
    {
        build?.Invoke(_builder);
        return _builder;
    }

    /// <summary>
    /// Invoke a function on this <typeparamref name="B"/>
    /// </summary>
    /// <param name="build">
    /// The <see cref="Func{T,T}"/> to invoke on this <typeparamref name="B"/>
    /// </param>
    /// <returns>
    /// This <typeparamref name="B"/> instance after invoking <paramref name="build"/>
    /// </returns>
    public virtual B Invoke(Func<B, B>? build)
    {
        // Throwaway what build returns, we always return our _builder
        _ = build?.Invoke(_builder);
        return _builder;
    }

    public override string ToString() => typeof(B).NameOf();
}
