using System.Linq.Expressions;
using System.Reflection;

namespace ScrubJay.Expressions;

public class FluentBodyBuilder<B> : BuilderBase<B>
    where B : FluentBodyBuilder<B>
{
    public IReadOnlyList<ParameterExpression> Parameters { get; }

    internal FluentBodyBuilder(ParameterExpression[] parameters)
    {
        Parameters = parameters;
    }

    private Expression Resolve(ExprNode paramRef)
    {
        if (paramRef.IsIndex.IsSome(out var index))
        {
            Validate.Index(index, Parameters.Count).ThrowIfError();
            return Parameters[index];
        }
        else if (paramRef.IsName.IsSome(out var name))
        {
            var param = Parameters.OneOrDefault(p => TextHelper.Equate((string?)p.Name, name));
            if (param is not null)
                return param;
            throw new ArgumentException($"There is no parameter named \"{name}\"", nameof(paramRef));
        }
        else
        {
            throw new UnreachableException();
        }
    }

    public MethodCallExpression Call(MethodInfo staticMethod, params ExprNode[] parameters)
    {
        if (!staticMethod.IsStatic)
            throw new ArgumentException(null, nameof(staticMethod));
        var methodArgs = parameters.ConvertAll(Resolve);
        MethodCallExpression callExpr = Expression.Call(staticMethod, methodArgs);
        return callExpr;
    }

    public MethodCallExpression Call(MethodInfo staticMethod, IReadOnlyList<ParameterExpression> parameters)
    {
        if (!staticMethod.IsStatic)
            throw new ArgumentException(null, nameof(staticMethod));
        var methodArgs = parameters.OfType<Expression>().ToArray();
        MethodCallExpression callExpr = Expression.Call(staticMethod, methodArgs);
        return callExpr;
    }


    public MethodCallExpression Call(ExprNode instance, MethodInfo instanceMethod, params ExprNode[] args)
    {
        if (instanceMethod.IsStatic)
            throw new ArgumentException(null, nameof(instanceMethod));

        var methodArgs = args.Select(Resolve).ToArray();
        MethodCallExpression callExpr = Expression.Call(Resolve(instance), instanceMethod, methodArgs);
        return callExpr;
    }
}