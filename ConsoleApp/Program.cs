#pragma warning disable S1481, CS0168
// ReSharper disable UnusedVariable

using System.Diagnostics;
using System.Linq.Expressions;

Expression expr = () => int.Parse("123");
var type = expr.GetType();
var name = type.NameOf();

Console.WriteLine(name);

using var pl = new PooledList<int>();
Console.WriteLine(pl.GetType().NameOf());

Console.WriteLine("Press any key to exit...");
Console.ReadKey();

Debugger.Break();
return;
