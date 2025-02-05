#if NETFRAMEWORK
using ScrubJay.Utilities;
#endif
using ScrubJay.Validation;

namespace ScrubJay.Tests.ValidationTests;

public class ValidateTests
{
    public static TheoryData<int, int> IntIndexLengths { get; } = new();
    public static TheoryData<Index, int> IndexLengths { get; } = new();
    public static TheoryData<Range> Ranges { get; } = new();

    public static int[] TestArray { get; } = [0, 1, 2, 3];

    static ValidateTests()
    {
        for (var index = -1; index <= 5; index++)
        for (var length = -1; length <= 5; length++)
        {
            IntIndexLengths.Add(index, length);
            if (index >= 0)
            {
                IndexLengths.Add(new Index(index, false), length);
                IndexLengths.Add(new Index(index, true), length);
            }
        }

        for (int index = 0; index <= 5; index++)
        {
            Ranges.Add(Range.StartAt(new Index(index, false)));
            Ranges.Add(Range.StartAt(new Index(index, true)));
            Ranges.Add(Range.EndAt(new Index(index, false)));
            Ranges.Add(Range.EndAt(new Index(index, true)));
        }

        for (int start = 0; start <= 5; start++)
        for (int end = 0; end <= 5; end++)
        {
            Ranges.Add(new Range(new Index(start, false), new Index(end, false)));
            Ranges.Add(new Range(new Index(start, false), new Index(end, true)));
            Ranges.Add(new Range(new Index(start, true), new Index(end, false)));
            Ranges.Add(new Range(new Index(start, true), new Index(end, true)));
        }
    }


    [Theory]
    [MemberData(nameof(IntIndexLengths))]
    public void ValidateIntIndexLengthWorks(int index, int length)
    {
        ReadOnlySpan<int> items = TestArray;

        var validationResult = Validate.IndexLength(index, length, items.Length);

        ReadOnlySpan<int> slice = [];

        Exception? tryEx = null;
        try
        {
            slice = items.Slice(index, length);
        }
        catch (Exception ex)
        {
            tryEx = ex;
        }

        if (validationResult.HasOk(out var offsetLength))
        {
            Assert.Null(tryEx);
            var thisSlice = items.Slice(offsetLength.Offset, offsetLength.Length);
#if NETFRAMEWORK
            Assert.True(Sequence.Equal<int>(slice, thisSlice));
#else
            Assert.Equal<int>(slice, thisSlice);
#endif
        }
        else
        {
            Assert.NotNull(tryEx);
        }
    }

    [Theory]
    [MemberData(nameof(IndexLengths))]
    public void ValidateIndexLengthWorks(Index index, int length)
    {
        ReadOnlySpan<int> items = TestArray;

        var validationResult = Validate.IndexLength(index, length, items.Length);

        int offset = index.GetOffset(items.Length);

        ReadOnlySpan<int> slice = [];

        Exception? tryEx = null;
        try
        {
            slice = items.Slice(offset, length);
        }
        catch (Exception ex)
        {
            tryEx = ex;
        }

        if (validationResult.HasOk(out var offsetLength))
        {
            Assert.Null(tryEx);
            var thisSlice = items.Slice(offsetLength.Offset, offsetLength.Length);
#if NETFRAMEWORK
            Assert.True(Sequence.Equal<int>(slice, thisSlice));
#else
            Assert.Equal<int>(slice, thisSlice);
#endif
        }
        else
        {
            Assert.NotNull(tryEx);
        }
    }

    [Theory]
    [MemberData(nameof(Ranges))]
    public void ValidateRangeWorks(Range range)
    {
        ReadOnlySpan<int> items = TestArray;

        var validationResult = Validate.Range(range, items.Length);

        ReadOnlySpan<int> slice = [];

        Exception? tryEx = null;
        try
        {
            slice = items[range];
        }
        catch (Exception ex)
        {
            tryEx = ex;
        }

        if (validationResult.HasOk(out var offsetLength))
        {
            Assert.Null(tryEx);
            var thisSlice = items.Slice(offsetLength.Offset, offsetLength.Length);
#if NETFRAMEWORK
            Assert.True(Sequence.Equal<int>(slice, thisSlice));
#else
            Assert.Equal<int>(slice, thisSlice);
#endif
        }
        else
        {
            Assert.NotNull(tryEx);
        }
    }
}
