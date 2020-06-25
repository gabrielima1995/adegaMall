using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace System.Web.Mvc
{
    public class RegularExpressionLocalizedAttributeAdapter : DataAnnotationsModelValidator<RegularExpressionLocalizedAttribute>
    {
        public RegularExpressionLocalizedAttributeAdapter(ModelMetadata metadata, ControllerContext context, RegularExpressionLocalizedAttribute attribute)
            : base(metadata, context, attribute)
        {
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
        {
            var errorMessage = Attribute.ErrorMessage ?? Attribute.FormatErrorMessage(Metadata.DisplayName);

            return new[] { new ModelClientValidationRegexRule(errorMessage, Attribute.Pattern) };
        }
    }
}