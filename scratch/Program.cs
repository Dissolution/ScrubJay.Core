// ReSharper disable RedundantUsingDirective
// ReSharper disable UnusedVariable

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using InlineIL;
using ScrubJay.Debugging;
using ScrubJay.Extensions;
using ScrubJay.Memory;
using ScrubJay.Scratch;
using ScrubJay.Validation;
using ScrubJay.Text;
using static InlineIL.IL;

int[] a = [1, 4, 7];
//Type aType = Utils.GetTypeOf<int[]>(a);
Type aSafeType = Utils.SafeGetTypeOf<int[]>(a);

ReadOnlySpan<char> b = "abcdf";
//Type bType = Utils.GetTypeOf<ReadOnlySpan<char>>(b);
Type bSafeType = Utils.SafeGetTypeOf<ReadOnlySpan<char>>(b);

object c = (object)Guid.NewGuid();
//Type cType = Utils.GetTypeOf<object>(c);
Type cSafeType = Utils.SafeGetTypeOf<object>(c);

Debugger.Break();
return 0;



namespace ScrubJay.Scratch
{
    public static class Utils
    {
        public static Type GetTypeOf<I>(I instance)
            where I : allows ref struct
        {
            var methods = typeof(I)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
               .Where(method => method.Name == nameof(GetType))
                //.Where(method => method.GetParameters().Length == 0)
                .ToList();

            Debug.Assert(methods.Count == 1);
            var getTypeMethod = methods[0];

            DynamicMethod dyn = new DynamicMethod(
                "GT",
                MethodAttributes.Public | MethodAttributes.Static,
                CallingConventions.Standard,
                typeof(Type),
                [typeof(I)],
                typeof(Utils).Module,
                true);
            ILGenerator il = dyn.GetILGenerator();
            if (typeof(I).IsValueType)
            {
                il.Emit(OpCodes.Ldarga, 0);
                il.Emit(OpCodes.Call, getTypeMethod);
            }
            else
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Callvirt, getTypeMethod);
            }

            il.Emit(OpCodes.Ret);

            Func<I, Type> gt = dyn.CreateDelegate<Func<I, Type>>();
            Type t = gt(instance);
            Debugger.Break();
            return t;
        }

        public static Type SafeGetTypeOf<I>(I instance)
            where I : allows ref struct
        {
            // Emit.Ldarg(nameof(instance));
            // Emit.Call(MethodRef.Method(TypeRef.Type<I>(), nameof(GetTypeOf)));
            // return Return<Type>();
            return typeof(I);
        }
    }




}





/*

// Usage
var done = parseDivideAsync("abc", "13");


Console.WriteLine((object)done);

return 0;

static Result<double> parse(string input) => Result.TryInvoke(() => double.Parse(input));

static Result<double> divide(double x, double y) => Result.TryInvoke(() =>
{
    if (y == 0)
        throw new DivideByZeroException();
    return x / y;
});

static async Result<double> parseDivideAsync(string a, string b)
{
    var x = await parse(a);
    var y = await Result.TryInvoke(() => double.Parse(b));
    Console.WriteLine("Successfully parsed inputs");
    return await divide(x, y);
}

*/
/*

static AsyncResult<double> Parse(string input)
{
    try
    {
        double f64 = double.Parse(input);
        return AsyncResult<double>.Success(f64);
    }
    catch (Exception ex)
    {
        return AsyncResult<double>.Fail(ex);
    }
}

static async AsyncResult<double> Add(string a, string b)
{
    var ad = await Parse(a);
    var bd = await Parse(b);
    return ad + bd;
}
*/










/*
object obj = (decimal)147.13m;
// if (obj is char)
// {
//     char ch = (char)obj;
//     Console.WriteLine(ch);
// }
// else
// {
//     Console.WriteLine("IS NOT");
// }

ref char ch = ref Notsafe.TryUnboxRef<char>(obj);
if (Notsafe.IsNullRef(ref ch))
{
    Console.WriteLine("Null ref");
}
else
{
    Console.WriteLine(ch);
}

ReadOnlySpan<char> text = new ReadOnlySpan<char>(in ch);
var str = text.AsString();
Console.WriteLine(str);
*/
/*
ReadOnlySpan<char> left = ['a', 'b', 'c'];
ReadOnlySpan<char> right = "abc".ToCharArray().AsSpan();

var eq = Equate
    .GetEqualityComparer<ReadOnlySpan<char>>()
    .Equals(left, right);
Debugger.Break();


char[] chars = new char[128];
var writer = new TryFormatWriter(chars)
{
    '(',
    (""),
    (147),
    {147, "F1"},
    (147, "F2", (IFormatProvider)CultureInfo.CurrentCulture),
    (ref TryFormatWriter w) => w.Add("^_^"),
};
string str = writer.GetString();
var result = writer.GetResult();

Debugger.Break();



Console.WriteLine(str);

Point pt = new(1, 2);

var property = typeof(Point).GetProperty(nameof(Point.X), BindingFlags.Public | BindingFlags.Instance);

object? x = property!.GetValue((object?)pt);

var getMethod = property.GetMethod;
object? x2 = getMethod!.Invoke((object?)pt, null);

DynamicMethod dyn = default!;
var gen = dyn.GetILGenerator();

gen.Emit(OpCodes.Ldarg_0);
gen.Emit(OpCodes.Call, getMethod);
gen.Emit(OpCodes.Ret);

var getter = dyn.CreateDelegate<Func<Point, int>>();


int x3 = getter(pt);

*/
