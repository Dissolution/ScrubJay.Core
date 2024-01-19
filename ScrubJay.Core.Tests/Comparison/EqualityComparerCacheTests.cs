using ScrubJay.Comparison;

namespace ScrubJay.Tests.Comparison;

public class EqualityComparerCacheTests
{
    [Fact]
    public void CheckTEquals()
    {
        bool boolean = true;
        Assert.True(Relate.Equal.Values<bool>(boolean, true));
        Assert.False(Relate.Equal.Values<bool>(boolean, false));
        int int32 = 147;
        Assert.True(Relate.Equal.Values<int>(int32, 147));
        Assert.False(Relate.Equal.Values<int>(int32, 13));
        Guid guid = Guid.NewGuid();
        Assert.True(Relate.Equal.Values<Guid>(guid, guid));
        Assert.False(Relate.Equal.Values<Guid>(guid, Guid.NewGuid()));
        string nonNullString = "abc";
        Assert.True(Relate.Equal.Values<string>(nonNullString, "abc"));
        Assert.False(Relate.Equal.Values<string>(nonNullString, "ABC"));
        Assert.False(Relate.Equal.Values<string>(nonNullString, null));
        string? nullString = null;
        Assert.True(Relate.Equal.Values<string>(nullString, null));
        Assert.False(Relate.Equal.Values<string>(nullString, string.Empty));
        Assert.True(Relate.Equal.Values<string?>(nullString, null));
        Assert.False(Relate.Equal.Values<string?>(nullString, ""));
        DateTime dateTime = new DateTime(1955, 11, 25, 8, 0, 0);
        Assert.True(Relate.Equal.Values<DateTime>(dateTime, dateTime));
        Assert.False(Relate.Equal.Values<DateTime>(dateTime, DateTime.Now));
        ComplexEntity testClass = new(guid, nonNullString);
        Assert.True(Relate.Equal.Values<ComplexEntity>(testClass, testClass));
        Assert.True(Relate.Equal.Values<ComplexEntity>(testClass, new ComplexEntity(guid, "joe")));
        Assert.False(Relate.Equal.Values<ComplexEntity>(testClass, new ComplexEntity()));
        Assert.False(Relate.Equal.Values<ComplexEntity>(testClass, null));
    }
    
    [Fact]
    public void CheckObjectEquals()
    {
        bool boolean = true;
        Assert.True(Relate.Equal.Objects((object?)boolean, (object?)true));
        Assert.False(Relate.Equal.Objects((object?)boolean, (object?)false));
        int int32 = 147;
        Assert.True(Relate.Equal.Objects((object?)int32, (object?)147));
        Assert.False(Relate.Equal.Objects((object?)int32, (object?)13));
        Guid guid = Guid.NewGuid();
        Assert.True(Relate.Equal.Objects((object?)guid, (object?)guid));
        Assert.False(Relate.Equal.Objects((object?)guid, (object?)Guid.NewGuid()));
        string nonNullString = "abc";
        Assert.True(Relate.Equal.Objects((object?)nonNullString, (object?)"abc"));
        Assert.False(Relate.Equal.Objects((object?)nonNullString, (object?)"ABC"));
        Assert.False(Relate.Equal.Objects((object?)nonNullString, (object?)null));
        string? nullString = null;
        Assert.True(Relate.Equal.Objects((object?)nullString, (object?)null));
        Assert.False(Relate.Equal.Objects((object?)nullString, (object?)string.Empty));
        Assert.True(Relate.Equal.Objects((object?)nullString, (object?)null));
        Assert.False(Relate.Equal.Objects((object?)nullString, (object?)""));
        DateTime dateTime = new DateTime(1955, 11, 25, 8, 0, 0);
        Assert.True(Relate.Equal.Objects((object?)dateTime, (object?)dateTime));
        Assert.False(Relate.Equal.Objects((object?)dateTime, (object?)DateTime.Now));
        ComplexEntity testClass = new(guid, nonNullString);
        Assert.True(Relate.Equal.Objects((object?)testClass, (object?)testClass));
        Assert.True(Relate.Equal.Objects((object?)testClass, (object?)new ComplexEntity(guid, "joe")));
        Assert.False(Relate.Equal.Objects((object?)testClass, (object?)new ComplexEntity()));
        Assert.False(Relate.Equal.Objects((object?)testClass, (object?)null));
    }
}