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
            foreach (object[] data in GetEnumTestData(enumType, paramCount))
            {
                yield return data;
            }
        }
    }

    [Theory]
    [MemberData(nameof(EnumTestData), 1)]
    public void IsDefaultWorks<E>(E e)
        where E : struct, Enum
    {
        bool isDefault = EqualityComparer<E>.Default.Equals(e, default(E));
        bool extIsDefault = EnumExtensions.IsDefault(e);
        Assert.Equal(isDefault, extIsDefault);
    }


    [Theory]
    [MemberData(nameof(EnumTestData), 2)]
    public void EqualWorks<E>(E left, E right)
        where E : struct, Enum
    {
        bool equal = EqualityComparer<E>.Default.Equals(left, right);
        bool extEqual = EnumExtensions.IsEqual(left, right);
        Assert.Equal(equal, extEqual);
    }

    [Theory]
    [MemberData(nameof(EnumTestData), 2)]
    public void LessThanWorks<E>(E left, E right)
        where E : struct, Enum
    {
        bool lessThan = Comparer<E>.Default.Compare(left, right) < 0;
        bool extLessThan = EnumExtensions.LessThan(left, right);
        Assert.Equal(lessThan, extLessThan);
    }


    [Theory]
    [MemberData(nameof(EnumTestData), 2)]
    public void LessThanOrEqualWorks<E>(E left, E right)
        where E : struct, Enum
    {
        bool lessThanOrEqual = Comparer<E>.Default.Compare(left, right) <= 0;
        bool extLessThanOrEqual = EnumExtensions.LessThanOrEqual(left, right);
        Assert.Equal(lessThanOrEqual, extLessThanOrEqual);
    }

    [Theory]
    [MemberData(nameof(EnumTestData), 2)]
    public void GreaterThanWorks<E>(E left, E right)
        where E : struct, Enum
    {
        bool greaterThan = Comparer<E>.Default.Compare(left, right) > 0;
        bool extGreaterThan = EnumExtensions.GreaterThan(left, right);
        Assert.Equal(greaterThan, extGreaterThan);
    }


    [Theory]
    [MemberData(nameof(EnumTestData), 2)]
    public void GreaterThanOrEqualWorks<E>(E left, E right)
        where E : struct, Enum
    {
        bool greaterThanOrEqual = Comparer<E>.Default.Compare(left, right) >= 0;
        bool extGreaterThanOrEqual = EnumExtensions.GreaterThanOrEqual(left, right);
        Assert.Equal(greaterThanOrEqual, extGreaterThanOrEqual);
    }

    [Theory]
    [MemberData(nameof(EnumTestData), 2)]
    public void CompareToWorks<E>(E left, E right)
        where E : struct, Enum
    {
        int compare = Comparer<E>.Default.Compare(left, right);
        int extCompare = EnumExtensions.CompareTo(left, right);
        Assert.Equal(compare, extCompare);
    }
}