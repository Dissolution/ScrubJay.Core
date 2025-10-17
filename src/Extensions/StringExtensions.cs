namespace ScrubJay.Extensions;

[PublicAPI]
public static class StringExtensions
{
    extension([NotNullWhen(false)] string? str)
    {
        public bool IsNull
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => str is null;
        }

        public bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => string.IsNullOrEmpty(str);
        }

        public bool IsWhiteSpace
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => string.IsNullOrWhiteSpace(str);
        }
    }
}