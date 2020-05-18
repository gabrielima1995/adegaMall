using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace System.Web.Mvc
{
    public class DocumentNumberLocalizedAttributeAdapter : DataAnnotationsModelValidator<DocumentNumberLocalizedAttribute>
    {
        public DocumentNumberLocalizedAttributeAdapter(ModelMetadata metadata, ControllerContext context, DocumentNumberLocalizedAttribute attribute)
            : base(metadata, context, attribute)
        {
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
        {
            var errorMessage = Attribute.ErrorMessage ?? Attribute.FormatErrorMessage(Metadata.DisplayName);

            return new[] { new ModelClientValidationRule() { ErrorMessage = errorMessage, ValidationType = "documentnumber" } };
        }
    }
}