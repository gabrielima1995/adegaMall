using ExpressiveAnnotations.MvcUnobtrusive.Validators;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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