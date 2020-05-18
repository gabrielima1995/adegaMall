namespace System.ComponentModel.DataAnnotations
{
    public class LinkedInLocalizedAttribute : RegularExpressionLocalizedAttribute
    {
        public LinkedInLocalizedAttribute(string stringLabel = "LINKEDIN_REGEX_ERROR")
            : base(@"(^$|^http(s)?:\/\/([\w]+\.)?linkedin\.com\/in\/[A-z 0-9 _ -]+\/?$|^http(s)?:\/\/([\w]+\.)?linkedin\.com\/pub\/[A-z 0-9 _ -]+(\/[A-z 0-9]+){3}\/?$)", stringLabel)
        {
        }
    }
}