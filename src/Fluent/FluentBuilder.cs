#pragma warning disable CA1715

namespace ScrubJay.Fluent;

/// <summary>
/// A base class implementation of the Builder Pattern
/// </summary>
/// <typeparam name="B"></typeparam>
[PublicAPI]
public abstract class FluentBuilder<B>
    where B : FluentBuilder<B>
{
    protected internal B _builder;

    protected FluentBuilder()
    {
        _builder = (B)this;
    }

    /// <summary>
    /// Executes a <see cref="Action{T}"/> upon this <see cref="FluentBuilder{B}"/>
    /// </summary>
    /// <param name="build">
    /// An <see cref="Action{T}"/> to invoke
    /// </param>
    /// <returns>
    /// A reference to this <see cref="FluentBuilder{B}"/> instance after execution
    /// </returns>
    public virtual B Execute(Action<B> build)
    {
        build.Invoke(_builder);
        return _builder;
    }

    public override string ToString()
    {
        return typeof(B).Name;
    }
}