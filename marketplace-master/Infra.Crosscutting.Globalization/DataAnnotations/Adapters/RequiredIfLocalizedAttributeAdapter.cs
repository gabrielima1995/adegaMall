using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ExpressiveAnnotations;
using ExpressiveAnnotations.MvcUnobtrusive.Validators;

namespace System.Web.Mvc
{
    /// <summary>
    ///     Model validator for <see cref="RequiredIfLocalizedAttribute" />.
    /// </summary>
    public class RequiredIfLocalizedAttributeAdapter : ExpressiveValidator<RequiredIfLocalizedAttribute>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RequiredIfLocalizedAttributeAdapter" /> class.
        /// </summary>
        /// <param name="metadata">The model metadata instance.</param>
        /// <param name="context">The controller context instance.</param>
        /// <param name="attribute">The expressive requirement attribute instance.</param>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="System.ComponentModel.DataAnnotations.ValidationException"></exception>
        public RequiredIfLocalizedAttributeAdapter(ModelMetadata metadata, ControllerContext context, RequiredIfLocalizedAttribute attribute)
            : base(metadata, context, attribute)
        {
            AllowEmpty = attribute.AllowEmptyStrings;

            try
            {
                var propType = metadata.ModelType;
                if (propType.IsNonNullableValueType())
                    throw new InvalidOperationException(
                        $"{nameof(RequiredIfLocalizedAttribute)} has no effect when applied to a field of non-nullable value type '{propType.FullName}'. Use nullable '{propType.FullName}?' version instead.");
            }
            catch (Exception e)
            {
                throw new ValidationException(
                    $"{GetType().Name}: validation applied to {metadata.PropertyName} field failed.", e);
            }
        }

        private bool AllowEmpty { get; set; }

        /// <summary>
        ///     Retrieves a collection of client validation rules (which are next sent to browsers).
        /// </summary>
        /// <returns>
        ///     A collection of client validation rules.
        /// </returns>
        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
        {
            var rule = GetBasicRule("requiredif");
            rule.ValidationParameters.Add("allowempty", AllowEmpty.ToJson());
            yield return rule;
        }
    }
}