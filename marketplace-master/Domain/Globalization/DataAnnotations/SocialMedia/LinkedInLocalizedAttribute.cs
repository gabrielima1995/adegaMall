namespace System.ComponentModel.DataAnnotations
{
    public class LinkedInLocalizedAttribute : RegularExpressionLocalizedAttribute
    {
        public LinkedInLocalizedAttribute(string stringLabel = "LINKEDIN_REGEX_ERROR")
            : base(@"(https?:\/\/)?[a-z]{2,3}\.linkedin\.com\/.*$", stringLabel)
        {
        }
    }
}