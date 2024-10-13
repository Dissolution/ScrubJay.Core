namespace ScrubJay.Fluent;

[PublicAPI]
public abstract class FluentBuilder<B>
    where B : FluentBuilder<B>
{
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

    public sealed override bool Equals(object? obj) => base.Equals(obj);

    public sealed override int GetHashCode() => base.GetHashCode();

    public override string ToString()
    {
        return typeof(B).Name;
    }
}