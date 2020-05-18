using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace System.Web.Mvc
{
    public class RangeLocalizedAttributeAdapter : DataAnnotationsModelValidator<RangeLocalizedAttribute>
    {
        public RangeLocalizedAttributeAdapter(ModelMetadata metadata, ControllerContext context, RangeLocalizedAttribute attribute)
            : base(metadata, context, attribute)
        {
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
        {
            var errorMessage = Attribute.ErrorMessage ?? Attribute.FormatErrorMessage(Metadata.DisplayName);

            return new[] { new ModelClientValidationRangeRule(errorMessage, Attribute.Minimum, Attribute.Maximum) };
        }
    }
}