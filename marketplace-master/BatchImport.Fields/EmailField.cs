namespace BatchImport.Fields
{
    public class EmailField : RegexField
    {
        private const string Pattern = @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";

        public EmailField(string name, string displayName, string sample, bool isMandatory = false, string errorMessage = "Deve ser um email válido")
            : base(name, displayName, sample, Pattern, isMandatory, errorMessage) { }
    }
}
