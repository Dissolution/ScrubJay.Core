using System.Linq.Expressions;

namespace ScrubJay.Expressions;

public sealed class BodyBuilder : FluentBodyBuilder<BodyBuilder>
{
    internal BodyBuilder(ParameterExpression[] parameters) : base(parameters)
    {
    }
}