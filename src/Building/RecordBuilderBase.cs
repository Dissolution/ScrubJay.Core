namespace ScrubJay.Building;

[PublicAPI]
public abstract class RecordBuilderBase<B, R> : BuilderBase<B>
    where B : RecordBuilderBase<B, R>
    where R : class
{
    public static implicit operator R(RecordBuilderBase<B, R> builder) => builder.TryGetRecord().OkOrThrow();

    protected R? _record;

    protected RecordBuilderBase()
    {

    }

    protected RecordBuilderBase(R? record)
    {
        _record = record;
    }

    protected virtual Validations GetValidations() => new()
    {
        Validate.IsNotNull(_record),
    };

    public Result<R> TryGetRecord()
        => GetValidations().ToResult(_record!);
}
