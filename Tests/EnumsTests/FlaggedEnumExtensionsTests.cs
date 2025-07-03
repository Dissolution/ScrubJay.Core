// net481 does not have generic targets
#pragma warning disable CA2263

using ScrubJay.Enums;
using ScrubJay.Utilities;
using ScrubJay.Validation;
using static ScrubJay.Tests.EnumsTests.EnumTestData;
// ReSharper disable InvokeAsExtensionMethod

namespace ScrubJay.Tests.EnumsTests;

public class FlaggedEnumExtensionsTests
{
    public static IEnumerable<object[]> EnumTestData(int paramCount)
    {
        var enumTypes = new Type[]
        {
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
    public void ToUInt64Works<E>(E @enum)
        where E : struct, Enum
    {
        ulong convertValue = Convert.ToUInt64(@enum);
        ulong extValue = FlagsEnumExtensions.ToUInt64<E>(@enum);
        Assert.Equal(convertValue, extValue);
    }

    [Theory]
    [MemberData(nameof(EnumTestData), 1)]
    public void FromUInt64Works<E>(E @enum)
        where E : struct, Enum
    {
        ulong convertValue = Convert.ToUInt64(@enum);

        E extEnum = FlagsEnumExtensions.FromUInt64<E>(convertValue);

        Assert.Equal(@enum, extEnum);
    }

   /*
    [Theory]
    [MemberData(nameof(EnumTestData), 1)]
    public void NotWorks<E>(E @enum)
        where E : struct, Enum
    {
        var notResult = Express
            .Func<E, E>(e => e
                .Convert(typeof(E).GetEnumUnderlyingType())
                .OnesComplement()
                .Convert<E>())
            .Compile()
            .Invoke(@enum);
        var extResult = FlagsEnumExtensions.BitwiseComplement<E>(@enum);
        Assert.Equal(notResult, extResult);
    }

    [Theory]
    [MemberData(nameof(EnumTestData), 2)]
    public void AndWorks<E>(E left, E right)
        where E : struct, Enum
    {
        var andResult = Express.Func<E, E, E>((l, r) => Express
                .Pair(
                    l.Convert(typeof(E).GetEnumUnderlyingType()),
                    r.Convert(typeof(E).GetEnumUnderlyingType()))
                .And()
                .Convert<E>())
            .Compile()
            .Invoke(left, right);
        var extResult = FlagsEnumExtensions.And<E>(left, right);
        Assert.Equal(andResult, extResult);
    }

    [Theory]
    [MemberData(nameof(EnumTestData), 2)]
    public void OrWorks<E>(E left, E right)
        where E : struct, Enum
    {
        var orResult = Express.Func<E, E, E>((l, r) => Express
                .Pair(
                    l.Convert(typeof(E).GetEnumUnderlyingType()),
                    r.Convert(typeof(E).GetEnumUnderlyingType()))
                .Or()
                .Convert<E>())
            .Compile()
            .Invoke(left, right);
        var extResult = FlagsEnumExtensions.Or<E>(left, right);
        Assert.Equal(orResult, extResult);
    }

    [Theory]
    [MemberData(nameof(EnumTestData), 2)]
    public void XorWorks<E>(E left, E right)
        where E : struct, Enum
    {
        var xorResult = Express.Func<E, E, E>((l, r) => Express
                .Pair(
                    l.Convert(typeof(E).GetEnumUnderlyingType()),
                    r.Convert(typeof(E).GetEnumUnderlyingType()))
                .ExclusiveOr()
                .Convert<E>())
            .Compile()
            .Invoke(left, right);
        var extResult = FlagsEnumExtensions.Xor<E>(left, right);
        Assert.Equal(xorResult, extResult);
    }
*/

    private static int GetLongBitCount(long lValue)
    {
        int iCount = 0;

        //Loop the value while there are still bits
        while (lValue != 0L)
        {
            //Remove the end bit
            lValue &= (lValue - 1L);

            //Increment the count
            iCount++;
        }

        //Return the count
        return iCount;
    }

    private static int GetULongBitCount(ulong lValue)
    {
        int iCount = 0;

        //Loop the value while there are still bits
        while (lValue != 0UL)
        {
            //Remove the end bit
            lValue &= (lValue - 1UL);

            //Increment the count
            iCount++;
        }

        //Return the count
        return iCount;
    }

    private static readonly Type[] _signedPrimitiveTypes = [typeof(long), typeof(int), typeof(short), typeof(sbyte)];

    [Theory]
    [MemberData(nameof(EnumTestData), 1)]
    public void FlagCountWorks<E>(E e)
        where E : struct, Enum
    {
        int countResult;
        if (_signedPrimitiveTypes.Contains(EnumExtensions.UnderlyingType<E>()))
        {
            countResult = GetLongBitCount(Convert.ToInt64(e));
        }
        else
        {
            countResult = GetULongBitCount(Convert.ToUInt64(e));
        }

        int extResult = FlagsEnumExtensions.FlagCount<E>(e);
        Assert.Equal(countResult, extResult);
    }

    [Theory]
    [MemberData(nameof(EnumTestData), 2)]
    public void FlagCountWorks2<E>(E left, E right)
        where E : struct, Enum
    {
        var e = FlagsEnumExtensions.Or(left, right);

        int countResult;
        if (_signedPrimitiveTypes.Contains(EnumExtensions.UnderlyingType<E>()))
        {
            countResult = GetLongBitCount(Convert.ToInt64(e));
        }
        else
        {
            countResult = GetULongBitCount(Convert.ToUInt64(e));
        }

        int extResult = FlagsEnumExtensions.FlagCount<E>(e);
        Assert.Equal(countResult, extResult);
    }

    [Theory]
    [MemberData(nameof(EnumTestData), 1)]
    public void HasMultipleFlagsWorks<E>(E e)
        where E : struct, Enum
    {
        var flags = Enum
            .GetValues(typeof(E))
            .ThrowIfNot<E[]>()
            .Where(flag =>
            {
                ulong value = FlagsEnumExtensions.ToUInt64(flag);
                return ((value & (value - 1)) == 0) && (value != 0);
            })
            .Where(flag => e.HasFlag(flag))
            .ToArray();

        bool hasMultipleFlags = FlagsEnumExtensions.HasMultipleFlags(e);

        if (flags.Length > 1)
        {
            Assert.True(hasMultipleFlags);
        }
        else
        {
            Assert.False(hasMultipleFlags);
        }
    }

    [Theory]
    [MemberData(nameof(EnumTestData), 1)]
    public void GetFlagsWorks<E>(E e)
        where E : struct, Enum
    {
        var flags = Enum
            .GetValues(typeof(E))
            .ThrowIfNot<E[]>()
            .Where(flag =>
            {
                ulong value = FlagsEnumExtensions.ToUInt64(flag);
                return ((value & (value - 1)) == 0) && (value != 0);
            })
            .Where(flag => e.HasFlag(flag))
            .ToArray();

        var extFlags = FlagsEnumExtensions.GetFlags(e);

        Assert.True(Sequence.Equal(flags, extFlags));
    }

    [Theory]
    [MemberData(nameof(EnumTestData), 2)]
    public void GetFlagsWorks2<E>(E left, E right)
        where E : struct, Enum
    {
        var e = FlagsEnumExtensions.Or(left, right);

        var flags = Enum
            .GetValues(typeof(E))
            .ThrowIfNot<E[]>()
            .Where(flag =>
            {
                ulong value = FlagsEnumExtensions.ToUInt64(flag);
                return ((value & (value - 1)) == 0) && (value != 0);
            })
            .Where(flag => e.HasFlag(flag))
            .ToArray();

        var extFlags = FlagsEnumExtensions.GetFlags(e);

        Assert.True(Sequence.Equal(flags, extFlags));
    }

    // All the *Flags methods use the methods already checked above!
}
