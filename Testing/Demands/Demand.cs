using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;

namespace ScrubJay.Functional.Tests;



public static class Demand
{
    public static CapturedValue<T> That<T>(
        T value,
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        return new CapturedValue<T>(value, valueName);
    }
    
    public static void Equal<T>(T left,
        T right,
        [CallerArgumentExpression(nameof(left))]
        string? leftName = null,
        [CallerArgumentExpression(nameof(right))]
        string? rightName = null)
    {
        if (EqualityComparer<T>.Default.Equals(left, right))
            return;
        throw new DemandException<T, T>(
            That<T>(left, leftName),
            That<T>(right, rightName),
            "supposed to be equal");
    }

    public static void NotEqual<T>(
        T left,
        T right,
        string? info = null,
        [CallerArgumentExpression(nameof(left))]
        string? leftName = null,
        [CallerArgumentExpression(nameof(right))]
        string? rightName = null)
    {
        if (!EqualityComparer<T>.Default.Equals(left, right))
            return;
        throw new DemandException<T, T>(
            That<T>(left, leftName),
            That<T>(right, rightName),
            info ?? "not supposed to be equal");
    }
}

