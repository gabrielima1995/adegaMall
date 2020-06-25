using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Threading;

using DataAnnotationsCompareLocalizedAttribute = System.ComponentModel.DataAnnotations.CompareLocalizedAttribute;

namespace System.Web.Mvc
{
    public class CompareLocalizedAttributeAdapter : DataAnnotationsModelValidator<DataAnnotationsCompareLocalizedAttribute>
    {
        public CompareLocalizedAttributeAdapter(ModelMetadata metadata, ControllerContext context, DataAnnotationsCompareLocalizedAttribute attribute)
            : base(metadata, context, new CompareAttributeWrapper(attribute, metadata))
        {
            Contract.Assert(attribute.GetType() == typeof(DataAnnotationsCompareLocalizedAttribute));
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
        {
            yield return new ModelClientValidationEqualToRule(Attribute.ErrorMessage ?? CultureInfo.CurrentCulture.GetStringLocalized(Attribute.StringLabel), FormatPropertyForClientValidation(Attribute.OtherProperty));
        }

        private static string FormatPropertyForClientValidation(string property)
        {
            Contract.Assert(property != null);

            return "*." + property;
        }

        // Wrapper for CompareAttribute that will eagerly get the OtherPropertyDisplayName and use it on the error message for client validation.
        // The System.ComponentModel.DataAnnotations.CompareAttribute doesn't populate the OtherPropertyDisplayName until after IsValid()
        // is called. Therefore, by the time we get the error message for client validation, the display name is not populated and won't be used.
        private sealed class CompareAttributeWrapper : DataAnnotationsCompareLocalizedAttribute
        {
            private readonly string _otherPropertyDisplayName;

            public CompareAttributeWrapper(DataAnnotationsCompareLocalizedAttribute attribute, ModelMetadata metadata)
                : base(attribute.OtherProperty, attribute.StringLabel)
            {
                _otherPropertyDisplayName = attribute.OtherPropertyDisplayName;
                if (_otherPropertyDisplayName == null && metadata.ContainerType != null)
                {
                    _otherPropertyDisplayName = ModelMetadataProviders.Current.GetMetadataForProperty(() => metadata.Model, metadata.ContainerType, attribute.OtherProperty).GetDisplayName();
                }

                if (_otherPropertyDisplayName == null)
                {
                    _otherPropertyDisplayName = attribute.OtherProperty;
                }

                // Copy settable properties from wrapped attribute. Don't reset default message accessor (set as
                // CompareAttribute constructor calls ValidationAttribute constructor) when all properties are null to
                // preserve default error message. Reset the message accessor when just ErrorMessageResourceType is
                // non-null to ensure correct InvalidOperationException.
                if (!String.IsNullOrEmpty(attribute.ErrorMessage) ||
                    !String.IsNullOrEmpty(attribute.ErrorMessageResourceName) ||
                    attribute.ErrorMessageResourceType != null)
                {
                    //ErrorMessage = attribute.ErrorMessage;
                    ErrorMessage = CultureInfo.CurrentCulture.GetStringLocalized(attribute.StringLabel);
                    ErrorMessageResourceName = attribute.ErrorMessageResourceName;
                    ErrorMessageResourceType = attribute.ErrorMessageResourceType;
                }
            }

            public override string FormatErrorMessage(string name)
            {
                return String.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, _otherPropertyDisplayName);
            }
        }
    }
}