using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace System.Web.Mvc
{
    public class StringLengthLocalizedAttributeAdapter : DataAnnotationsModelValidator<StringLengthLocalizedAttribute>
    {
        public StringLengthLocalizedAttributeAdapter(ModelMetadata metadata, ControllerContext context, StringLengthLocalizedAttribute attribute)
            : base(metadata, context, attribute)
        {
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
        {
            var errorMessage = Attribute.ErrorMessage ?? Attribute.FormatErrorMessage(Metadata.DisplayName);

            return new[] { new ModelClientValidationStringLengthRule(errorMessage, Attribute.MinimumLength, Attribute.MaximumLength) };
        }
    }
}