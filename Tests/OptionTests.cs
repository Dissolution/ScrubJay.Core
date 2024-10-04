using ScrubJay.Functional;

namespace ScrubJay.Tests;

public class OptionTests
{
    public static TheoryData<Option<int>> Options { get; } =
    [
        Option<int>.None(),
        Option<int>.Some(147),
    ];

    [Theory]
    [MemberData(nameof(Options))]
    public void EnumerationWorks(Option<int> option)
    {
        if (option.IsSome(out int some))
        {
            var e = option.GetEnumerator();
            Assert.NotNull(e);
            Assert.True(e.MoveNext());
            Assert.Equal(some, e.Current);
            Assert.False(e.MoveNext());
        }
        else
        {
            var e = option.GetEnumerator();
            Assert.NotNull(e);
            Assert.False(e.MoveNext());
        }
        
        
        bool hasValue = false;
        
        foreach (int i in option)
        {
            Assert.True(option.IsSome(out some));
            Assert.Equal(some, i);
            hasValue = true;
        }

        Assert.Equal(option.IsSome(), hasValue);
    }
}