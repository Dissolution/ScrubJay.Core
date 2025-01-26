namespace ScrubJay.Fluent;

/// <summary>
/// A base class implementation of the Builder Pattern
/// </summary>
/// <typeparam name="TBuilder"></typeparam>
[PublicAPI]
public abstract class FluentBuilder<TBuilder>
    where TBuilder : FluentBuilder<TBuilder>
{
    protected internal TBuilder _builder;

    protected FluentBuilder()
    {
        _builder = (TBuilder)this;
    }

    public virtual TBuilder Invoke(Action<TBuilder>? builderAction)
    {
        builderAction?.Invoke(_builder);
        return _builder;
    }

    public override string ToString() => typeof(TBuilder).NameOf();
}
