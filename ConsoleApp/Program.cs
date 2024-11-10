#pragma warning disable S1481, CS0168
// ReSharper disable UnusedVariable

using System.Diagnostics;
using ScrubJay.Constraints;

var pair = Pair.Parse<int, string>($"({147},{"Joe"}");


/*
Validate.InRange(4, 1..4);

var parsed = Bounds.Parse<int>($"{4}..{3}");
*/


Debugger.Break();