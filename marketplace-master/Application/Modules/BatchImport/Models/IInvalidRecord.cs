namespace Application.Modules.BatchImport.Models
{
    public interface IInvalidRecord
    {
        IInvalidRecordField[] Fields { get; }
    }

    public interface IInvalidRecordField
    {
        string Name { get; }

        object Value { get; }

        string[] Errors { get; }

        bool HasErrors { get; }
    }
}
