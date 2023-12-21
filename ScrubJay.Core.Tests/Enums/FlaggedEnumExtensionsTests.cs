using ScrubJay.Comparison;
using ScrubJay.Enums;
using ScrubJay.Expressions;
using ScrubJay.Validation;
using static ScrubJay.Tests.Enums.EnumTestData;
// ReSharper disable InvokeAsExtensionMethod

namespace ScrubJay.Tests.Enums;

public class FlaggedEnumExtensionsTests
{
    public static IEnumerable<object[]> EnumTestData(int paramCount)
    {
        var enumTypes = new Type[]
        {
            typeof(Flagged.EFSbyte),
            typeof(Flagged.EFByte),
            typeof(Flagged.EFShort),
            typeof(Flagged.EFUshort),
            typeof(Flagged.EFInt),
            typeof(Flagged.EFUint),
            typeof(Flagged.EFLong),
            typeof(Flagged.EFUlong),
        };

        foreach (var enumType in enumTypes)
        foreach (var data in GetEnumTestData(enumType, paramCount))
        {
            yield return data;
        }
    }

    [Theory]
    [MemberData(nameof(EnumTestData), 1)]
    public void ToUInt64Works<TEnum>(TEnum @enum)
        where TEnum : struct, Enum
    {
        ulong convertValue = Convert.ToUInt64(@enum);
        var extValue = FlagsEnumExtensions.ToUInt64<TEnum>(@enum);
        Assert.Equal(convertValue, extValue);
    }
    
    [Theory]
    [MemberData(nameof(EnumTestData), 1)]
    public void FromUInt64Works<TEnum>(TEnum @enum)
        where TEnum : struct, Enum
    {
        ulong convertValue = Convert.ToUInt64(@enum);

        TEnum extEnum = FlagsEnumExtensions.FromUInt64<TEnum>(convertValue);

        Assert.Equal(@enum, extEnum);
    }
    
    
    [Theory]
    [MemberData(nameof(EnumTestData), 1)]
    public void NotWorks<TEnum>(TEnum @enum)
        where TEnum : struct, Enum
    {
        var notResult = Express
            .Func<TEnum, TEnum>(e => e
                .Convert(typeof(TEnum).GetEnumUnderlyingType())
                .OnesComplement()
                .Convert<TEnum>())
            .Compile()
            .Invoke(@enum);
        var extResult = FlagsEnumExtensions.Not<TEnum>(@enum);
        Assert.Equal(notResult, extResult);
    }
    
    [Theory]
    [MemberData(nameof(EnumTestData), 2)]
    public void AndWorks<TEnum>(TEnum left, TEnum right)
        where TEnum : struct, Enum
    {
        var andResult = Express.Func<TEnum, TEnum, TEnum>((l, r) => Express
                .Pair(
                    l.Convert(typeof(TEnum).GetEnumUnderlyingType()),
                    r.Convert(typeof(TEnum).GetEnumUnderlyingType()))
                .And()
                .Convert<TEnum>())
            .Compile()
            .Invoke(left, right);
        var extResult = FlagsEnumExtensions.And<TEnum>(left, right);
        Assert.Equal(andResult, extResult);
    }
    
    [Theory]
    [MemberData(nameof(EnumTestData), 2)]
    public void OrWorks<TEnum>(TEnum left, TEnum right)
        where TEnum : struct, Enum
    {
        var orResult = Express.Func<TEnum, TEnum, TEnum>((l, r) => Express
                .Pair(
                    l.Convert(typeof(TEnum).GetEnumUnderlyingType()),
                    r.Convert(typeof(TEnum).GetEnumUnderlyingType()))
                .Or()
                .Convert<TEnum>())
            .Compile()
            .Invoke(left, right);
        var extResult = FlagsEnumExtensions.Or<TEnum>(left, right);
        Assert.Equal(orResult, extResult);
    }
    
    [Theory]
    [MemberData(nameof(EnumTestData), 2)]
    public void XorWorks<TEnum>(TEnum left, TEnum right)
        where TEnum : struct, Enum
    {
        var xorResult = Express.Func<TEnum, TEnum, TEnum>((l, r) => Express
                .Pair(
                    l.Convert(typeof(TEnum).GetEnumUnderlyingType()),
                    r.Convert(typeof(TEnum).GetEnumUnderlyingType()))
                .ExclusiveOr()
                .Convert<TEnum>())
            .Compile()
            .Invoke(left, right);
        var extResult = FlagsEnumExtensions.Xor<TEnum>(left, right);
        Assert.Equal(xorResult, extResult);
    }


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
    
    [Theory]
    [MemberData(nameof(EnumTestData), 1)]
    public void FlagCountWorks<TEnum>(TEnum e)
        where TEnum : struct, Enum
    {
        int countResult;
        if (EnumExtensions.UnderlyingType<TEnum>().EqualsAny(typeof(long), typeof(int), typeof(short), typeof(sbyte)))
        {
            countResult = GetLongBitCount(Convert.ToInt64(e));
        }
        else
        {
            
            countResult = GetULongBitCount(Convert.ToUInt64(e));
        }

        var extResult = FlagsEnumExtensions.FlagCount<TEnum>(e);
        Assert.Equal(countResult, extResult);
    }
    
    [Theory]
    [MemberData(nameof(EnumTestData), 2)]
    public void FlagCountWorks2<TEnum>(TEnum left, TEnum right)
        where TEnum : struct, Enum
    {
        var e = FlagsEnumExtensions.Or(left, right);
        
        int countResult;
        if (EnumExtensions.UnderlyingType<TEnum>().EqualsAny(typeof(long), typeof(int), typeof(short), typeof(sbyte)))
        {
            countResult = GetLongBitCount(Convert.ToInt64(e));
        }
        else
        {
            countResult = GetULongBitCount(Convert.ToUInt64(e));
        }

        var extResult = FlagsEnumExtensions.FlagCount<TEnum>(e);
        Assert.Equal(countResult, extResult);
    }
    
    [Theory]
    [MemberData(nameof(EnumTestData), 1)]
    public void GetFlagsWorks<TEnum>(TEnum e)
        where TEnum : struct, Enum
    {
        var flags = Enum
            .GetValues(typeof(TEnum))
            .AsValid<TEnum[]>()
            .Where(flag => MathHelper.IsPowerOfTwo(FlagsEnumExtensions.ToUInt64(flag)))
            .Where(flag => e.HasFlag(flag))
            .ToArray();

        var extFlags = FlagsEnumExtensions.GetFlags(e);

        Assert.True(EnumerableEqualityComparer<TEnum>.Default.Equals(flags, extFlags));
    }
    
    [Theory]
    [MemberData(nameof(EnumTestData), 2)]
    public void GetFlagsWorks2<TEnum>(TEnum left, TEnum right)
        where TEnum : struct, Enum
    {
        var e = FlagsEnumExtensions.Or(left, right);
        
        var flags = Enum
            .GetValues(typeof(TEnum))
            .AsValid<TEnum[]>()
            .Where(flag => MathHelper.IsPowerOfTwo(FlagsEnumExtensions.ToUInt64(flag)))
            .Where(flag => e.HasFlag(flag))
            .ToArray();

        var extFlags = FlagsEnumExtensions.GetFlags(e);

        Assert.True(EnumerableEqualityComparer<TEnum>.Default.Equals(flags, extFlags));
    }
    
    // All the *Flags methods use the methods already checked above!
}