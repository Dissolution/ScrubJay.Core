namespace ScrubJay.Tests;

internal static  class Bad
{

    public sealed class BadClass
    {
        public override bool Equals(object? obj) => throw new NotSupportedException();

        public override int GetHashCode() => throw new NotSupportedException();

        public override string ToString() => throw new NotSupportedException();
    }
    
    public sealed class BadEqualityComparer<T> : IEqualityComparer<T>
    {
        public bool Equals(T? x, T? y) => throw new NotSupportedException();

        public int GetHashCode(T? obj) => throw new NotSupportedException();
    }
}