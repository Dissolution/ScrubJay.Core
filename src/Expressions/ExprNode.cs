using System.Linq.Expressions;

namespace ScrubJay.Expressions;

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