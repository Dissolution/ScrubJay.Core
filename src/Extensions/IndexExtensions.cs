namespace ScrubJay.Extensions;

[PublicAPI]
public static class IndexExtensions
{
    extension(Index index)
    {
        public bool IsUnbounded
        {
            get
            {
                return index.Value == 0;
            }
        }

        public void Deconstruct(out bool isFromEnd, out int value)
        {
            isFromEnd = index.IsFromEnd;
            value = index.Value;
        }
    }
}