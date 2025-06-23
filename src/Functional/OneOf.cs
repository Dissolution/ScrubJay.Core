// using ScrubJay.Text.Rendering;
//
// namespace ScrubJay.Functional;
//
// public static class OneOf
// {
// }
//
// public readonly struct OneOf<T1> :
//     /* All listed interfaces are implemented, commented out ones cannot be declared because they 'may unify for some type parameter substitutions' */
// #if NET7_0_OR_GREATER
//     IEqualityOperators<OneOf<T1>, OneOf<T1>, bool>,
//     IEqualityOperators<OneOf<T1>, T1, bool>,
// #endif
//     IEquatable<OneOf<T1>>,
//     IEquatable<T1>,
//     IFormattable
// {
//     public static implicit operator OneOf<T1>(T1 first) => new OneOf<T1>(first);
//
//     public static bool operator ==(OneOf<T1> left, OneOf<T1> right) => left.Equals(right);
//
//     public static bool operator !=(OneOf<T1> left, OneOf<T1> right) => !left.Equals(right);
//
//     public static bool operator ==(OneOf<T1> left, T1? right) => left.Equals(right);
//
//     public static bool operator !=(OneOf<T1> left, T1? right) => !left.Equals(right);
//
//     public static OneOf<T1> Create(T1 first) => new OneOf<T1>(first);
//
//
//     private readonly int _arg;
//
//     private readonly T1? _first;
//
//     public OneOf(T1 first)
//     {
//         _arg = 1;
//         _first = first;
//     }
//
//     public bool Is([MaybeNullWhen(false)] out T1 first)
//     {
//         if (_arg == 1)
//         {
//             first = _first!;
//             return true;
//         }
//         else
//         {
//             first = default;
//             return false;
//         }
//     }
//
//     public void Match(Action<T1> onFirst)
//     {
//         if (_arg == 1)
//         {
//             onFirst(_first!);
//         }
//         else
//         {
//             throw new InvalidOperationException("Invalid Argument Type Index");
//         }
//     }
//
//     public R Match<R>(Fn<T1, R> onFirst)
//     {
//         if (_arg == 1)
//         {
//             return onFirst(_first!);
//         }
//         else
//         {
//             throw new InvalidOperationException("Invalid Argument Type Index");
//         }
//     }
//
//     public bool Equals(OneOf<T1> oneOf)
//     {
//         if (oneOf._arg != _arg)
//             return false;
//
//         if (_arg == 1)
//         {
//             return Equate.Values(oneOf._first, _first);
//         }
//         else
//         {
//             return false;
//         }
//     }
//
//     public bool Equals(T1? first)
//     {
//         return (_arg == 1) && Equate.Values(first, _first);
//     }
//
//     public override bool Equals([NotNullWhen(true)] object? obj)
//     {
//         if (obj is T1 first)
//             return Equals(first);
//         return false;
//     }
//
//     public override int GetHashCode()
//     {
//         if (_arg == 1)
//         {
//             return Hasher.HashMany(_arg, _first);
//         }
//         else
//         {
//             return Hasher.NullHash;
//         }
//     }
//
//     public string ToString(string? format, IFormatProvider? provider = null)
//     {
//         using var builder = TextBuilder.New
//             .Render(GetType())
//             .Append('.');
//
//         if (_arg == 1)
//         {
//             builder.Render(_first?.GetType())
//                 .Append('(')
//                 .Append(_first, format, provider)
//                 .Append(')');
//         }
//         else
//         {
//             builder.Append("INVALID");
//         }
//
//         return builder.ToString();
//     }
//
//     public override string ToString() => ToString(null, null);
// }
//
// [PublicAPI]
// [StructLayout(LayoutKind.Auto)]
// public readonly struct OneOf<T1, T2> :
//     /* All listed interfaces are implemented, commented out ones cannot be declared because they 'may unify for some type parameter substitutions' */
// #if NET7_0_OR_GREATER
//     IEqualityOperators<OneOf<T1, T2>, OneOf<T1, T2>, bool>,
//     IEqualityOperators<OneOf<T1, T2>, T1, bool>,
//     //IEqualityOperators<OneOf<T1, T2>, T2, bool>,
// #endif
//     IEquatable<OneOf<T1, T2>>,
//     IEquatable<T1>,
//     //IEquatable<T2>,
//     IFormattable
// {
//     public static implicit operator OneOf<T1, T2>(T1 first) => new OneOf<T1, T2>(first);
//     public static implicit operator OneOf<T1, T2>(T2 second) => new OneOf<T1, T2>(second);
//
//
//     public static bool operator ==(OneOf<T1, T2> left, OneOf<T1, T2> right) => left.Equals(right);
//     public static bool operator !=(OneOf<T1, T2> left, OneOf<T1, T2> right) => !left.Equals(right);
//     public static bool operator ==(OneOf<T1, T2> oneOf, T1? first) => oneOf.Equals(first);
//     public static bool operator !=(OneOf<T1, T2> oneOf, T1? first) => !oneOf.Equals(first);
//     public static bool operator ==(OneOf<T1, T2> oneOf, T2? second) => oneOf.Equals(second);
//     public static bool operator !=(OneOf<T1, T2> oneOf, T2? second) => !oneOf.Equals(second);
//
//
//     public static OneOf<T1, T2> Create(T1 first) => new OneOf<T1, T2>(first);
//     public static OneOf<T1, T2> Create(T2 second) => new OneOf<T1, T2>(second);
//
//
//     private readonly int _arg;
//     private readonly T1? _first;
//     private readonly T2? _second;
//
//     public OneOf(T1 first)
//     {
//         _arg = 1;
//         _first = first;
//         _second = default;
//     }
//
//     public OneOf(T2 second)
//     {
//         _arg = 2;
//         _first = default;
//         _second = second;
//     }
//
//     public bool Is([MaybeNullWhen(false)] out T1 first)
//     {
//         if (_arg == 1)
//         {
//             first = _first!;
//             return true;
//         }
//
//         first = default;
//         return false;
//     }
//
//     public bool Is([MaybeNullWhen(false)] out T2 second)
//     {
//         if (_arg == 2)
//         {
//             second = _second!;
//             return true;
//         }
//
//         second = default;
//         return false;
//     }
//
//     public void Match(Action<T1> onFirst, Action<T2> onSecond)
//     {
//         switch (_arg)
//         {
//             case 1:
//                 onFirst(_first!);
//                 break;
//             case 2:
//                 onSecond(_second!);
//                 break;
//             default:
//                 throw new InvalidOperationException("Invalid Argument Type Index");
//         }
//     }
//
//     public R Match<R>(Fn<T1, R> onFirst, Fn<T2, R> onSecond)
//     {
//         return _arg switch
//         {
//             1 => onFirst(_first!),
//             2 => onSecond(_second!),
//             _ => throw new InvalidOperationException("Invalid Argument Type Index"),
//         };
//     }
//
//     public bool Equals(OneOf<T1, T2> oneOf)
//     {
//         if (oneOf._arg != _arg)
//             return false;
//
//         return _arg switch
//         {
//             1 => Equate.Values(oneOf._first, _first),
//             2 => Equate.Values(oneOf._second, _second),
//             _ => false,
//         };
//     }
//
//     public bool Equals(T1? first)
//         => (_arg == 1) && Equate.Values(first, _first);
//
//     public bool Equals(T2? second)
//         => (_arg == 2) && Equate.Values(second, _second);
//
//     public override bool Equals([NotNullWhen(true)] object? obj)
//     {
//         return obj switch
//         {
//             T1 first => Equals(first),
//             T2 second => Equals(second),
//             _ => false,
//         };
//     }
//
//     public override int GetHashCode()
//     {
//         return _arg switch
//         {
//             1 => Hasher.HashMany(_arg, _first),
//             2 => Hasher.HashMany(_arg, _second),
//             _ => Hasher.NullHash,
//         };
//     }
//
//     public string ToString(string? format, IFormatProvider? provider = null)
//     {
//         using var builder = new TextBuilder();
//         builder.RenderType(this)
//             .Append('.');
//
//         switch (_arg)
//         {
//             case 1:
//                 builder.Append(_first?.GetType().Render())
//                     .Append('(')
//                     .Append(_first, format, provider)
//                     .Append(')');
//                 break;
//             case 2:
//                 builder.Append(_second?.GetType().Render())
//                     .Append('(')
//                     .Append(_second, format, provider)
//                     .Append(')');
//                 break;
//             default:
//                 builder.Append("INVALID");
//                 break;
//         }
//
//         return builder.ToString();
//     }
//
//     public override string ToString() => ToString(null, null);
// }
//
// [PublicAPI]
// [StructLayout(LayoutKind.Auto)]
// public readonly struct OneOf<T1, T2, T3> :
//     /* All listed interfaces are implemented, commented out ones cannot be declared because they 'may unify for some type parameter substitutions' */
// #if NET7_0_OR_GREATER
//     IEqualityOperators<OneOf<T1, T2, T3>, OneOf<T1, T2, T3>, bool>,
//     IEqualityOperators<OneOf<T1, T2, T3>, T1, bool>,
//     //IEqualityOperators<OneOf<T1, T2, T3>, T2, bool>,
//     //IEqualityOperators<OneOf<T1, T2, T3>, T3, bool>,
// #endif
//     IEquatable<OneOf<T1, T2, T3>>,
//     IEquatable<T1>,
//     //IEquatable<T2>,
//     //IEquatable<T3>,
//     IFormattable
// {
//     public static implicit operator OneOf<T1, T2, T3>(T1 first) => new OneOf<T1, T2, T3>(first);
//     public static implicit operator OneOf<T1, T2, T3>(T2 second) => new OneOf<T1, T2, T3>(second);
//     public static implicit operator OneOf<T1, T2, T3>(T3 third) => new OneOf<T1, T2, T3>(third);
//
//
//     public static bool operator ==(OneOf<T1, T2, T3> left, OneOf<T1, T2, T3> right) => left.Equals(right);
//     public static bool operator !=(OneOf<T1, T2, T3> left, OneOf<T1, T2, T3> right) => !left.Equals(right);
//     public static bool operator ==(OneOf<T1, T2, T3> oneOf, T1? first) => oneOf.Equals(first);
//     public static bool operator !=(OneOf<T1, T2, T3> oneOf, T1? first) => !oneOf.Equals(first);
//     public static bool operator ==(OneOf<T1, T2, T3> oneOf, T2? second) => oneOf.Equals(second);
//     public static bool operator !=(OneOf<T1, T2, T3> oneOf, T2? second) => !oneOf.Equals(second);
//     public static bool operator ==(OneOf<T1, T2, T3> oneOf, T3? third) => oneOf.Equals(third);
//     public static bool operator !=(OneOf<T1, T2, T3> oneOf, T3? third) => !oneOf.Equals(third);
//
//
//     public static OneOf<T1, T2, T3> Create(T1 first) => new OneOf<T1, T2, T3>(first);
//     public static OneOf<T1, T2, T3> Create(T2 second) => new OneOf<T1, T2, T3>(second);
//     public static OneOf<T1, T2, T3> Create(T3 third) => new OneOf<T1, T2, T3>(third);
//
//
//     private readonly int _arg;
//     private readonly T1? _first;
//     private readonly T2? _second;
//     private readonly T3? _third;
//
//     public OneOf(T1 first)
//     {
//         _arg = 1;
//         _first = first;
//         _second = default;
//         _third = default;
//     }
//
//     public OneOf(T2 second)
//     {
//         _arg = 2;
//         _first = default;
//         _second = second;
//         _third = default;
//     }
//
//     public OneOf(T3 third)
//     {
//         _arg = 2;
//         _first = default;
//         _second = default;
//         _third = third;
//     }
//
//     public bool Is([MaybeNullWhen(false)] out T1 first)
//     {
//         if (_arg == 1)
//         {
//             first = _first!;
//             return true;
//         }
//
//         first = default;
//         return false;
//     }
//
//     public bool Is([MaybeNullWhen(false)] out T2 second)
//     {
//         if (_arg == 2)
//         {
//             second = _second!;
//             return true;
//         }
//
//         second = default;
//         return false;
//     }
//
//     public bool Is([MaybeNullWhen(false)] out T3 third)
//     {
//         if (_arg == 2)
//         {
//             third = _third!;
//             return true;
//         }
//
//         third = default;
//         return false;
//     }
//
//     public void Match(Action<T1> onFirst, Action<T2> onSecond, Action<T3> onThird)
//     {
//         switch (_arg)
//         {
//             case 1:
//                 onFirst(_first!);
//                 break;
//             case 2:
//                 onSecond(_second!);
//                 break;
//             case 3:
//                 onThird(_third!);
//                 break;
//             default:
//                 throw new InvalidOperationException("Invalid Argument Type Index");
//         }
//     }
//
//     public R Match<R>(Fn<T1, R> onFirst, Fn<T2, R> onSecond, Fn<T3, R> onThird)
//     {
//         return _arg switch
//         {
//             1 => onFirst(_first!),
//             2 => onSecond(_second!),
//             3 => onThird(_third!),
//             _ => throw new InvalidOperationException("Invalid Argument Type Index"),
//         };
//     }
//
//     public bool Equals(OneOf<T1, T2, T3> oneOf)
//     {
//         if (oneOf._arg != _arg)
//             return false;
//
//         return _arg switch
//         {
//             1 => Equate.Values(oneOf._first, _first),
//             2 => Equate.Values(oneOf._second, _second),
//             3 => Equate.Values(oneOf._third, _third),
//             _ => false,
//         };
//     }
//
//     public bool Equals(T1? first)
//         => (_arg == 1) && Equate.Values(first, _first);
//
//     public bool Equals(T2? second)
//         => (_arg == 2) && Equate.Values(second, _second);
//
//     public bool Equals(T3? third)
//         => (_arg == 3) && Equate.Values(third, _third);
//
//     public override bool Equals([NotNullWhen(true)] object? obj)
//     {
//         return obj switch
//         {
//             T1 first => Equals(first),
//             T2 second => Equals(second),
//             T3 third => Equals(third),
//             _ => false,
//         };
//     }
//
//     public override int GetHashCode()
//     {
//         return _arg switch
//         {
//             1 => Hasher.HashMany(_arg, _first),
//             2 => Hasher.HashMany(_arg, _second),
//             3 => Hasher.HashMany(_arg, _third),
//             _ => Hasher.NullHash,
//         };
//     }
//
//     public string ToString(string? format, IFormatProvider? provider = null)
//     {
//         using var builder = TextBuilder.New
//             .RenderType(this)
//             .Append('.');
//
//         switch (_arg)
//         {
//             case 1:
//                 builder.Append(_first?.GetType().Render())
//                 .Append('(')
//                 .Append(_first, format, provider)
//                 .Append(')');
//                 break;
//             case 2:
//                 builder.Append(_second?.GetType().Render())
//                 .Append('(')
//                 .Append(_second, format, provider)
//                 .Append(')');
//                 break;
//             case 3:
//                 builder.Append(_third?.GetType().Render())
//                 .Append('(')
//                 .Append(_third, format, provider)
//                 .Append(')');
//                 break;
//             default:
//                 builder.Append("INVALID");
//                 break;
//         }
//
//         return builder.ToString();
//     }
//
//     public override string ToString() => ToString(null, null);
// }