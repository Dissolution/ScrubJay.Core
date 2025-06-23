using ScrubJay.Text.Rendering;

namespace ScrubJay.Building;

/// <summary>
/// A base class implementation of <see cref="IFluentBuilder{S}"/>
/// </summary>
/// <typeparam name="S">
/// The <see cref="Type"/> of the instance (self)
/// </typeparam>
[PublicAPI]
public abstract class FluentBuilderBase<S> : IFluentBuilder<S>
    where S : IFluentBuilder<S>
{
    /// <summary>
    /// Stores a reference to this builder instance as a <typeparamref name="S"/>
    /// </summary>
    protected S _builder;

    public S Self
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _builder;
    }

    protected FluentBuilderBase()
    {
        _builder = (S)(IFluentBuilder<S>)this;
    }

    public override string ToString() => typeof(S).Render();
}
