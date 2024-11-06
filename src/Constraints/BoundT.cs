#pragma warning disable MA0048

namespace ScrubJay.Constraints;

[PublicAPI]
public static class Bound
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bound<T> Inclusive<T>(T value) => new Bound<T>(value, true);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bound<T> Exclusive<T>(T value) => new Bound<T>(value, false);
}


[PublicAPI]
[StructLayout(LayoutKind.Auto)]
public readonly struct Bound<T> :
#if NET7_0_OR_GREATER
    IEqualityOperators<Bound<T>, Bound<T>, bool>,
#endif
    IEquatable<Bound<T>>
{
    public static bool operator ==(Bound<T> left, Bound<T> right) => left.Equals(right);
    public static bool operator !=(Bound<T> left, Bound<T> right) => !left.Equals(right);

    public static Bound<T> Inclusive(T value) => new Bound<T>(value, true);
    public static Bound<T> Exclusive(T value) => new Bound<T>(value, false);

    public readonly T Value;
    public readonly bool IsInclusive;

    public Bound(T value, bool isInclusive)
    {
        this.Value = value;
        this.IsInclusive = isInclusive;
    }

    public void Deconstruct(out T value, out bool isInclusive)
    {
        value = Value;
        isInclusive = IsInclusive;
    }

    public bool Equals(Bound<T> other)
    {
        return other.IsInclusive == this.IsInclusive && EqualityComparer<T>.Default.Equals(other.Value, this.Value);
    }

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Bound<T> bound && Equals(bound);

    public override int GetHashCode() => Hasher.Combine<T, bool>(Value, IsInclusive);

    public override string ToString() => IsInclusive ? $"[{Value}]" : $"({Value})";
}

[InterpolatedStringHandler]
public ref struct BoundBuilder<T>
{
    private enum Step
    {
        PreLowerIncMarker,
        PostLowerIncMarker,
        PostLowerValue,
        RangeSeparator,
        PostUpperValue,
        PreUpperIncMarker,
        PostUpperIncMarker,
    }


    private bool _hasLower = false;
    private bool _incLower = true;
    private T? _lower = default;


    private bool _hasUpper = false;
    private bool _incUpper = false;
    private T? _upper = default;


    private Step _parsedStep = 0;


    public Option<Bound<T>> Lower => !_hasLower ? None<Bound<T>>() : Some<Bound<T>>(new(_lower!, _incLower));

    public Option<Bound<T>> Upper => !_hasUpper ? None<Bound<T>>() : Some<Bound<T>>(new(_upper!, _incUpper));

    public Bounds<T> Bounds => new Bounds<T>(Lower, Upper);


    public BoundBuilder(int formattedLength, int argumentCount)
    {
        if (argumentCount is < 0 or > 2)
        {
            Debugger.Break();
            throw new InvalidOperationException();
        }

        if (formattedLength is < 2 or > 4)
        {
            Debugger.Break();
            throw new InvalidOperationException();
        }
    }


    public void AppendLiteral(string str)
    {
        if (str == "[")
        {
            if (_parsedStep != Step.PreLowerIncMarker)
                throw new InvalidOperationException();
            _incLower = true;
            _parsedStep = Step.PostLowerIncMarker;
        }
        else if (str == "(")
        {
            if (_parsedStep != Step.PreLowerIncMarker)
                throw new InvalidOperationException();
            _incLower = false;
            _parsedStep = Step.PostLowerIncMarker;
        }
        else if (str == "..")
        {
            if (_parsedStep is not (Step.PostLowerValue or Step.PreLowerIncMarker))
                Debugger.Break();
            _parsedStep = Step.RangeSeparator;
        }
        else if (str == "]")
        {
            if (_parsedStep != Step.PostUpperValue)
                throw new InvalidOperationException();
            _incUpper = true;
            _parsedStep = Step.PostUpperIncMarker;
        }
        else if (str == ")")
        {
            if (_parsedStep != Step.PostUpperValue)
                throw new InvalidOperationException();
            _incLower = false;
            _parsedStep = Step.PostUpperIncMarker;
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    public void AppendFormatted(T? value)
    {
        if (_parsedStep is Step.PostLowerIncMarker or Step.PreLowerIncMarker)
        {
            _lower = value;
            _hasLower = true;
            _parsedStep = Step.PostLowerValue;
        }
        else if (_parsedStep is Step.RangeSeparator or Step.PreUpperIncMarker)
        {
            _upper = value;
            _hasUpper = true;
            _parsedStep = Step.PostUpperValue;
        }
        else
        {
            throw new NotImplementedException();
        }
    }

}