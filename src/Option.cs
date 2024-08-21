namespace ScrubJay;

public static class Option
{
    // None must live as a SubType in order to enable GlobalHelpers support for
    // `None` as a Field or Property (and not type name)
    
    [StructLayout(LayoutKind.Explicit, Size = 0)]
    public readonly struct None :
#if NET7_0_OR_GREATER
        IEqualityOperators<None, None, bool>,
        IBitwiseOperators<None, None, bool>,
#endif
        IEquatable<None>
    {
        // None is false
        public static implicit operator bool(None none) => false;

        public static bool operator true(None none) => false;
        public static bool operator false(None none) => true;

        public static bool operator &(None left, None right) => false;
        public static bool operator |(None left, None right) => false;
        public static bool operator ^(None left, None right) => false;

        [Obsolete("Cannot apply operator '~' to operand of type 'None'", true)]
        public static bool operator ~(None none) => throw new NotSupportedException();
        
        // All Nones are the same
        public static bool operator ==(None left, None right) => true;
        public static bool operator !=(None left, None right) => false;
        
        public bool Equals(None none) => true;
        public override bool Equals([NotNullWhen(true)] object? obj) => obj is None;
        public override int GetHashCode() => 0;
        public override string ToString() => nameof(None);
    }
    
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> NotNull<T>(T? value)
        where T : notnull
    {
        if (value is not null)
            return Option<T>.Some(value);
        return Option<T>.None;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> AsOption<T>(this T? nullable)
        where T : struct
    {
        if (nullable.HasValue)
            return Option<T>.Some(nullable.Value);
        return Option<T>.None;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Some<T>(T value) => Option<T>.Some(value);

}