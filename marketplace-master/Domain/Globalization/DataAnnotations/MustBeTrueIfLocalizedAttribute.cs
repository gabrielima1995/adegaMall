using ExpressiveAnnotations.Attributes;
using System.Globalization;

namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    ///     Validation attribute, executed for null-only annotated field, which indicates that such a field
    ///     is required to be non-null, when computed result of given logical expression is true.
    /// </summary>
    public sealed class MustBeTrueIfLocalizedAttribute : ExpressiveAttribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MustBeTrueIfLocalizedAttribute" /> class.
        /// </summary>
        /// <param name="expression">The logical expression based on which requirement condition is computed.</param>
        /// <param name="stringLabel">Label of string localization.</param>
        public MustBeTrueIfLocalizedAttribute(string expression, string stringLabel = "MUST_BE_TRUE_ERROR")
            : base(expression, () => CultureInfo.CurrentCulture.GetStringLocalized(stringLabel))
        {
        }

        /// <summary>
        ///     Validates a specified value with respect to the associated validation attribute.
        ///     Internally used by the <see cref="ExpressiveAttribute.IsValid(object,System.ComponentModel.DataAnnotations.ValidationContext)" /> method.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The validation context.</param>
        /// <returns>
        ///     An instance of the <see cref="T:System.ComponentModel.DataAnnotations.ValidationResult" /> class.
        /// </returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        protected override ValidationResult IsValidInternal(object value, ValidationContext validationContext)
        {
            //AssertNonValueType(value);

            if (!(value is bool b && b))
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