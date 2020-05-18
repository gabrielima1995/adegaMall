namespace System.ComponentModel.DataAnnotations
{
    public class NicknameLocalizedAttribute : RegularExpressionLocalizedAttribute
    {
        public NicknameLocalizedAttribute(string stringLabel = "NICKNAME_ERROR") : base("^[a-zA-Z0-9\\._\\-]+$", stringLabel)
        {
        }
    }
}