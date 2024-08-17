global using OkExResult = ScrubJay.Result<ScrubJay.Result.Ok, System.Exception>;

namespace ScrubJay;

public static class Result
{
    [StructLayout(LayoutKind.Explicit, Size = 0)]
    public readonly struct Ok :
#if NET7_0_OR_GREATER
        IEqualityOperators<Ok, Ok, bool>,
        IBitwiseOperators<Ok, Ok, bool>,
#endif
        IEquatable<Ok>
    {
        // Ok is True
        public static implicit operator bool(Ok ok) => true;
        
        public static bool operator true(Ok ok) => true;
        public static bool operator false(Ok ok) => false;

        public static bool operator &(Ok left, Ok right) => true;
        public static bool operator |(Ok left, Ok right) => true;
        public static bool operator ^(Ok left, Ok right) => false;

        [Obsolete("Cannot apply operator '~' to operand of type 'Ok'", true)]
        public static bool operator ~(Ok ok) => throw new NotSupportedException();

        // All Okays are the same
        public static bool operator ==(Ok left, Ok right) => true;
        public static bool operator !=(Ok left, Ok right) => false;

        public bool Equals(Ok ok) => true;
        public override bool Equals(object? obj) => obj is Ok;
        public override int GetHashCode() => 1;
        public override string ToString() => nameof(Ok);
    }

}