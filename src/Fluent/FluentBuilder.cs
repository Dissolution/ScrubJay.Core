namespace ScrubJay.Fluent;

/// <summary>
/// A base class implementation of the Builder Pattern
/// </summary>
/// <typeparam name="TBuilder">
/// The <see cref="Type"/> of the implementation of <see cref="FluentBuilder{TBuilder}"/>
/// </typeparam>
[PublicAPI]
public abstract class FluentBuilder<TBuilder>
    where TBuilder : FluentBuilder<TBuilder>
{
    protected internal TBuilder _builder;

    protected FluentBuilder()
    {
        _builder = (TBuilder)this;
    }

    /// <summary>
    /// Invoke an action on this <typeparamref name="TBuilder"/>
    /// </summary>
    /// <param name="build">
    /// The <see cref="Action{T}"/> to invoke on this <typeparamref name="TBuilder"/>
    /// </param>
    /// <returns>
    /// This <typeparamref name="TBuilder"/> instance after invoking <paramref name="build"/>
    /// </returns>
    public virtual TBuilder Invoke(Action<TBuilder>? build)
    {
        build?.Invoke(_builder);
        return _builder;
    }

    /// <summary>
    /// Invoke a function on this <typeparamref name="TBuilder"/>
    /// </summary>
    /// <param name="build">
    /// The <see cref="Func{T,T}"/> to invoke on this <typeparamref name="TBuilder"/>
    /// </param>
    /// <returns>
    /// This <typeparamref name="TBuilder"/> instance after invoking <paramref name="build"/>
    /// </returns>
    public virtual TBuilder Invoke(Func<TBuilder, TBuilder>? build)
    {
        // Throwaway what build returns, we always return _builder
        _ = build?.Invoke(_builder);
        return _builder;
    }

    public override string ToString() => typeof(TBuilder).NameOf();
}
