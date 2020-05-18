namespace System.ComponentModel.DataAnnotations
{
    public class FacebookLocalizedAttribute : RegularExpressionLocalizedAttribute
    {
        public FacebookLocalizedAttribute(string stringLabel = "FACEBOOK_REGEX_ERROR") 
            : base(@"(^$|^http(s)?:\/\/(www\.)?(facebook|fb)\.com\/[A-z 0-9 _ - \.]+\/?$)", stringLabel)
        {
        }
    }
}