// ReSharper disable InvokeAsExtensionMethod

#pragma warning disable CA1822, IDE0062, CA1708

namespace ScrubJay.Comparison;

[PublicAPI]
public static class EquateExtensions
{
    extension(in char ch)
    {
        public bool Equate(in char other)
            => ch == other;

        public bool Equate(char[]? other)
            => EquateExtensions.Equate(ch.AsSpan(), other.AsSpan());

        public bool Equate(scoped text other)
            => EquateExtensions.Equate(ch.AsSpan(), other);

        public bool Equate(string? other)
            => EquateExtensions.Equate(ch.AsSpan(), other.AsSpan());

        public bool Equate(in char other, StringComparison comparison)
            => EquateExtensions.Equate(ch.AsSpan(), other.AsSpan(), comparison);

        public bool Equate(char[]? other, StringComparison comparison)
            => EquateExtensions.Equate(ch.AsSpan(), other.AsSpan(), comparison);

        public bool Equate(scoped text other, StringComparison comparison)
            => EquateExtensions.Equate(ch.AsSpan(), other, comparison);

        public bool Equate(string? other, StringComparison comparison)
            => EquateExtensions.Equate(ch.AsSpan(), other.AsSpan(), comparison);
    }

    extension(char[]? chars)
    {
        public bool Equate(in char other)
            => EquateExtensions.Equate(chars.AsSpan(), other.AsSpan());

        public bool Equate(char[]? other)
            => EquateExtensions.Equate(chars.AsSpan(), other.AsSpan());

        public bool Equate(scoped text other)
            => EquateExtensions.Equate(chars.AsSpan(), other);

        public bool Equate(string? other)
            => EquateExtensions.Equate(chars.AsSpan(), other.AsSpan());

        public bool Equate(in char other, StringComparison comparison)
            => EquateExtensions.Equate(chars.AsSpan(), other.AsSpan(), comparison);

        public bool Equate(char[]? other, StringComparison comparison)
            => EquateExtensions.Equate(chars.AsSpan(), other.AsSpan(), comparison);

        public bool Equate(scoped text other, StringComparison comparison)
            => EquateExtensions.Equate(chars.AsSpan(), other, comparison);

        public bool Equate(string? other, StringComparison comparison)
            => EquateExtensions.Equate(chars.AsSpan(), other.AsSpan(), comparison);
    }

    extension(scoped text text)
    {
        public bool Equate(in char other)
            => EquateExtensions.Equate(text, other.AsSpan());

        public bool Equate(char[]? other)
            => EquateExtensions.Equate(text, other.AsSpan());

        public bool Equate(scoped text other)
        {
            return MemoryExtensions.SequenceEqual<char>(text, other);
        }

        public bool Equate(string? other)
            => EquateExtensions.Equate(text, other.AsSpan());


        public bool Equate(in char other, StringComparison comparison)
            => EquateExtensions.Equate(text, other.AsSpan(), comparison);

        public bool Equate(char[]? other, StringComparison comparison)
            => EquateExtensions.Equate(text, other.AsSpan(), comparison);

        public bool Equate(scoped text other, StringComparison comparison)
        {
            return MemoryExtensions.Equals(text, other, comparison);
        }

        public bool Equate(string? other, StringComparison comparison)
            => EquateExtensions.Equate(text, other.AsSpan(), comparison);
    }

    extension(string? str)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equate(in char other)
        {
            return str is not null && str.Length == 1 && str[0] == other;
        }

        public bool Equate(char[]? other)
            => EquateExtensions.Equate(str.AsSpan(), other.AsSpan());

        public bool Equate(scoped text other)
            => MemoryExtensions.SequenceEqual(str.AsSpan(), other);

        public bool Equate(string? other)
            => string.Equals(str, other);

        public bool Equate(in char other, StringComparison comparison)
            => EquateExtensions.Equate(str.AsSpan(), other.AsSpan(), comparison);

        public bool Equate(char[]? other, StringComparison comparison)
            => EquateExtensions.Equate(str.AsSpan(), other.AsSpan(), comparison);

        public bool Equate(scoped text other, StringComparison comparison)
            => EquateExtensions.Equate(str.AsSpan(), other, comparison);

        public bool Equate(string? other, StringComparison comparison)
            => EquateExtensions.Equate(str.AsSpan(), other.AsSpan(), comparison);
    }

    extension<T>(T? value)
    {
        public bool Equate(T? other)
        {
            return EqualityComparer<T>.Default.Equals(value!, other!);
        }

        public bool Equate(T? other, IEqualityComparer<T>? comparer)
        {
            return (comparer ?? EqualityComparer<T>.Default).Equals(value!, other!);
        }
    }

    extension(object? obj)
    {
        public bool Equate(object? other)
        {
            return ObjectRelater.Default.Equate(obj, other);
        }

        public bool Equate(object? other, IEqualityComparer? comparer)
        {
            return (comparer ?? ObjectRelater.Default).Equals(obj, other);
        }
    }
}