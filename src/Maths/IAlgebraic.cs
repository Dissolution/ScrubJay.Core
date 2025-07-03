namespace ScrubJay.Maths;

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