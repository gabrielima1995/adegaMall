using CSharpFunctionalExtensions;
using System.Collections.Generic;

namespace NewContext.Domain.Languages
{
    public class Label : ValueObject
    {
        private Label(string value)
        {
            Value = value.ToUpper();
        }

        public string Value { get; private set; }

        public static Result<Label> Create(string value)
        {
            return Result.Ok(new Label(value));
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;

        public static implicit operator string(Label label) => label.Value;

        public static explicit operator Label(string value) => Create(value).Value;
    }
}
