#pragma warning disable S1481, CS0168
// ReSharper disable UnusedVariable

using System.Diagnostics;
using ScrubJay.Constraints;
using ScrubJay.Functional;



var parsed = Bounds.Parse<int>($"{4}..{3}");

Debugger.Break();