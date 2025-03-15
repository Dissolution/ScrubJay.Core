namespace ScrubJay.Comparison;

public interface ITextComparer :
#if NET9_0_OR_GREATER
    IComparer<ReadOnlySpan<char>>,
#endif
    IComparer<char>,
    IComparer<string>,
    IComparer<char[]>;