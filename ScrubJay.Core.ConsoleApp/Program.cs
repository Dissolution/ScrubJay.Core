using System.Reflection;
using ScrubJay.Debugging;
using ScrubJay.Expressions;




















var expr = Express.Func<BindingFlags, BindingFlags, BindingFlags>((left, right) => Express
    .Pair(
        left.Convert(typeof(BindingFlags).GetEnumUnderlyingType()),
        right.Convert(typeof(BindingFlags).GetEnumUnderlyingType()))
    .Or()
    .Convert<BindingFlags>());

Console.WriteLine(expr);

var iid = expr.GetInstanceId();
Console.WriteLine(iid);


Console.WriteLine("Press enter to close this window");
Console.ReadLine();