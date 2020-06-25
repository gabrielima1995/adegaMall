using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace System.Web.Mvc
{
    public class EmailAddressLocalizedAttributeAdapter : DataAnnotationsModelValidator<EmailAddressLocalizedAttribute>
    {
        public EmailAddressLocalizedAttributeAdapter(ModelMetadata metadata, ControllerContext context, EmailAddressLocalizedAttribute attribute)
            : base(metadata, context, attribute)
        {
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
        {
            var errorMessage = Attribute.ErrorMessage ?? Attribute.FormatErrorMessage(Metadata.DisplayName);

            return new[] { new ModelClientValidationRule
            {
                ValidationType = "email",
                ErrorMessage = errorMessage
            } };
        }
    }
}