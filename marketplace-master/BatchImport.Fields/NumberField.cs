using System;

namespace BatchImport.Fields
{
    public abstract class NumberField<T> : FieldBase where T : struct, IComparable<T>
    {
        protected string InvalidTypeErrorMessage { get; set; } = "Não é um número válido";

        public T? MinValue { get; }

        public T? MaxValue { get; }

        public NumberField(string name, string displayName, string sample, bool isMandatory = false, T? minValue = null, T? maxValue = null)
            : base(name, displayName, sample, isMandatory)
        {
            MinValue = minValue;
            MaxValue = maxValue;
        }

        protected abstract T ClrParse(string input);

        protected abstract bool ClrTryParse(string input, out T output);

        public override object Parse(string input)
        {
            if (input == null) return null;
            return ClrParse(input);
        }

        public override FieldValidation Validate(string input)
        {
            var validation = base.Validate(input);

            var parsed = default(T);
            var isDecimal = input != null && ClrTryParse(input, out parsed);

            if (input != null && !isDecimal)
                validation.AddError(InvalidTypeErrorMessage);
            else
            {
                if (input != null && MaxValue != null && parsed.CompareTo(MaxValue.Value) > 0)
                    validation.AddError($"Não pode ser maior que '{MaxValue}'");

                if (input != null && MinValue != null && parsed.CompareTo(MinValue.Value) < 0)
                    validation.AddError($"Não pode ser menor que '{MinValue}'");
            }

            return validation;
        }
    }
}
