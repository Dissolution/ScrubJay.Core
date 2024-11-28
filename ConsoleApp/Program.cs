#pragma warning disable S1481, CS0168
// ReSharper disable UnusedVariable

using System.Diagnostics;
using ScrubJay.Extensions;


// var str = string.Join(", ", Test.GetEnumerator(..4));

foreach (var i in 2..)
{
    Console.WriteLine(i);
}

Console.WriteLine("Press any key to exit...");
Console.ReadKey();

Debugger.Break();
return;