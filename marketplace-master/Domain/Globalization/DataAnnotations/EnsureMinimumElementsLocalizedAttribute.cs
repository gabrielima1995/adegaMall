using ExpressiveAnnotations.Attributes;
using System.Collections;
using System.Globalization;

namespace System.ComponentModel.DataAnnotations
{
    public class EnsureMinimumElementsLocalizedAttribute : ValidationAttribute
    {
        public EnsureMinimumElementsLocalizedAttribute(string stringLabel = "REQUIRED_ERROR", int minElements = 1)
            : base(CultureInfo.CurrentCulture.GetStringLocalized(stringLabel))
        {
            MinElements = minElements;
        }

        public int MinElements { get; }

        public override bool IsValid(object value)
        {
            if (value is IList list)
                return list.Count >= MinElements;

            return false;
        }
    }

    public class EnsureMinimumElementsIfLocalizedAttribute : ExpressiveAttribute
    {
        public EnsureMinimumElementsIfLocalizedAttribute(string expression, string stringLabel = "REQUIRED_ERROR", int minElements = 1)
            : base(expression, () => CultureInfo.CurrentCulture.GetStringLocalized(stringLabel))
        {
            MinElements = minElements;
        }

        public int MinElements { get; }

        protected override ValidationResult IsValidInternal(object value, ValidationContext validationContext)
        {
            var isValid = value is IList list && list.Count >= MinElements;

            if (!isValid)
            {
                Compile(validationContext.ObjectType);
                if (CachedValidationFuncs[validationContext.ObjectType](validationContext.ObjectInstance)) // check if the requirement condition is satisfied
                    return new ValidationResult( // requirement confirmed => notify
                        FormatErrorMessage(validationContext.DisplayName, Expression, validationContext.ObjectInstance),
                        new[] { validationContext.MemberName });
            }

            return ValidationResult.Success;
        }
    }
}