namespace ScrubJay.Expressions;

public class LambdaBuilder : FluentLambdaBuilder<LambdaBuilder>
{
    public LambdaBuilder(Type delegateType) : base(delegateType)
    {
    }

    public LambdaBuilder(Type genericTypeDefinition, params Type[] genericTypes) : base(genericTypeDefinition, genericTypes)
    {
    }
}

public class LambdaBuilder<D> : FluentLambdaBuilder<LambdaBuilder<D>, D>
    where D : Delegate
{
    public LambdaBuilder() : base()
    {
    }
}

