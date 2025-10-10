using System.Buffers;

namespace ScrubJay.Text;

partial class TextBuilder
{
    private void GrowBy(int count)
    {
        Debug.Assert(count > 0);
        GrowTo(Capacity + (count * 16));
    }

    private void GrowTo(int minCapacity)
    {
        Debug.Assert(minCapacity > Capacity);
        char[] array = ArrayNest<char>.Rent(minCapacity);
        if (_position > 0)
        {
            Debug.Assert(_chars is not null);
            Notsafe.Text.CopyBlock(_chars!, array, _position);
            ArrayNest.Return(_chars, true);
        }

        _chars = array;
    }
}