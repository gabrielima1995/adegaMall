using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Domain.Modules.BatchImport
{
    public class ImportExportMetadata
    {
        private readonly List<Field> fields;

        public ImportExportMetadata(IEnumerable<Field> fields)
        {
            this.fields = fields.ToList();
        }

        public ReadOnlyCollection<Field> Fields => fields.AsReadOnly();

        public ReadOnlyCollection<string> GetSampleRecord()
        {
            return fields.Select(field => field.SampleValue).ToList().AsReadOnly();
        }
    }
}
