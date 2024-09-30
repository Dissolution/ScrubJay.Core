using System.Linq.Expressions;

namespace ScrubJay.Expressions;

public record class ExpressionPair(Expression Left, Expression Right)
{
    public static ExpressionPair Create<T>(params T[] expressions)
        where T : Expression
    {
        Validate.ThrowIfNull(expressions);
        if (expressions.Length == 2)
            return new ExpressionPair(expressions[0], expressions[1]);
        throw new ArgumentException("There must be exactly two expressions", nameof(expressions));
    }
    
    public static ExpressionPair Create<T>(IEnumerable<T>? expressions)
        where T : Expression
    {
        Validate.ThrowIfNull(expressions);
        using var e = expressions.GetEnumerator();
        if (e.MoveNext())
        {
            var left = e.Current!;
            if (e.MoveNext())
            {
                var right = e.Current!;
                if (!e.MoveNext())
                {
                    return new ExpressionPair(left, right);
                }
            }
        }
        throw new ArgumentException("There must be exactly two expressions", nameof(expressions));
    }
}