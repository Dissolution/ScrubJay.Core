using ScrubJay.Enums;
using static ScrubJay.Tests.Enums.EnumTestData;

// ReSharper disable InvokeAsExtensionMethod

namespace ScrubJay.Tests.Enums;

public class EnumExtensionsTests
{
    public static IEnumerable<object[]> EnumTestData(int paramCount)
    {
        var enumTypes = new Type[]
        {
            typeof(NonFlagged.ESbyte),
            typeof(NonFlagged.EByte),
            typeof(NonFlagged.EShort),
            typeof(NonFlagged.EUshort),
            typeof(NonFlagged.EInt),
            typeof(NonFlagged.EUint),
            typeof(NonFlagged.ELong),
            typeof(NonFlagged.EUlong),

            typeof(Flagged.EfSbyte),
            typeof(Flagged.EfByte),
            typeof(Flagged.EfShort),
            typeof(Flagged.EfUshort),
            typeof(Flagged.EfInt),
            typeof(Flagged.EfUint),
            typeof(Flagged.EfLong),
            typeof(Flagged.EfUlong),
        };

        foreach (var enumType in enumTypes)
        {
            foreach (var data in GetEnumTestData(enumType, paramCount))
            {
                yield return data;
            }
        }
    }

    [Theory]
    [MemberData(nameof(EnumTestData), 1)]
    public void IsDefaultWorks<TEnum>(TEnum e)
        where TEnum : struct, Enum
    {
        var isDefault = EqualityComparer<TEnum>.Default.Equals(e, default(TEnum));
        var extIsDefault = EnumExtensions.IsDefault(e);
        Assert.Equal(isDefault, extIsDefault);
    }


    [Theory]
    [MemberData(nameof(EnumTestData), 2)]
    public void EqualWorks<TEnum>(TEnum left, TEnum right)
        where TEnum : struct, Enum
    {
        var equal = EqualityComparer<TEnum>.Default.Equals(left, right);
        var extEqual = EnumExtensions.IsEqual(left, right);
        Assert.Equal(equal, extEqual);
    }

    [Theory]
    [MemberData(nameof(EnumTestData), 2)]
    public void LessThanWorks<TEnum>(TEnum left, TEnum right)
        where TEnum : struct, Enum
    {
        var lessThan = Comparer<TEnum>.Default.Compare(left, right) < 0;
        var extLessThan = EnumExtensions.LessThan(left, right);
        Assert.Equal(lessThan, extLessThan);
    }


    [Theory]
    [MemberData(nameof(EnumTestData), 2)]
    public void LessThanOrEqualWorks<TEnum>(TEnum left, TEnum right)
        where TEnum : struct, Enum
    {
        var lessThanOrEqual = Comparer<TEnum>.Default.Compare(left, right) <= 0;
        var extLessThanOrEqual = EnumExtensions.LessThanOrEqual(left, right);
        Assert.Equal(lessThanOrEqual, extLessThanOrEqual);
    }

    [Theory]
    [MemberData(nameof(EnumTestData), 2)]
    public void GreaterThanWorks<TEnum>(TEnum left, TEnum right)
        where TEnum : struct, Enum
    {
        var greaterThan = Comparer<TEnum>.Default.Compare(left, right) > 0;
        var extGreaterThan = EnumExtensions.GreaterThan(left, right);
        Assert.Equal(greaterThan, extGreaterThan);
    }


    [Theory]
    [MemberData(nameof(EnumTestData), 2)]
    public void GreaterThanOrEqualWorks<TEnum>(TEnum left, TEnum right)
        where TEnum : struct, Enum
    {
        var greaterThanOrEqual = Comparer<TEnum>.Default.Compare(left, right) >= 0;
        var extGreaterThanOrEqual = EnumExtensions.GreaterThanOrEqual(left, right);
        Assert.Equal(greaterThanOrEqual, extGreaterThanOrEqual);
    }

    [Theory]
    [MemberData(nameof(EnumTestData), 2)]
    public void CompareToWorks<TEnum>(TEnum left, TEnum right)
        where TEnum : struct, Enum
    {
        int compare = Comparer<TEnum>.Default.Compare(left, right);
        var extCompare = EnumExtensions.CompareTo(left, right);
        Assert.Equal(compare, extCompare);
    }
}