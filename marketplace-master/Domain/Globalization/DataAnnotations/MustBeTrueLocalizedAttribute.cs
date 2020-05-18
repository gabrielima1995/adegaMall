using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;

namespace System.ComponentModel.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public class MustBeTrueLocalizedAttribute : ValidationAttribute, IClientValidatable
    {
        public string StringLabel { get; }

        public MustBeTrueLocalizedAttribute(string stringLabel = "MUST_BE_TRUE_ERROR")
            : base()
        {
            StringLabel = stringLabel;
        }


        public override bool IsValid(object value)
        {
            return value is bool b && b;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var modelClientValidationRule = new ModelClientValidationRule
            {
                ValidationType = "mustbetrue",
                ErrorMessage = FormatErrorMessage(metadata.DisplayName)
            };

            yield return modelClientValidationRule;
        }

        public override string FormatErrorMessage(string name)
        {
            base.FormatErrorMessage(name);

            return string.Format(CultureInfo.CurrentCulture, CultureInfo.CurrentCulture.GetStringLocalized(StringLabel), name);
        }
    }
}