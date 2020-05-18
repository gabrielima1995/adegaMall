using System;
using System.Threading.Tasks;

namespace BatchImport
{
    public interface IDataReader : IDataRecord, IDisposable
    {
        bool ReadNext();
        Task<bool> ReadNextAsync();
    }

    public interface IDataRecord
    {
        int FieldCount { get; }
        string this[int i] { get; }
        string this[string name] { get; }
    }
}
