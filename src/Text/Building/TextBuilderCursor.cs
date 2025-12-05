namespace ScrubJay.Text;

// sits between, not on


public sealed class TextBuilderCursor
{
    private readonly TextBuilder _builder;
    private int _position; // index of the 'next' item

    public text PreviousText
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _builder.Written[.._position];
    }

    public text NextText
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _builder.Written[_position..];
    }

    public TextBuilderCursor(TextBuilder builder)
    {
        _builder = builder;
        _position = 0;
    }

    public bool MovePrevious()
    {
        int prev = _position - 1;
        if (prev < 0)
            return false;
        _position = prev;
        return true;
    }

    public bool MoveNext()
    {
        int next = _position + 1;
        if (next > _builder.Length)
            return false;
        _position = next;
        return true;
    }

    public bool RemoveNext(int count)
    {

        _builder.TryGetAndRemoveAt(new Range(_position, _position + count));
        return true;
    }
}