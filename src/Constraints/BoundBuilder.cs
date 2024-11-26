#pragma warning disable MA0025


namespace ScrubJay.Constraints;

[StructLayout(LayoutKind.Auto)]
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


    private Step _parsedStep = default(Step);


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