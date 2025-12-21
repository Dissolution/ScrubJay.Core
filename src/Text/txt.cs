namespace ScrubJay.Text;

[PublicAPI]
public readonly ref struct txt
{
    public static implicit operator txt(in char ch) => new txt(in ch);
    public static implicit operator txt(string? str) => new txt(str);
    public static implicit operator txt(text text) => new txt(text);

#if NET7_0_OR_GREATER
    public static implicit operator text(txt txt)
    {
        unsafe
        {
            return new ReadOnlySpan<char>(
                Notsafe.RefAsVoidPtr(ref txt._firstChar),
                txt._length);
        }
    }

    private readonly ref char _firstChar;
    private readonly int _length;

    public bool IsEmpty => _length == 0;

    public txt(in char ch)
    {
        _firstChar = ref Notsafe.InAsRef(in ch);
        _length = 1;
    }

    public txt(string? str)
    {
        if (str is not null)
        {
            _firstChar = ref Notsafe.InAsRef(in str.GetPinnableReference());
            _length = str.Length;
        }
        else
        {
            _firstChar = ref Notsafe.NullRef<char>();
            _length = 0;
        }
    }

    public txt(text text)
    {
        _firstChar = ref Notsafe.InAsRef(in text.GetPinnableReference());
        _length = text.Length;
    }

    public override string ToString()
    {
        unsafe
        {
            return new string(Notsafe.RefAsPtr<char>(ref _firstChar), 0, _length);
        }
    }
#else
    public static implicit operator text(txt txt) => txt._text;

    private readonly text _text;

    public bool IsEmpty => _text.IsEmpty;

    public txt(in char ch)
    {
        _text = ch.AsSpan();
    }

    public txt(string? str)
    {
        _text = str.AsSpan();
    }

    public txt(text text)
    {
        _text = text;
    }

    public override string ToString()
    {
        return _text.AsString();
    }
#endif
}