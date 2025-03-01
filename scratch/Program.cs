// ReSharper disable UnusedVariable

using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Reflection.Emit;

string str = "eat at joes";

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

Debugger.Break();
