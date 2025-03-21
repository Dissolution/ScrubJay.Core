// ReSharper disable RedundantUsingDirective
// ReSharper disable UnusedVariable

using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using ScrubJay.Extensions;
using ScrubJay.Memory;
using ScrubJay.Validation;
using ScrubJay.Text;

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
Debugger.Break();
