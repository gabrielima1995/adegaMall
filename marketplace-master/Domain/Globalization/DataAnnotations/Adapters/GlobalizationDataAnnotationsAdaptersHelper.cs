using System.ComponentModel.DataAnnotations;

namespace System.Web.Mvc
{
    public static class GlobalizationDataAnnotationsAdaptersHelper
    {
        public static void RegisterAdapters()
        {
            DataAnnotationsModelValidatorProvider.RegisterAdapter(
                typeof(StringLengthLocalizedAttribute),
                typeof(StringLengthLocalizedAttributeAdapter));

            DataAnnotationsModelValidatorProvider.RegisterAdapter(
                typeof(CompareLocalizedAttribute),
                typeof(CompareLocalizedAttributeAdapter));

            DataAnnotationsModelValidatorProvider.RegisterAdapter(
                typeof(RequiredLocalizedAttribute),
                typeof(RequiredLocalizedAttributeAdapter));

            DataAnnotationsModelValidatorProvider.RegisterAdapter(
                typeof(EmailAddressLocalizedAttribute),
                typeof(EmailAddressLocalizedAttributeAdapter));

            DataAnnotationsModelValidatorProvider.RegisterAdapter(
                typeof(RegularExpressionLocalizedAttribute),
                typeof(RegularExpressionLocalizedAttributeAdapter));

            DataAnnotationsModelValidatorProvider.RegisterAdapter(
                typeof(NicknameLocalizedAttribute),
                typeof(NicknameLocalizedAttributeAdapter));

            DataAnnotationsModelValidatorProvider.RegisterAdapter(
                typeof(RequiredIfLocalizedAttribute),
                typeof(RequiredIfLocalizedAttributeAdapter));

            DataAnnotationsModelValidatorProvider.RegisterAdapter(
                typeof(DocumentNumberLocalizedAttribute),
                typeof(DocumentNumberLocalizedAttributeAdapter));

            DataAnnotationsModelValidatorProvider.RegisterAdapter(
                typeof(RangeLocalizedAttribute),
                typeof(RangeLocalizedAttributeAdapter));

            DataAnnotationsModelValidatorProvider.RegisterAdapter(
                typeof(MustBeTrueIfLocalizedAttribute),
                typeof(MustBeTrueIfLocalizedAttributeAdapter));
        }
    }
}