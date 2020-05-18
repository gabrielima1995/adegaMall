using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BatchImport
{
    public class FieldValidation
    {
        private readonly List<string> errors = new List<string>();

        public FieldValidation(string fieldName, object value) : this(fieldName, value, null)
        {
        }

        public FieldValidation(string fieldName, object value, IEnumerable<string> errors)
        {
            FieldName = fieldName;
            Value = value;
            AddErrors(errors);
        }

        public string FieldName { get; }
        public object Value { get; }

        public ReadOnlyCollection<string> Errors => new ReadOnlyCollection<string>(errors);
        public bool IsValid => !IsFaulted;
        public bool IsFaulted => errors.Any();

        public void AddError(string error)
        {
            errors.Add(error);
        }

        public void AddErrors(IEnumerable<string> errors)
        {
            if (errors != null)
                foreach (var e in errors)
                    this.errors.Add(e);
        }
    }
}
