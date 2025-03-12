using ScrubJay.Enums;
using static ScrubJay.Tests.EnumsTests.EnumTestData;

// ReSharper disable InvokeAsExtensionMethod

namespace ScrubJay.Tests.EnumsTests;

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
            foreach (object[]? data in GetEnumTestData(enumType, paramCount))
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
        bool isDefault = EqualityComparer<TEnum>.Default.Equals(e, default(TEnum));
        bool extIsDefault = EnumExtensions.IsDefault(e);
        Assert.Equal(isDefault, extIsDefault);
    }


    [Theory]
    [MemberData(nameof(EnumTestData), 2)]
    public void EqualWorks<TEnum>(TEnum left, TEnum right)
        where TEnum : struct, Enum
    {
        bool equal = EqualityComparer<TEnum>.Default.Equals(left, right);
        bool extEqual = EnumExtensions.IsEqual(left, right);
        Assert.Equal(equal, extEqual);
    }

    [Theory]
    [MemberData(nameof(EnumTestData), 2)]
    public void LessThanWorks<TEnum>(TEnum left, TEnum right)
        where TEnum : struct, Enum
    {
        bool lessThan = Comparer<TEnum>.Default.Compare(left, right) < 0;
        bool extLessThan = EnumExtensions.LessThan(left, right);
        Assert.Equal(lessThan, extLessThan);
    }


    [Theory]
    [MemberData(nameof(EnumTestData), 2)]
    public void LessThanOrEqualWorks<TEnum>(TEnum left, TEnum right)
        where TEnum : struct, Enum
    {
        bool lessThanOrEqual = Comparer<TEnum>.Default.Compare(left, right) <= 0;
        bool extLessThanOrEqual = EnumExtensions.LessThanOrEqual(left, right);
        Assert.Equal(lessThanOrEqual, extLessThanOrEqual);
    }

    [Theory]
    [MemberData(nameof(EnumTestData), 2)]
    public void GreaterThanWorks<TEnum>(TEnum left, TEnum right)
        where TEnum : struct, Enum
    {
        bool greaterThan = Comparer<TEnum>.Default.Compare(left, right) > 0;
        bool extGreaterThan = EnumExtensions.GreaterThan(left, right);
        Assert.Equal(greaterThan, extGreaterThan);
    }


    [Theory]
    [MemberData(nameof(EnumTestData), 2)]
    public void GreaterThanOrEqualWorks<TEnum>(TEnum left, TEnum right)
        where TEnum : struct, Enum
    {
        bool greaterThanOrEqual = Comparer<TEnum>.Default.Compare(left, right) >= 0;
        bool extGreaterThanOrEqual = EnumExtensions.GreaterThanOrEqual(left, right);
        Assert.Equal(greaterThanOrEqual, extGreaterThanOrEqual);
    }

    [Theory]
    [MemberData(nameof(EnumTestData), 2)]
    public void CompareToWorks<TEnum>(TEnum left, TEnum right)
        where TEnum : struct, Enum
    {
        int compare = Comparer<TEnum>.Default.Compare(left, right);
        int extCompare = EnumExtensions.CompareTo(left, right);
        Assert.Equal(compare, extCompare);
    }
}