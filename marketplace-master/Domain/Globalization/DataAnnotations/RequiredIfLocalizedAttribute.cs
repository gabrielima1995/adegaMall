using ExpressiveAnnotations.Attributes;
using System.Globalization;

namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    ///     Validation attribute, executed for null-only annotated field, which indicates that such a field
    ///     is required to be non-null, when computed result of given logical expression is true.
    /// </summary>
    public sealed class RequiredIfLocalizedAttribute : ExpressiveAttribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RequiredIfLocalizationAttribute" /> class.
        /// </summary>
        /// <param name="expression">The logical expression based on which requirement condition is computed.</param>
        /// <param name="stringLabel">Label of string localization.</param>
        public RequiredIfLocalizedAttribute(string expression, string stringLabel = "REQUIRED_ERROR")
            : base(expression, () => CultureInfo.CurrentCulture.GetStringLocalized(stringLabel))
        {
            AllowEmptyStrings = false;
        }

        /// <summary>
        ///     Gets or sets a flag indicating whether the attribute should allow empty or whitespace strings.
        /// </summary>
        /// <remarks>
        ///     <c>false</c> by default.
        /// </remarks>
        public bool AllowEmptyStrings { get; set; }

        /// <summary>
        ///     Validates a specified value with respect to the associated validation attribute.
        ///     Internally used by the <see cref="ExpressiveAttribute.IsValid(object, ValidationContext)" /> method.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The validation context.</param>
        /// <returns>
        ///     An instance of the <see cref="T:System.ComponentModel.DataAnnotations.ValidationResult" /> class.
        /// </returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        protected override ValidationResult IsValidInternal(object value, ValidationContext validationContext)
        {
            var isEmpty = value is string
                ? string.IsNullOrWhiteSpace((string)value)
                : value == GetDefaultValue(PropertyType);

            if (value == null || (isEmpty && !AllowEmptyStrings))
            {
                Compile(validationContext.ObjectType);
                if (CachedValidationFuncs[validationContext.ObjectType](validationContext.ObjectInstance)) // check if the requirement condition is satisfied
                    return new ValidationResult( // requirement confirmed => notify
                        FormatErrorMessage(validationContext.DisplayName, Expression, validationContext.ObjectInstance),
                        new[] { validationContext.MemberName });
            }

            return ValidationResult.Success;
        }

        private object GetDefaultValue(Type t)
        {
            if (t.IsValueType) return Activator.CreateInstance(t);
            return null;
        }
    }
}