namespace ScrubJay.Fluent;


[PublicAPI]
public abstract class FluentBuilder<TBuilder, TBuilding> : FluentBuilder<TBuilder>
    where TBuilder : FluentBuilder<TBuilder, TBuilding>
{
    public static implicit operator TBuilding(FluentBuilder<TBuilder, TBuilding> builder) => builder.TryBuild().OkOrThrow();

    protected FluentBuilder() : base()
    {

    }

    public abstract Result<TBuilding, Exception> TryBuild();
}

[PublicAPI]
public abstract class FluentRecordBuilder<TBuilder, TRecord> : FluentBuilder<TBuilder, TRecord>
    where TBuilder : FluentRecordBuilder<TBuilder, TRecord>
    where TRecord : class, new()
{
    protected readonly TRecord _record;

    protected virtual bool IsValid => _record is not null;

    protected FluentRecordBuilder()
    {
        _record = new();
    }

    protected FluentRecordBuilder(TRecord? record)
    {
        _record = record ?? new();
    }

    public override Result<TRecord, Exception> TryBuild()
    {
        if (!IsValid)
            return new InvalidOperationException($"The {typeof(TRecord).NameOf()} is not in a valid state");
        return _record;
    }
}
