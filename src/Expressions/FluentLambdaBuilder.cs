using System.Linq.Expressions;
using ScrubJay.Building;

namespace ScrubJay.Expressions;

public class FluentLambdaBuilder<B> : BuilderBase<B>
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
