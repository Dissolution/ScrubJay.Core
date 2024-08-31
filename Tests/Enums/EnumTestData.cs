namespace ScrubJay.Tests.Enums;

public static class EnumTestData
{
    static EnumTestData()
    {

    }
    
    public static class NonFlagged
    {
        // https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/types#836-integral-types
        
        public enum ESbyte : sbyte
        {
            Min = sbyte.MinValue,
            NegOne = -1,
            Zero = 0,
            One = 1,
            Two = 2,
            Three = 3,
            Max = sbyte.MaxValue,
        }
        
        public enum EByte : byte
        {
            Min = byte.MinValue,
            Zero = 0,
            One = 1,
            Two = 2,
            Three = 3,
            Max = byte.MaxValue,
        }
        
        public enum EShort : short
        {
            Min = short.MinValue,
            NegOne = -1,
            Zero = 0,
            One = 1,
            Two = 2,
            Three = 3,
            Max = short.MaxValue,
        }
        
        public enum EUshort : ushort
        {
            Min = ushort.MinValue,
            Zero = 0,
            One = 1,
            Two = 2,
            Three = 3,
            Max = ushort.MaxValue,
        }
        
        public enum EInt : int
        {
            Min = int.MinValue,
            NegOne = -1,
            Zero = 0,
            One = 1,
            Two = 2,
            Three = 3,
            Max = int.MaxValue,
        }
        
        public enum EUint : uint
        {
            Min = uint.MinValue,
            Zero = 0,
            One = 1,
            Two = 2,
            Three = 3,
            Max = uint.MaxValue,
        }
        
        public enum ELong : long
        {
            Min = long.MinValue,
            NegOne = -1,
            Zero = 0,
            One = 1,
            Two = 2,
            Three = 3,
            Max = long.MaxValue,
        }
        
        public enum EUlong : ulong
        {
            Min = ulong.MinValue,
            Zero = 0,
            One = 1,
            Two = 2,
            Three = 3,
            Max = ulong.MaxValue,
        }
    }


    public static class Flagged
    {
        [Flags]
        public enum EfSbyte : sbyte
        {
            Default = 0,
            Alpha = 1 << 0,
            Beta = 1 << 1,
            Gamma = 1 << 2,
            Alphabet = Alpha | Beta,
            //Max = sbyte.MaxValue,
        }

        [Flags]
        public enum EfByte : byte
        {
            Default = 0,
            Alpha = 1 << 0,
            Beta = 1 << 1,
            Gamma = 1 << 2,
            Alphabet = Alpha | Beta,
            //Max = byte.MaxValue,
        }
        
        [Flags]
        public enum EfShort : short
        {
            Default = 0,
            Alpha = 1 << 0,
            Beta = 1 << 1,
            Gamma = 1 << 2,
            Alphabet = Alpha | Beta,
            //Max = short.MaxValue,
        }

        [Flags]
        public enum EfUshort : ushort
        {
            Default = 0,
            Alpha = 1 << 0,
            Beta = 1 << 1,
            Gamma = 1 << 2,
            Alphabet = Alpha | Beta,
            //Max = ushort.MaxValue,
        }
        
        [Flags]
        public enum EfInt : int
        {
            Default = 0,
            Alpha = 1 << 0,
            Beta = 1 << 1,
            Gamma = 1 << 2,
            Alphabet = Alpha | Beta,
           // Max = int.MaxValue,
        }

        [Flags]
        public enum EfUint : uint
        {
            Default = 0,
            Alpha = 1 << 0,
            Beta = 1 << 1,
            Gamma = 1 << 2,
            Alphabet = Alpha | Beta,
            //Max = uint.MaxValue,
        }
        
        [Flags]
        public enum EfLong : long
        {
            Default = 0,
            Alpha = 1 << 0,
            Beta = 1 << 1,
            Gamma = 1 << 2,
            Alphabet = Alpha | Beta,
            //Max = long.MaxValue,
        }

        [Flags]
        public enum EfUlong : ulong
        {
            Default = 0,
            Alpha = 1 << 0,
            Beta = 1 << 1,
            Gamma = 1 << 2,
            Alphabet = Alpha | Beta,
            //Max = ulong.MaxValue,
        }

    }

    public static IEnumerable<object[]> GetEnumTestData<TEnum>(int paramCount)
        where TEnum : struct, Enum
    {
        if (paramCount < 1)
            throw new ArgumentOutOfRangeException(nameof(paramCount), paramCount, "Parameter count must be 1 or greater");
        var enumMembers = (TEnum[])Enum.GetValues(typeof(TEnum));
        int combinations = (int)Math.Pow(enumMembers.Length, paramCount);
        for (int i = 0; i < combinations; i++)
        {
            var values = new object[paramCount];
            int index = i;
            for (int j = 0; j < paramCount; j++)
            {
                int enumIndex = index % enumMembers.Length;
                values[j] = enumMembers[enumIndex];
                index /= enumMembers.Length;
            }
            yield return values;
        }
    }
    
    public static IReadOnlyList<object[]> GetEnumTestData(Type enumType, int paramCount)
    {
        if (!enumType.IsEnum)
            throw new ArgumentException("Type must be an enum type", nameof(enumType));
        if (paramCount < 1)
            throw new ArgumentOutOfRangeException(nameof(paramCount), paramCount, "Parameter count must be 1 or greater");
        var enumMembers = Enum.GetValues(enumType);
        int combinations = (int)Math.Pow(enumMembers.Length, paramCount);
        var testData = new List<object[]>(combinations);
        for (int i = 0; i < combinations; i++)
        {
            var values = new object[paramCount];
            int index = i;
            for (int j = 0; j < paramCount; j++)
            {
                int enumIndex = index % enumMembers.Length;
                values[j] = enumMembers.GetValue(enumIndex)!;
                index /= enumMembers.Length;
            }
            testData.Add(values);
        }
        return testData;
    }
}