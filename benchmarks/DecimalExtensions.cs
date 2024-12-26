using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Benchmarks;

public static class DecimalExtensions
{
    private delegate int getDigits(ref decimal dec);

    private static readonly getDigits _getDigits;

    public static readonly decimal Epsilon = new decimal(1, 0, 0, false, 28); //1e-28m;

    static DecimalExtensions()
    {
        var flagsFields = typeof(decimal).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(field => field.Name.Contains("flag", StringComparison.OrdinalIgnoreCase))
            .ToList();
        if (flagsFields.Count != 1)
            Debugger.Break();

        string flagsFieldName = flagsFields[0].Name;

        // getDigits
        // (ref dec) => ((dec.flags & ~int.MinValue) >> 16)

        var decParameter = Expression.Parameter(typeof(decimal).MakeByRefType(), "dec");
        var decFlagsField = Expression.Field(decParameter, flagsFieldName);
        var negMinValue = Expression.Constant(~int.MinValue, typeof(int));
        var sixteen = Expression.Constant(16, typeof(int));
        var getDigits = Expression.RightShift(Expression.And(decFlagsField, negMinValue), sixteen);
        _getDigits = Expression.Lambda<getDigits>(getDigits, decParameter).Compile();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int TrailingDigitCount(this decimal dec) => _getDigits(ref dec);
}