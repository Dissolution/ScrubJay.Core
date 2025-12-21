namespace ScrubJay.Memory;

[PublicAPI]
[Flags]
public enum SplitOptions
{
    None = 0,
    IgnoreEmpty = 1 << 0,
}