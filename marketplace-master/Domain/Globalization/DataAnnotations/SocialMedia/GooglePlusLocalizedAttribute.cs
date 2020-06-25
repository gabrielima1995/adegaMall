namespace System.ComponentModel.DataAnnotations
{
    public class GooglePlusLocalizedAttribute : RegularExpressionLocalizedAttribute
    {
        public GooglePlusLocalizedAttribute(string stringLabel = "GOOGLEPLUS_REGEX_ERROR")
            : base(@"(^$|^https?:\/\/plus\.google\.com\/\+[^/]+|\d{21}$)", stringLabel)
        {
        }
    }
}