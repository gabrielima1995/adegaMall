using Application.Modules.BatchImport.Models;
using BatchImport;
using System.Linq;

namespace Infra.BatchImportAdapter.Internals
{
    internal class InvalidRecord : IInvalidRecord
    {
        public IInvalidRecordField[] Fields { get; }

        private InvalidRecord(IInvalidRecordField[] fields)
        {
            Fields = fields;
        }

        public static InvalidRecord Create(RecordValidation validation)
        {
            var fields = validation.Fields
                .Select(f => InvalidRecordField.Create(f))
                .ToArray();

            return new InvalidRecord(fields);
        }
    }

    internal class InvalidRecordField : IInvalidRecordField
    {
        public string Name { get; }

        public object Value { get; }

        public string[] Errors { get; }

        public bool HasErrors { get; }

        private InvalidRecordField(string name, object value, string[] errors)
        {
            Name = name;
            Value = value;
            Errors = errors;
            HasErrors = errors.Any();
        }

        public static InvalidRecordField Create(FieldValidation validation)
        {
            return new InvalidRecordField(validation.FieldName, validation.Value, validation.Errors.ToArray());
        }
    }
}
