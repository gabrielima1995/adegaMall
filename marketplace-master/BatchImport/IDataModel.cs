using System.Collections.ObjectModel;

namespace BatchImport
{
    public interface IDataModel
    {
        ReadOnlyCollection<string> GetSampleRecord();
        ReadOnlyCollection<IField> ListFields();
        RecordValidation ValidateRecord(IDataRecord record);
    }
}
