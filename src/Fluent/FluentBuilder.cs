namespace ScrubJay.Fluent;

/// <summary>
/// A base class implementation of the Builder Pattern
/// </summary>
/// <typeparam name="B"></typeparam>
[PublicAPI]
public abstract class FluentBuilder<B>
    where B : FluentBuilder<B>, new()
{
    public static B New => new B();
    
    protected B _builder;
    
    protected FluentBuilder()
    {
        _builder = (B)this;
    }

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