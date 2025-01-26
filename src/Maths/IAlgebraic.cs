namespace ScrubJay.Maths;

public interface IAlgebraic<TSelf, TOther> :
#if NET7_0_OR_GREATER
    IEqualityOperators<TSelf, TOther, bool>,
    IComparisonOperators<TSelf, TOther, bool>,
    IAdditionOperators<TSelf, TOther, TSelf>,
    ISubtractionOperators<TSelf, TOther, TSelf>,
    IMultiplyOperators<TSelf, TOther, TSelf>,
    IDivisionOperators<TSelf, TOther, TSelf>,
    IModulusOperators<TSelf, TOther, TSelf>,
#endif
    IEquatable<TOther>,
    IComparable<TOther>
    where TSelf : IAlgebraic<TSelf, TOther>;