namespace ScrubJay.Maths;

/// <summary>
/// Indicates that this type is algebraically compatible with another type:<br/>
/// <c>==, !=, &lt;, &lt;=, &gt;, &gt;=, +, -, *, /, %</c>
/// </summary>
/// <typeparam name="S">The type of the Self</typeparam>
/// <typeparam name="O">The type of the Other</typeparam>
[PublicAPI]
public interface IAlgebraic<S, O> :
#if NET7_0_OR_GREATER
    IEqualityOperators<S, O, bool>,
    IComparisonOperators<S, O, bool>,
    IAdditionOperators<S, O, S>,
    ISubtractionOperators<S, O, S>,
    IMultiplyOperators<S, O, S>,
    IDivisionOperators<S, O, S>,
    IModulusOperators<S, O, S>,
#endif
    IEquatable<O>,
    IComparable<O>
    where S : IAlgebraic<S, O>;