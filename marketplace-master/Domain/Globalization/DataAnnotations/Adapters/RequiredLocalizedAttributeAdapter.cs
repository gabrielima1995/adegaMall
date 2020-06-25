using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace System.Web.Mvc
{
    public class RequiredLocalizedAttributeAdapter : DataAnnotationsModelValidator<RequiredLocalizedAttribute>
    {
        public RequiredLocalizedAttributeAdapter(ModelMetadata metadata, ControllerContext context, RequiredLocalizedAttribute attribute)
            : base(metadata, context, attribute)
        {
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
        {
            var errorMessage = Attribute.ErrorMessage ?? Attribute.FormatErrorMessage(Metadata.DisplayName);

            return new[] { new ModelClientValidationRequiredRule(errorMessage) };
        }
    }
}