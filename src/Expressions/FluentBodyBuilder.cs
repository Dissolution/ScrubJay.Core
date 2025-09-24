using System.Linq.Expressions;
using System.Reflection;

namespace ScrubJay.Expressions;

public class FluentBodyBuilder<B>
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
            var param = Parameters.OneOrDefault(p => ((string?)p.Name).Equate(name));
            if (param is not null)
                return param;
            throw Ex.Arg(paramRef, $"There is no parameter named \"{name}\"");
        }
        else
        {
            throw new UnreachableException();
        }
    }

    public MethodCallExpression Call(MethodInfo staticMethod, params ExprNode[] parameters)
    {
        if (!staticMethod.IsStatic)
            throw Ex.Arg(staticMethod);
        var methodArgs = parameters.ConvertAll(Resolve);
        MethodCallExpression callExpr = Expression.Call(staticMethod, methodArgs);
        return callExpr;
    }

    public MethodCallExpression Call(MethodInfo staticMethod, IReadOnlyList<ParameterExpression> parameters)
    {
        if (!staticMethod.IsStatic)
            throw Ex.Arg(staticMethod);
        var methodArgs = parameters.OfType<Expression>().ToArray();
        MethodCallExpression callExpr = Expression.Call(staticMethod, methodArgs);
        return callExpr;
    }


    public MethodCallExpression Call(ExprNode instance, MethodInfo instanceMethod, params ExprNode[] args)
    {
        if (instanceMethod.IsStatic)
            throw Ex.Arg(instanceMethod);

        var methodArgs = args.Select(Resolve).ToArray();
        MethodCallExpression callExpr = Expression.Call(Resolve(instance), instanceMethod, methodArgs);
        return callExpr;
    }
}