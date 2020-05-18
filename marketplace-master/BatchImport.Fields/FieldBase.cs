namespace BatchImport.Fields
{
    public abstract class FieldBase : IField
    {
        public bool IsMandatory { get; protected set; }

        public virtual string Name { get; protected set; }

        public virtual string DisplayName { get; protected set; }

        public virtual string Sample { get; protected set; }

        public FieldBase(string name, string displayName, string sample, bool isMandatory = false)
        {
            Name = name;
            DisplayName = displayName;
            Sample = sample;
            IsMandatory = isMandatory;
        }

        public abstract object Parse(string input);

        public virtual FieldValidation Validate(string input)
        {
            var validation = new FieldValidation(Name, input);

            if (IsMandatory && input == null)
                validation.AddError("É obrigatório");

            return validation;
        }

        public override string ToString()
        {
            return $"{GetType().Name}: {Name}";
        }
    }
}
