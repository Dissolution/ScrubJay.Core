using System.Buffers;

namespace ScrubJay.Text;

partial class TextBuilder
{
    private void GrowBy(int adding)
    {
        Debug.Assert(adding > 0);
        GrowTo(Capacity + (adding * 16));
    }

    private void GrowTo(int minCapacity)
    {
        Debug.Assert(minCapacity > Capacity);
        char[] array = ArrayPool<char>.Shared.Rent(Math.Max(minCapacity * 2, 1024));
        if (_chars.Length > 0)
        {
            Debug.Assert(_chars is not null);
            Written.CopyTo(array);
            ArrayPool<char>.Shared.Return(_chars, true);
        }

        _chars = array;
    }
}