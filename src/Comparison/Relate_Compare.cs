using System.Reflection;
using ScrubJay.Collections;
using ScrubJay.Utilities;
using ScrubJay.Validation;
// ReSharper disable MethodOverloadWithOptionalParameter
// ReSharper disable InvokeAsExtensionMethod

namespace ScrubJay.Comparison;

public static partial class Relate
{
    public static class Compare
    {
        private static readonly ConcurrentTypeMap<IComparer> _comparers = new();

        private static IComparer FindComparer(Type type)
        {
            return typeof(Comparer<>)
                .MakeGenericType(type)
                .GetProperty("Default", BindingFlags.Public | BindingFlags.Static)
                .ThrowIfNull()
                .GetValue(null)
                .AsValid<IComparer>();
        }

        public static IComparer GetDefaultComparer(Type type)
        {
            return _comparers.GetOrAdd(type, static t => FindComparer(t));
        }

        public static IComparer<T> GetDefaultComparer<T>()
        {
            return _comparers.GetOrAdd<T>(static t => FindComparer(t)).AsValid<IComparer<T>>();
        }

        public static IComparer<T> CreateComparer<T>(Func<T?, T?, int> compare)
            => Comparer<T>.Create((x, y) => compare(x, y));


        public static int Values<T>(T? left, T? right) => Comparer<T>.Default.Compare(left!, right!);

        public static int Objects(object? left, object? right)
        {
            if (ReferenceEquals(left, right)) return 0;
            if (left is not null)
            {
                return GetDefaultComparer(left.GetType()).Compare(left!, right!);
            }

            return GetDefaultComparer(right!.GetType()).Compare(left!, right!);
        }

#region Sequence
#region IEquatable
        public static int Sequence<T>(T[]? left, T[]? right, Constraints.IsComparable<T> _ = default)
            where T : IComparable<T>
        {
            return MemoryExtensions.SequenceCompareTo<T>(left.AsSpan(), right.AsSpan());
        }

        public static int Sequence<T>(Span<T> left, T[]? right, Constraints.IsComparable<T> _ = default)
            where T : IComparable<T>
        {
            return MemoryExtensions.SequenceCompareTo<T>(left, right.AsSpan());
        }

        public static int Sequence<T>(ReadOnlySpan<T> left, T[]? right, Constraints.IsComparable<T> _ = default)
            where T : IComparable<T>
        {
            return MemoryExtensions.SequenceCompareTo<T>(left, right.AsSpan());
        }

        public static int Sequence<T>(T[]? left, Span<T> right, Constraints.IsComparable<T> _ = default)
            where T : IComparable<T>
        {
            return MemoryExtensions.SequenceCompareTo<T>(left.AsSpan(), right);
        }

        public static int Sequence<T>(Span<T> left, Span<T> right, Constraints.IsComparable<T> _ = default)
            where T : IComparable<T>
        {
            return MemoryExtensions.SequenceCompareTo<T>(left, right);
        }

        public static int Sequence<T>(ReadOnlySpan<T> left, Span<T> right, Constraints.IsComparable<T> _ = default)
            where T : IComparable<T>
        {
            return MemoryExtensions.SequenceCompareTo<T>(left, right);
        }

        public static int Sequence<T>(T[]? left, ReadOnlySpan<T> right, Constraints.IsComparable<T> _ = default)
            where T : IComparable<T>
        {
            return MemoryExtensions.SequenceCompareTo<T>(left.AsSpan(), right);
        }

        public static int Sequence<T>(Span<T> left, ReadOnlySpan<T> right, Constraints.IsComparable<T> _ = default)
            where T : IComparable<T>
        {
            return MemoryExtensions.SequenceCompareTo<T>(left, right);
        }

        public static int Sequence<T>(ReadOnlySpan<T> left, ReadOnlySpan<T> right, Constraints.IsComparable<T> _ = default)
            where T : IComparable<T>
        {
            return MemoryExtensions.SequenceCompareTo<T>(left, right);
        }
#endregion

#region Open
        public static int Sequence<T>(T[]? left, T[]? right)
        {
            return SpanExtensions.SequenceCompareTo<T>(left.AsSpan(), right.AsSpan());
        }

        public static int Sequence<T>(Span<T> left, T[]? right)
        {
            return SpanExtensions.SequenceCompareTo<T>(left, right.AsSpan());
        }

        public static int Sequence<T>(ReadOnlySpan<T> left, T[]? right)
        {
            return SpanExtensions.SequenceCompareTo<T>(left, right.AsSpan());
        }

        public static int Sequence<T>(T[]? left, Span<T> right)
        {
            return SpanExtensions.SequenceCompareTo<T>(left.AsSpan(), right);
        }

        public static int Sequence<T>(Span<T> left, Span<T> right)
        {
            return SpanExtensions.SequenceCompareTo<T>(left, right);
        }

        public static int Sequence<T>(ReadOnlySpan<T> left, Span<T> right)
        {
            return SpanExtensions.SequenceCompareTo<T>(left, right);
        }

        public static int Sequence<T>(T[]? left, ReadOnlySpan<T> right)
        {
            return SpanExtensions.SequenceCompareTo<T>(left.AsSpan(), right);
        }

        public static int Sequence<T>(Span<T> left, ReadOnlySpan<T> right)
        {
            return SpanExtensions.SequenceCompareTo<T>(left, right);
        }

        public static int Sequence<T>(ReadOnlySpan<T> left, ReadOnlySpan<T> right)
        {
            return SpanExtensions.SequenceCompareTo<T>(left, right);
        }
#endregion

#region Open w/EqualityComparer
        public static int Sequence<T>(T[]? left, T[]? right, IComparer<T>? comparer)
        {
            return SpanExtensions.SequenceCompareTo<T>(left.AsSpan(), right.AsSpan(), comparer);
        }

        public static int Sequence<T>(Span<T> left, T[]? right, IComparer<T>? comparer)
        {
            return SpanExtensions.SequenceCompareTo<T>(left, right.AsSpan(), comparer);
        }

        public static int Sequence<T>(ReadOnlySpan<T> left, T[]? right, IComparer<T>? comparer)
        {
            return SpanExtensions.SequenceCompareTo<T>(left, right.AsSpan(), comparer);
        }

        public static int Sequence<T>(T[]? left, Span<T> right, IComparer<T>? comparer)
        {
            return SpanExtensions.SequenceCompareTo<T>(left.AsSpan(), right, comparer);
        }

        public static int Sequence<T>(Span<T> left, Span<T> right, IComparer<T>? comparer)
        {
            return SpanExtensions.SequenceCompareTo<T>(left, right, comparer);
        }

        public static int Sequence<T>(ReadOnlySpan<T> left, Span<T> right, IComparer<T>? comparer)
        {
            return SpanExtensions.SequenceCompareTo<T>(left, right, comparer);
        }

        public static int Sequence<T>(T[]? left, ReadOnlySpan<T> right, IComparer<T>? comparer)
        {
            return SpanExtensions.SequenceCompareTo<T>(left.AsSpan(), right, comparer);
        }

        public static int Sequence<T>(Span<T> left, ReadOnlySpan<T> right, IComparer<T>? comparer)
        {
            return SpanExtensions.SequenceCompareTo<T>(left, right, comparer);
        }

        public static int Sequence<T>(ReadOnlySpan<T> left, ReadOnlySpan<T> right, IComparer<T>? comparer)
        {
            return SpanExtensions.SequenceCompareTo<T>(left, right, comparer);
        }
#endregion

#region IEnumerable
        public static int Sequence<T>(T[]? left, IEnumerable<T>? right, IComparer<T>? comparer = default)
            => SpanExtensions.SequenceCompareTo<T>(left, right, comparer);

        public static int Sequence<T>(Span<T> left, IEnumerable<T>? right, IComparer<T>? comparer = default)
            => SpanExtensions.SequenceCompareTo<T>(left, right, comparer);

        public static int Sequence<T>(ReadOnlySpan<T> left, IEnumerable<T>? right, IComparer<T>? comparer = default)
            => SpanExtensions.SequenceCompareTo<T>(left, right, comparer);

        public static int Sequence<T>(IEnumerable<T>? left, T[]? right, IComparer<T>? comparer = default)
            => SpanExtensions.SequenceCompareTo<T>(right, left, comparer);

        public static int Sequence<T>(IEnumerable<T>? left, Span<T> right, IComparer<T>? comparer = default)
            => SpanExtensions.SequenceCompareTo<T>(right, left, comparer);

        public static int Sequence<T>(IEnumerable<T>? left, ReadOnlySpan<T> right, IComparer<T>? comparer = default)
            => SpanExtensions.SequenceCompareTo<T>(right, left, comparer);
#endregion
#endregion

#region Text
        public static int Text(string? left, string? right)
        {
            return string.CompareOrdinal(left, right);
        }

        public static int Text(string? left, ReadOnlySpan<char> right)
        {
            return MemoryExtensions.SequenceCompareTo<char>(left.AsSpan(), right);
        }

        public static int Text(string? left, char[]? right)
        {
            return MemoryExtensions.SequenceCompareTo<char>(left.AsSpan(), right);
        }

        public static int Text(ReadOnlySpan<char> left, string? right)
        {
            return MemoryExtensions.SequenceCompareTo<char>(left, right.AsSpan());
        }

        public static int Text(ReadOnlySpan<char> left, ReadOnlySpan<char> right)
        {
            return MemoryExtensions.SequenceCompareTo<char>(left, right);
        }

        public static int Text(ReadOnlySpan<char> left, char[]? right)
        {
            return MemoryExtensions.SequenceCompareTo<char>(left, right);
        }

        public static int Text(char[]? left, string? right)
        {
            return MemoryExtensions.SequenceCompareTo<char>(left, right.AsSpan());
        }

        public static int Text(char[]? left, ReadOnlySpan<char> right)
        {
            return MemoryExtensions.SequenceCompareTo<char>(left, right);
        }

        public static int Text(char[]? left, char[]? right)
        {
            return MemoryExtensions.SequenceCompareTo<char>(left, right);
        }
#endregion

#region Text w/StringComparison
        public static int Text(string? left, string? right, StringComparison comparison)
        {
            return string.Compare(left, right, comparison);
        }

        public static int Text(string? left, ReadOnlySpan<char> right, StringComparison comparison)
        {
            return MemoryExtensions.CompareTo(left.AsSpan(), right, comparison);
        }

        public static int Text(string? left, char[]? right, StringComparison comparison)
        {
            return MemoryExtensions.CompareTo(left.AsSpan(), right, comparison);
        }

        public static int Text(ReadOnlySpan<char> left, string? right, StringComparison comparison)
        {
            return MemoryExtensions.CompareTo(left, right.AsSpan(), comparison);
        }

        public static int Text(ReadOnlySpan<char> left, ReadOnlySpan<char> right, StringComparison comparison)
        {
            return MemoryExtensions.CompareTo(left, right, comparison);
        }

        public static int Text(ReadOnlySpan<char> left, char[]? right, StringComparison comparison)
        {
            return MemoryExtensions.CompareTo(left, right, comparison);
        }

        public static int Text(char[]? left, string? right, StringComparison comparison)
        {
            return MemoryExtensions.CompareTo(left, right.AsSpan(), comparison);
        }

        public static int Text(char[]? left, ReadOnlySpan<char> right, StringComparison comparison)
        {
            return MemoryExtensions.CompareTo(left, right, comparison);
        }

        public static int Text(char[]? left, char[]? right, StringComparison comparison)
        {
            return MemoryExtensions.CompareTo(left, right, comparison);
        }
#endregion
    }
}