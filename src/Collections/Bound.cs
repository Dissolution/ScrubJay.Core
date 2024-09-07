namespace ScrubJay.Collections;

public static class Bound
{
    public static Bound<T> Inclusive<T>(T value) => new Bound<T>(value, true);
    public static Bound<T> Exclusive<T>(T value) => new Bound<T>(value, false);
}