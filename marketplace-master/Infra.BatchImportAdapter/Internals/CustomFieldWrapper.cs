using BatchImport;

namespace Infra.BatchImportAdapter.Internals
{
    /// <summary>
    /// Wrapper to identify an IField as being a CustomField (Domain).
    /// </summary>
    internal class CustomFieldWrapper : IField
    {
        private readonly IField _wrappedField;

        public CustomFieldWrapper(IField wrappedField)
        {
            _wrappedField = wrappedField;
        }

        public string Name => _wrappedField.Name;

        public string DisplayName => _wrappedField.DisplayName;

        public string Sample => _wrappedField.Sample;

        public object Parse(string input) => _wrappedField.Parse(input);

        public FieldValidation Validate(string input) => _wrappedField.Validate(input);
    }
}
