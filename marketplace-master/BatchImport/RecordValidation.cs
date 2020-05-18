using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BatchImport
{
    public class RecordValidation
    {
        public RecordValidation(IEnumerable<FieldValidation> fields) => Fields = new ReadOnlyCollection<FieldValidation>(fields.ToList());

        public ReadOnlyCollection<FieldValidation> Fields { get; }

        public bool IsValid => Fields.All(i => i.IsValid);
    }
}
