namespace Domain.DTO
{
    public class MessageTemplateEditing
    {
        public string Key { get; set; }

        public string Lang { get; set; }

        public string SubjectTemplate { get; set; }

        public string BodyTemplate { get; set; }

        public bool Enabled { get; set; }
    }
}