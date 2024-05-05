using System.Diagnostics;
using System.Linq.Expressions;

namespace ScrubJay.Expressions;

public static class Express
{
    public static Expression<Func<T1, TResult>> Func<T1, TResult>(
        Expression<Func<ParameterExpression, Expression>> buildExpression)
    {
        var @params = buildExpression.Parameters;
        Debug.Assert(@params.Count == 1);

        var param = Express.Parameter<T1>(@params[0].Name);
        var body = buildExpression.Compile().Invoke(param);
        var lambda = Express.Lambda<System.Func<T1, TResult>>(body, param);
        return lambda;
    }
    
    public static Expression<Func<T1, T2, TResult>> Func<T1, T2, TResult>(
        Expression<Func<ParameterExpression, ParameterExpression, Expression>> buildExpression)
    {
        var @params = buildExpression.Parameters;
        Debug.Assert(@params.Count == 2);

        var leftParam = Express.Parameter<T1>(@params[0].Name);
        var rightParam = Express.Parameter<T2>(@params[1].Name);
        var body = buildExpression.Compile().Invoke(leftParam, rightParam);
        var lambda = Express.Lambda<System.Func<T1, T2, TResult>>(body, leftParam, rightParam);
        return lambda;
    }

    public static Expression<TDelegate> Lambda<TDelegate>(
        Expression body,
        params ParameterExpression[]? parameterExpressions)
        where TDelegate : Delegate
    {
        return Expression.Lambda<TDelegate>(body, parameterExpressions);
    }

    
    
    
    public static ExpressionPair Pair<TExpression>(params TExpression[] expressions)
        where TExpression : Expression
    {
        return ExpressionPair.Create(expressions);
    }

    public static ExpressionPair Pair(Expression left, Expression right)
    {
        return new ExpressionPair(left, right);
    }
    
    public static ExpressionPair Pair<TExpression>(IEnumerable<TExpression> expressions)
        where TExpression : Expression
    {
        return ExpressionPair.Create(expressions);
    }

    public static ParameterExpression Parameter(
        Type type,
        [CallerArgumentExpression(nameof(type))]
        string? parameterName = null)
    {
        return Expression.Parameter(type, parameterName);
    }
    
    public static ParameterExpression Parameter<T>(string? typeName = null)
    {
        return Expression.Parameter(typeof(T), typeName ?? nameof(T));
    }
    
    public static ParameterExpression[] Parameters(
        Type type, 
        int count,
        params string?[]? parameterNames)
    {
        if (count < 1)
            return Array.Empty<ParameterExpression>();
        var parameterExpressions = new ParameterExpression[count];
        for (var i = 0; i < count; i++)
        {
            if (parameterNames.TryGet(i).IsOk(out var name))
            {
                parameterExpressions[i] = Expression.Parameter(type, name);
            }
            else
            {
                parameterExpressions[i] = Expression.Parameter(type);
            }
        }
        return parameterExpressions;
    }


    public static ParameterExpression[] Parameters<T>(
        int count,
        params string?[]? parameterNames)
        => Parameters(typeof(T), count, parameterNames);

    
    
    
    
    
    public static UnaryExpression Convert(this Expression expression, Type type)
    {
        return Expression.Convert(expression, type);
    }

    public static UnaryExpression Convert<T>(this Expression expression)
    {
        return Convert(expression, typeof(T));
    }
    
    public static UnaryExpression OnesComplement(this Expression expression)
    {
        return Expression.OnesComplement(expression);
    }
    
    public static BinaryExpression And(this ExpressionPair expressions)
    {
        return Expression.And(expressions.Left, expressions.Right);
    }
    
    public static BinaryExpression Or(this ExpressionPair expressions)
    {
        return Expression.Or(expressions.Left, expressions.Right);
    }
    
    public static BinaryExpression ExclusiveOr(this ExpressionPair expressions)
    {
        return Expression.ExclusiveOr(expressions.Left, expressions.Right);
    }
}