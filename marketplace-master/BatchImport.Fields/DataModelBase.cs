using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BatchImport.Fields
{
    public abstract class DataModelBase : IDataModel
    {
        public abstract ReadOnlyCollection<IField> ListFields();

        public virtual ReadOnlyCollection<string> GetSampleRecord()
        {
            var values = ListFields().Select(x => x.Sample).ToList();
            return new ReadOnlyCollection<string>(values);
        }

        public virtual ReadOnlyCollection<object> ParseRecord(IDataRecord record)
        {
            var fields = ListFields().ToArray();
            var values = new object[fields.Length];

            for (int i = 0; i < fields.Length; i++)
                values[i] = fields[i].Parse(record[i]);

            return new ReadOnlyCollection<object>(values);
        }

        public virtual RecordValidation ValidateRecord(IDataRecord record)
        {
            var fields = ListFields().ToArray();
            var fieldValidations = new List<FieldValidation>(fields.Length);
            var loopLength = Math.Min(record.FieldCount, fields.Length);

            for (int i = 0; i < loopLength; i++)
            {
                var validation = fields[i].Validate(record[i]);
                fieldValidations.Add(validation);
            }

            return new RecordValidation(fieldValidations);
        }
    }
}
