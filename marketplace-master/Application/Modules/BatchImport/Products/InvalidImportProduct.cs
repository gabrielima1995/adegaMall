using Application.Modules.BatchImport.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Modules.BatchImport.Products
{
    internal class InvalidImportProduct : IInvalidRecord
    {
        public InvalidImportProduct(IEnumerable<InvalidImportField> invalidFields)
        {
            if (invalidFields == null || invalidFields.Count() == 0)
                throw new ArgumentNullException(nameof(invalidFields), $"Cannot create an instance of {nameof(InvalidImportProduct)} with null or zero invalid fields.");

            Fields = invalidFields.ToArray();
        }

        public IInvalidRecordField[] Fields { get; }
    }

    internal class InvalidImportField : IInvalidRecordField
    {
        private InvalidImportField(string name, object value, string[] errors)
        {
            Name = name;
            Value = value;
            Errors = errors;
        }

        public static InvalidImportField ValidField(string name, object value)
        {
            return new InvalidImportField(name, value, new string[0]);
        }

        public static InvalidImportField InvalidField(string name, object value, string error)
        {
            return new InvalidImportField(name, value, new[] { error });
        }

        public string Name { get; }
        public object Value { get; }
        public string[] Errors { get; }
        public bool HasErrors => Errors.Any();
    }
}
