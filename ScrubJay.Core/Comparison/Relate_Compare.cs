using System.Reflection;
using ScrubJay.Collections;

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

        public static IComparer GetComparer(Type type)
        {
            return _comparers.GetOrAdd(type, static t => FindComparer(t));
        }

        public static IComparer<T> CreateComparer<T>(Func<T?, T?, int> compare)
            => Comparer<T>.Create((x, y) => compare(x, y));
        

        public static int Values<T>(T? left, T? right) => Comparer<T>.Default.Compare(left!, right!);

        public static int Objects(object? left, object? right)
        {
            if (ReferenceEquals(left, right)) return 0;
            if (left is not null)
            {
                return GetComparer(left.GetType()).Compare(left!, right!);
            }

            return GetComparer(right!.GetType()).Compare(left!, right!);
        }
    }
}