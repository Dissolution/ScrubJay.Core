namespace ScrubJay.Comparison;

public interface ITextEqualityComparer :
#if NET9_0_OR_GREATER
    IEqualityComparer<ReadOnlySpan<char>>,
#endif
    IEqualityComparer<char>,
    IEqualityComparer<string>,
    IEqualityComparer<char[]>;