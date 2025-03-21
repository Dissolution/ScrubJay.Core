// using ScrubJay.Parsing;
//
// namespace ScrubJay.Enums;
//
// public interface IEnumlike<S> :
// #if NET7_0_OR_GREATER
//     IEqualityOperators<IEnumlike<S>, S, bool>,
//     IComparisonOperators<IEnumlike<S>, S, bool>,
// #endif
//     ITrySpanParsable<S>,
//     IEquatable<S>,
//     IComparable<S>
//     where S : IEnumlike<S>
// {
// #if NET7_0_OR_GREATER
//     static Result<S, ParseException> ITrySpanParsable<S>.TryParse(text text, IFormatProvider? provider)
//         => throw new NotImplementedException();
//
//     static bool IEqualityOperators<IEnumlike<S>, S, bool>.operator ==(IEnumlike<S>? left, S? right)
//     {
//         if (left is null)
//             return right is null;
//         return left.Equals(right);
//     }
//
//     static bool IEqualityOperators<IEnumlike<S>, S, bool>.operator !=(IEnumlike<S>? left, S? right)
//     {
//         if (left is null)
//             return right is not null;
//         return !left.Equals(right);
//     }
//
//     static bool IComparisonOperators<IEnumlike<S>, S, bool>.operator >(IEnumlike<S> left, S right)
//         => left.CompareTo(right) > 0;
//
//     static bool IComparisonOperators<IEnumlike<S>, S, bool>.operator >=(IEnumlike<S> left, S right)
//         => left.CompareTo(right) >= 0;
//
//     static bool IComparisonOperators<IEnumlike<S>, S, bool>.operator <(IEnumlike<S> left, S right)
//         => left.CompareTo(right) < 0;
//
//     static bool IComparisonOperators<IEnumlike<S>, S, bool>.operator <=(IEnumlike<S> left, S right)
//         => left.CompareTo(right) <= 0;
// #endif
// }
