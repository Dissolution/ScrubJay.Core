using ScrubJay.Iteration;

namespace ScrubJay.Memory;

[PublicAPI]
public interface ISpanSplitIterator<T> : IIterator<Segment<T>>
{
    SplitOptions Options { get; }
}