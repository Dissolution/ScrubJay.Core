namespace ScrubJay.Tests;

public readonly record struct IdName(int Id, string Name);

public sealed class IdNameComparer : Comparer<IdName>, IHasDefault<IdNameComparer>
{
    public new static IdNameComparer Default { get; } = new();

    public override int Compare(IdName x, IdName y)
    {
        return x.Id.CompareTo(y.Id);
    }
}