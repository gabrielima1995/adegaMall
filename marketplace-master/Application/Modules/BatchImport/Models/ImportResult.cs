using System.Collections.Generic;
using System.Linq;

namespace Application.Modules.BatchImport.Models
{
    public class ImportResult<T>
    {
        public ImportResult() : this(new string[0], new T[0], new IInvalidRecord[0])
        {
        }

        public ImportResult(IEnumerable<string> fieldNames, IEnumerable<T> validRecords, IEnumerable<IInvalidRecord> invalidRecords)
        {
            FieldNames = fieldNames;
            ValidRecords = validRecords;
            InvalidRecords = invalidRecords;
        }

        public IEnumerable<string> FieldNames { get; }
        public IEnumerable<T> ValidRecords { get; }
        public IEnumerable<IInvalidRecord> InvalidRecords { get; }
        public bool HasErrors => InvalidRecords.Any();
        public bool IsValid => !HasErrors;
    }
}
