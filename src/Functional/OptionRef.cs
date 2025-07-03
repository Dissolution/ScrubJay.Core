#if NET8_0_OR_GREATER
namespace ScrubJay.Functional;

public ref struct OptionRef<T>
{
    [MaybeNull]
    private ref T _reference;

    // none
    public OptionRef()
    {
        _reference = ref Notsafe.NullRef<T>();
    }

    // some
    public OptionRef(ref T someValue)
    {
        _reference = ref someValue;
    }

    public ref T Ref
    {
        [return: MaybeNull]
        get => ref _reference!;
    }

    public bool IsSome(ref T someValue)
    {
        if (Notsafe.IsNonNullRef(in _reference))
        {
            someValue = ref _reference;
            return true;
        }
        else
        {
            someValue = ref Notsafe.NullRef<T>();
            return false;
        }
    }

    public bool IsNone() => Notsafe.IsNullRef(ref _reference);

}
#endif