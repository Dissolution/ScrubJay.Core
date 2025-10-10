// ReSharper disable InvokeAsExtensionMethod

#pragma warning disable CA1822,CA1708

namespace ScrubJay.Comparison;

[PublicAPI]
public static class CompareExtensions
{
    extension(in char ch)
    {
        public int Compare(in char other)
            => ch.CompareTo(other);

        public int Compare(char[]? other)
            => CompareExtensions.Compare(ch.AsSpan(), other.AsSpan());

        public int Compare(scoped text other)
            => CompareExtensions.Compare(ch.AsSpan(), other);

        public int Compare(string? other)
            => CompareExtensions.Compare(ch.AsSpan(), other.AsSpan());

        public int Compare(in char other, StringComparison comparison)
            => CompareExtensions.Compare(ch.AsSpan(), other.AsSpan(), comparison);

        public int Compare(char[]? other, StringComparison comparison)
            => CompareExtensions.Compare(ch.AsSpan(), other.AsSpan(), comparison);

        public int Compare(scoped text other, StringComparison comparison)
            => CompareExtensions.Compare(ch.AsSpan(), other, comparison);

        public int Compare(string? other, StringComparison comparison)
            => CompareExtensions.Compare(ch.AsSpan(), other.AsSpan(), comparison);
    }

    extension(char[]? chars)
    {
        public int Compare(in char other)
            => CompareExtensions.Compare(chars.AsSpan(), other.AsSpan());

        public int Compare(char[]? other)
            => CompareExtensions.Compare(chars.AsSpan(), other.AsSpan());

        public int Compare(scoped text other)
            => CompareExtensions.Compare(chars.AsSpan(), other);

        public int Compare(string? other)
            => CompareExtensions.Compare(chars.AsSpan(), other.AsSpan());

        public int Compare(in char other, StringComparison comparison)
            => CompareExtensions.Compare(chars.AsSpan(), other.AsSpan(), comparison);

        public int Compare(char[]? other, StringComparison comparison)
            => CompareExtensions.Compare(chars.AsSpan(), other.AsSpan(), comparison);

        public int Compare(scoped text other, StringComparison comparison)
            => CompareExtensions.Compare(chars.AsSpan(), other, comparison);

        public int Compare(string? other, StringComparison comparison)
            => CompareExtensions.Compare(chars.AsSpan(), other.AsSpan(), comparison);
    }

    extension(scoped text text)
    {
        public int Compare(in char other)
            => CompareExtensions.Compare(text, other.AsSpan());

        public int Compare(char[]? other)
            => CompareExtensions.Compare(text, other.AsSpan());

        public int Compare(scoped text other)
        {
            return MemoryExtensions.SequenceCompareTo<char>(text, other);
        }

        public int Compare(string? other)
            => CompareExtensions.Compare(text, other.AsSpan());


        public int Compare(in char other, StringComparison comparison)
            => CompareExtensions.Compare(text, other.AsSpan(), comparison);

        public int Compare(char[]? other, StringComparison comparison)
            => CompareExtensions.Compare(text, other.AsSpan(), comparison);

        public int Compare(scoped text other, StringComparison comparison)
        {
            return MemoryExtensions.CompareTo(text, other, comparison);
        }

        public int Compare(string? other, StringComparison comparison)
            => CompareExtensions.Compare(text, other.AsSpan(), comparison);
    }

    extension(string? str)
    {
        public int Compare(in char other)
            => CompareExtensions.Compare(str.AsSpan(), other.AsSpan());

        public int Compare(char[]? other)
            => CompareExtensions.Compare(str.AsSpan(), other.AsSpan());

        public int Compare(scoped text other)
            => CompareExtensions.Compare(str.AsSpan(), other);

        public int Compare(string? other)
            => CompareExtensions.Compare(str.AsSpan(), other.AsSpan());

        public int Compare(in char other, StringComparison comparison)
            => CompareExtensions.Compare(str.AsSpan(), other.AsSpan(), comparison);

        public int Compare(char[]? other, StringComparison comparison)
            => CompareExtensions.Compare(str.AsSpan(), other.AsSpan(), comparison);

        public int Compare(scoped text other, StringComparison comparison)
            => CompareExtensions.Compare(str.AsSpan(), other, comparison);

        public int Compare(string? other, StringComparison comparison)
            => CompareExtensions.Compare(str.AsSpan(), other.AsSpan(), comparison);
    }

    extension<T>(T? value)
    {
        public int Compare(T? other)
        {
            return Comparer<T>.Default.Compare(value!, other!);
        }

        public int Compare(T? other, IComparer<T>? comparer)
        {
            return (comparer ?? Comparer<T>.Default).Compare(value!, other!);
        }
    }

    extension(object? obj)
    {
        public int Compare(object? other)
        {
            return ObjectRelater.Default.Compare(obj, other);
        }

        public int Compare(object? other, IComparer? comparer)
        {
            return (comparer ?? ObjectRelater.Default).Compare(obj, other);
        }
    }
}