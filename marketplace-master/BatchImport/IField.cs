namespace BatchImport
{
    public interface IField
    {
        string Name { get; }
        string DisplayName { get; }
        string Sample { get; }

        object Parse(string input);
        FieldValidation Validate(string input);
    }
}
