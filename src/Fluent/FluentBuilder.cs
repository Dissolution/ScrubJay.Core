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
    /// <summary>
    /// An <see cref="Action{T}">Action&lt;TBuilder&gt;</see>
    /// </summary>
    public delegate void BuilderAction(TBuilder builder);

    /// <summary>
    /// A fluent <see cref="Func{T,T}">Func&lt;TBuilder, TBuilder&gt;</see>
    /// </summary>
    public delegate TBuilder FluentBuild(TBuilder builder);


    protected internal TBuilder _builder;

    protected FluentBuilder()
    {
        _builder = (TBuilder)this;
    }

    /// <summary>
    /// Invoke an <see cref="BuilderAction"/> on this <typeparamref name="TBuilder"/>
    /// </summary>
    /// <param name="build">
    /// The <see cref="BuilderAction"/> to invoke on this <typeparamref name="TBuilder"/>
    /// </param>
    /// <returns>
    /// This <typeparamref name="TBuilder"/> instance after invoking <paramref name="build"/>
    /// </returns>
    public virtual TBuilder Invoke(BuilderAction? build)
    {
        build?.Invoke(_builder);
        return _builder;
    }

    /// <summary>
    /// Invoke an <see cref="FluentBuild"/> on this <typeparamref name="TBuilder"/>
    /// </summary>
    /// <param name="build">
    /// The <see cref="FluentBuild"/> to invoke on this <typeparamref name="TBuilder"/>
    /// </param>
    /// <returns>
    /// This <typeparamref name="TBuilder"/> instance after invoking <paramref name="build"/>
    /// </returns>
    public virtual TBuilder Invoke(FluentBuild? build)
    {
        // Throwaway what build returns, we always return _builder
        _ = build?.Invoke(_builder);
        return _builder;
    }

    public override string ToString() => typeof(TBuilder).NameOf();
}
