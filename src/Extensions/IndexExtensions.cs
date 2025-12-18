namespace ScrubJay.Extensions;

[PublicAPI]
public static class IndexExtensions
{
    extension(Index index)
    {
        /// <summary>
        /// Is this <see cref="Index"/> unbounded?
        /// </summary>
        public bool IsUnbounded => index.Value == 0;

        public void Deconstruct(out int value, out bool isFromEnd)
        {
            isFromEnd = index.IsFromEnd;
            value = index.Value;
        }
    }
}