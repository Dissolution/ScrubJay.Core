// ReSharper disable UnusedVariable

using System.Diagnostics;
using ScrubJay;
using ScrubJay.Collections;


List<int> list = [1,2,3];
var listSlice = ListSlice.Of(list, 2,1);

Console.WriteLine(listSlice);
Debugger.Break();