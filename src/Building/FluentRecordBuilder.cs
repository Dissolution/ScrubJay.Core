using ScrubJay.Building;

namespace ScrubJay.Fluent;

[PublicAPI]
public abstract class FluentRecordBuilder<TBuilder, TRecord> : BuilderBase<TBuilder>
    where TBuilder : FluentRecordBuilder<TBuilder, TRecord>
    where TRecord : class
{
    public static implicit operator TRecord(FluentRecordBuilder<TBuilder, TRecord> builder) => builder.TryGetRecord().OkOrThrow();

    protected TRecord? _record;

    protected FluentRecordBuilder()
    {

    }

    protected FluentRecordBuilder(TRecord? record)
    {
        _record = record;
    }

    protected virtual Validations GetValidations() => new()
    {
        Validate.IsNotNull(_record),
    };

    public Result<TRecord> TryGetRecord()
        => GetValidations().ToResult(_record!);
}
