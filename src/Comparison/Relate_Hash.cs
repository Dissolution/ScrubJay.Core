using ScrubJay.Utilities;

namespace ScrubJay.Comparison;

public static partial class Relate
{
    public static class Hash
    {
        public static int Value<T>(T? item)
        {
            return Hasher.GetHashCode<T>(item);
        }

        public static int Sequence<T>(T[]? items)
        {
            return Hasher.Combine<T>(items);
        }

        public static int Sequence<T>(Span<T> items)
        {
            return Hasher.Combine<T>(items);
        }

        public static int Sequence<T>(ReadOnlySpan<T> items)
        {
            return Hasher.Combine<T>(items);
        }

        public static int Sequence<T>(IEnumerable<T>? items)
        {
            return Hasher.Combine<T>(items);
        }

    }
}