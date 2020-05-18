namespace System.ComponentModel.DataAnnotations
{
    public class InstagramLocalizedAttribute : RegularExpressionLocalizedAttribute
    {
        public InstagramLocalizedAttribute(string stringLabel = "INSTAGRAM_REGEX_ERROR")
            : base(@"(^$|^https?:\/\/(www\.)?instagram\.com\/([A-Za-z0-9_](?:(?:[A-Za-z0-9_]|(?:\.(?!\.))){0,28}(?:[A-Za-z0-9_]))?\/?)$)", stringLabel)
        {
        }
    }
}