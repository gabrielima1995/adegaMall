using ExpressiveAnnotations.MvcUnobtrusive.Validators;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace System.Web.Mvc
{
    /// <summary>
    ///     Model validator for <see cref="MustBeTrueIfLocalizedAttribute" />.
    /// </summary>
    public class MustBeTrueIfLocalizedAttributeAdapter : ExpressiveValidator<MustBeTrueIfLocalizedAttribute>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MustBeTrueIfLocalizedAttributeAdapter" /> class.
        /// </summary>
        /// <param name="metadata">The model metadata instance.</param>
        /// <param name="context">The controller context instance.</param>
        /// <param name="attribute">The expressive requirement attribute instance.</param>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="System.ComponentModel.DataAnnotations.ValidationException"></exception>
        public MustBeTrueIfLocalizedAttributeAdapter(ModelMetadata metadata, ControllerContext context, MustBeTrueIfLocalizedAttribute attribute)
            : base(metadata, context, attribute)
        {
        }

        /// <summary>
        ///     Retrieves a collection of client validation rules (which are next sent to browsers).
        /// </summary>
        /// <returns>
        ///     A collection of client validation rules.
        /// </returns>
        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
        {
            var rule = GetBasicRule("mustbetrue");
            yield return rule;
        }
    }
}