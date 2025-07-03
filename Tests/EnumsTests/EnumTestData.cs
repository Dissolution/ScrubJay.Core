#pragma warning disable CA1028, CA1069, CA1034, CA2263

namespace ScrubJay.Tests.EnumsTests;

public static class EnumTestData
{
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

        // ReSharper disable once EnumUnderlyingTypeIsInt
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
            None = 0,
            Alpha = 1 << 0,
            Beta = 1 << 1,
            Gamma = 1 << 2,
            Alphabet = Alpha | Beta,
            //Max = sbyte.MaxValue,
        }

        [Flags]
        public enum EfByte : byte
        {
            None = 0,
            Alpha = 1 << 0,
            Beta = 1 << 1,
            Gamma = 1 << 2,
            Alphabet = Alpha | Beta,
            //Max = byte.MaxValue,
        }

        [Flags]
        public enum EfShort : short
        {
            None = 0,
            Alpha = 1 << 0,
            Beta = 1 << 1,
            Gamma = 1 << 2,
            Alphabet = Alpha | Beta,
            //Max = short.MaxValue,
        }

        [Flags]
        public enum EfUshort : ushort
        {
            None = 0,
            Alpha = 1 << 0,
            Beta = 1 << 1,
            Gamma = 1 << 2,
            Alphabet = Alpha | Beta,
            //Max = ushort.MaxValue,
        }

        [Flags]
        // ReSharper disable once EnumUnderlyingTypeIsInt
        public enum EfInt : int
        {
            None = 0,
            Alpha = 1 << 0,
            Beta = 1 << 1,
            Gamma = 1 << 2,
            Alphabet = Alpha | Beta,
            // Max = int.MaxValue,
        }

        [Flags]
        public enum EfUint : uint
        {
            None = 0,
            Alpha = 1 << 0,
            Beta = 1 << 1,
            Gamma = 1 << 2,
            Alphabet = Alpha | Beta,
            //Max = uint.MaxValue,
        }

        [Flags]
        public enum EfLong : long
        {
            None = 0,
            Alpha = 1 << 0,
            Beta = 1 << 1,
            Gamma = 1 << 2,
            Alphabet = Alpha | Beta,
            //Max = long.MaxValue,
        }

        [Flags]
        public enum EfUlong : ulong
        {
            None = 0,
            Alpha = 1 << 0,
            Beta = 1 << 1,
            Gamma = 1 << 2,
            Alphabet = Alpha | Beta,
            //Max = ulong.MaxValue,
        }
    }

    public static IEnumerable<object[]> GetEnumTestData<E>(int paramCount)
        where E : struct, Enum
    {
        if (paramCount < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(paramCount), paramCount, "Parameter count must be 1 or greater");
        }

        E[] enumMembers = (E[])Enum.GetValues(typeof(E));
        int combinations = (int)Math.Pow(enumMembers.Length, paramCount);
        for (int i = 0; i < combinations; i++)
        {
            object[] values = new object[paramCount];
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
        {
            throw new ArgumentException("Type must be an enum type", nameof(enumType));
        }

        if (paramCount < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(paramCount), paramCount, "Parameter count must be 1 or greater");
        }

        var enumMembers = Enum.GetValues(enumType);
        int combinations = (int)Math.Pow(enumMembers.Length, paramCount);
        var testData = new List<object[]>(combinations);
        for (int i = 0; i < combinations; i++)
        {
            object[] values = new object[paramCount];
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