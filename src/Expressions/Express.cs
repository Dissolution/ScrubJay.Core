// Prefix generic type parameter with T, Rename type, Should override Equals
#pragma warning disable CA1715, CA1716, CA1815

using System.Linq.Expressions;
using System.Reflection;
using ScrubJay.Fluent;
using ScrubJay.Functional.Linq;

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

public class FluentLambdaBuilder<B> : FluentBuilder<B>
    where B : FluentLambdaBuilder<B>
{
    protected readonly Type _delegateType;

    protected readonly ParameterExpression[] _parameters;

    protected Expression? _body;


    public IReadOnlyList<ParameterExpression> Parameters => _parameters;

    public FluentLambdaBuilder(Type delegateType)
    {
        Validate.Implements(delegateType, typeof(Delegate)).ThrowIfError();
        _delegateType = delegateType;

        var genericTypes = delegateType.GetGenericArguments();
        if ((genericTypes.Length == 0) || (genericTypes[^1] == typeof(void)))
        {
            // action
            Debugger.Break();
            int count = genericTypes.Length;

            _parameters = new ParameterExpression[count];
            for (int i = 0; i < count; i++)
            {
                _parameters[i] = Expression.Parameter(genericTypes[0], $"arg{i}");
            }
        }
        else
        {
            // func
            Debugger.Break();
            //var returnType = genericTypes[^1];
            var paramTypes = genericTypes.AsSpan(..^1);
            int count = paramTypes.Length;

            _parameters = new ParameterExpression[count];
            for (int i = 0; i < count; i++)
            {
                _parameters[i] = Expression.Parameter(paramTypes[0], $"arg{i}");
            }
        }
        Debugger.Break();
    }

    public FluentLambdaBuilder(Type genericTypeDefinition, params Type[] genericTypes)
        : this(genericTypeDefinition.MakeGenericType(genericTypes))
    {

    }

    public B ParamName(Index index, string? name)
    {
        Validate.Index(index, _parameters.Length).ThrowIfError();
        ParameterExpression param = _parameters[index];
        _parameters[index] = Expression.Parameter(param.Type, name);
        return _builder;
    }

    public B ParamNames(params string?[] names)
    {
        int count = _parameters.Length;
        if (names.Length != count)
            throw new ArgumentException(null, nameof(names));
        for (int i = 0; i < count; i++)
        {
            ParameterExpression param = _parameters[i];
            _parameters[i] = Expression.Parameter(param.Type, names[i]);
        }
        return _builder;
    }

    public B Body(Func<BodyBuilder, Expression> createBody)
    {
        _body = createBody(new(_parameters));
        return _builder;
    }

    public Result<Delegate> TryCompile()
    {
        if (_body is null)
            return new InvalidOperationException("Body has not been set");

        try
        {
            var lambda = Expression.Lambda(_delegateType, _body, _parameters);
            var del = lambda.Compile();
            return Ok(del);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
}

public class LambdaBuilder<D> : FluentLambdaBuilder<LambdaBuilder<D>, D>
    where D : Delegate
{
    public LambdaBuilder() : base()
    {
    }
}

public class FluentLambdaBuilder<B, D> : FluentLambdaBuilder<B>
    where B : FluentLambdaBuilder<B, D>
    where D : Delegate
{
    public FluentLambdaBuilder() : base(typeof(D))
    {
    }

    public new Result<D> TryCompile()
    {
        return base.TryCompile()
            .Select(static del => del.Is<D>());
    }
}

public sealed class BodyBuilder : FluentBodyBuilder<BodyBuilder>
{
    internal BodyBuilder(ParameterExpression[] parameters) : base(parameters)
    {
    }
}

public readonly struct ExprNode
{


    public static implicit operator ExprNode(int index) => new(index);

    public static implicit operator ExprNode(string name) => new(name);

    public static implicit operator ExprNode(ParameterExpression parameter) => new(parameter);

    private readonly object? _obj;

    private ExprNode(object? obj)
    {
        _obj = obj;
    }

    public Option<int> IsIndex => _obj.As<int>();

    public Option<string> IsName => _obj.As<string>();

    public Option<ParameterExpression> IsParameter => _obj.As<ParameterExpression>();



}

public class FluentBodyBuilder<B> : FluentBuilder<B>
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
            var param = Parameters.OneOrDefault(p => TextHelper.Equate(p.Name, name));
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
