using BatchImport;
using BatchImport.Fields;
using System.Collections.Generic;

namespace Infra.BatchImportAdapter.Internals
{
    internal abstract class DataModelAndParser<TParseResult> : DataModelBase, IDataRecordParser<TParseResult>
    {
        public abstract TParseResult ParseRecord(IEnumerable<IField> fields, IDataRecord record);
    }

    internal interface IDataRecordParser<TParseResult>
    {
        TParseResult ParseRecord(IEnumerable<IField> fields, IDataRecord record);
    }
}
