namespace ScrubJay.Tests.ExtensionsTests;

public class RangeExtensionsTests
{
    public static TheoryData<Range> Ranges { get; } = [];

    static RangeExtensionsTests()
    {
        Ranges.Add(Range.All);
        
        Span<Index> indices = [0, 10, ^1, ^10];
        
        foreach (Index index in indices)
        {
            Ranges.Add(Range.StartAt(index));
            Ranges.Add(Range.EndAt(index));
        }
        
        foreach (Index start in indices)
        foreach (Index end in indices)
        {
            Ranges.Add(new Range(start, end));
        }
    }
    
    // [Theory]
    // [MemberData(nameof(Ranges))]
    // public void GetLengthWorks(Range range)
    // {
    //     Option<int> length = RangeExtensions.GetLength(range);
    //
    //     int[] numbers = [0, 1, 4, 7, 8, 13, 147];
    //     Span<int> slice;
    //     try
    //     {
    //         slice = numbers.AsSpan()[range];
    //     }
    //     catch (Exception)
    //     {
    //         // If we could not slice it, it was an invalid range
    //         Assert.True(length.IsNone);
    //         return;
    //     }
    //
    //     if (!length.HasSome(out var len))
    //     {
    //         // If we didn't find a length, it had to be incalculable
    //         Assert.Equal(numbers.Length, slice.Length);
    //     }
    //     else
    //     {
    //         // Found length must be slice length
    //         Assert.Equal<int>(slice.Length, len);
    //     }
    // }

}