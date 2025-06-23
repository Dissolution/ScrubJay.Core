// #if NET7_0_OR_GREATER
// namespace ScrubJay.Functional;
//
// public readonly ref struct MutOption<T>
// {
//     public static implicit operator bool(MutOption<T> option) => option.IsSome();
//     public static implicit operator MutOption<T>(None _) => new();
//
//     public static MutOption<T> None() => new();
//
//     public static MutOption<T> Some(ref T value) => new(ref value);
//
//     private readonly ref T _value;
//
//
//     public MutOption()
//     {
//         _value = ref Notsafe.NullRef<T>();
//     }
//
//     public MutOption(ref T value)
//     {
//         _value = ref value;
//     }
//
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public bool IsNone()
//     {
//         return Notsafe.IsNullRef<T>(ref _value);
//     }
//
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public bool IsSome()
//     {
//         return Notsafe.IsNonNullRef<T>(ref _value);
//     }
//
//     public bool IsSome([MaybeNullWhen(false)] out T value)
//     {
//         if (IsSome())
//         {
//             value = _value;
//             return true;
//         }
//
//         value = default;
//         return false;
//     }
//
//     public void Match(ActionRef<T> onSome, Action onNone)
//     {
//         if (IsSome())
//         {
//             onSome.Invoke(ref _value);
//         }
//         else
//         {
//             onNone.Invoke();
//         }
//     }
//
//     public R Match<R>(FuncRef<T, R> onSome, Func<R> onNone)
//     {
//         if (IsSome())
//         {
//             return onSome.Invoke(ref _value);
//         }
//         else
//         {
//             return onNone.Invoke();
//         }
//     }
//
//     public bool Equals(T value, IEqualityComparer<T>? comparer = null)
//     {
//         if (IsSome())
//         {
//             return Equate.Values(_value, value, comparer);
//         }
//         else
//         {
//             return false;
//         }
//     }
//
//     public bool Equals(None _) => IsNone();
//
//     public string ToString(string? format, IFormatProvider? provider = null)
//     {
//         if (IsSome())
//         {
//             return TextBuilder.New.Append("Some(").Append(_value, format, provider).Append(')').ToStringAndDispose();
//         }
//
//         return "None";
//     }
//
//     public override string ToString() => ToString(null, null);
// }
//
// #endif