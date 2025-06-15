using System.Linq.Expressions;
using ScrubJay.Text.Rendering;

namespace ScrubJay.Expressions;

public readonly struct ExprNode :
#if NET7_0_OR_GREATER
    IEqualityOperators<ExprNode, ExprNode, bool>,
#endif
    IEquatable<ExprNode>
{
    public static bool operator ==(ExprNode left, ExprNode right) => left.Equals(right);
    public static bool operator !=(ExprNode left, ExprNode right) => !left.Equals(right);

    public static implicit operator ExprNode(int index) => new(index);

    public static implicit operator ExprNode(string name) => new(name);

    public static implicit operator ExprNode(ParameterExpression parameter) => new(parameter);

    private readonly object? _obj;

    private ExprNode(object? obj)
    {
        _obj = obj;
    }

    public Option<int> IsIndex => _obj.Is<int>();

    public Option<string> IsName => _obj.Is<string>();

    public Option<ParameterExpression> IsParameter => _obj.Is<ParameterExpression>();


    public bool Equals(ExprNode other)
        => ObjectComparer.Default.Equals(_obj, other._obj);

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is ExprNode node && Equals(node);

    public override int GetHashCode()
        => Hasher.Hash(_obj);

    public override string ToString()
    {
        if (_obj is int index)
            return $"[{index}]";
        if (_obj is string name)
            return $"\"{name}\"";
        if (_obj is ParameterExpression parameter)
        {
            var type = parameter.IsByRef ? parameter.Type.MakeByRefType() : parameter.Type;
            return $"{type.Render()} {parameter.Name}";
        }
        return _obj?.ToString() ?? "null";
    }
}
