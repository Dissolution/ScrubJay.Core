# Rendering

**Note**: these are just my opinions!

I find myself overriding the default representations of `Enums` and `Types` consistently when I want to create user-friendly
messages.

## Types

By default, C# types stringify to harder to understand values:

| C# Type     | ToString                                          |
|-------------|---------------------------------------------------|
| `float`     | "System.Single"                                   |
| `int`       | "System.Int32"                                    |
| `List<int>` | "System.Collections.Generic.List1`[System.Int32]" |

Having a better way of representing `Type` led me down a path to Rendering

> An Aside on `IFormattable`
>
> `void Format(string? format, IFormatProvider? formatProvider)` has always felt clunky.
> Event the .net team opted for `Span<char>` when designing `ISpanFormattable` so they could avoid having to allocate `strings`,
and as this library is for me and very opinionated, I will never use `IFormatProvider`.

~~~~


### Contrived Example:

```csharp
public static class BindingFlagsExtensions
{
    public static string Render(this BindingFlags flags)
    {
        StringBuilder builder = new();
        if ((flags & BindingFlags.Public) != 0)
            builder.Append("public ");
        if ((flags & BindingFlags.NonPublic) != 0)
            builder.Append("private ");
        if ((flags & BindingFlags.Static) != 0)
            builder.Append("static");
        if (builder.Length > 0 && builder[^1] == ' ')
            builder.Length -= 1;
        return builder.ToString();
    }
}
```