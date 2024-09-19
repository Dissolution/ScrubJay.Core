#pragma warning disable S1481, CS0168
// ReSharper disable UnusedVariable

using System.Diagnostics;
using ScrubJay;


Result<int, ICloneable> result;

ICloneable thing = default!;
Console.WriteLine(thing);
Debugger.Break();