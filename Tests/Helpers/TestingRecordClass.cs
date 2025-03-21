namespace ScrubJay.Tests.Helpers;

[PublicAPI]
internal record class TestingRecordClass(int Id, string Name)
{
    public Guid InstanceGuid { get; } = Guid.NewGuid();
}

[PublicAPI]
internal readonly record struct TestingReadonlyRecordStruct(int Id, string Name)
{
    public Guid InstanceGuid { get; } = Guid.NewGuid();
}
