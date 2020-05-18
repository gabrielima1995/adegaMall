namespace Infra.Crosscutting.Email
{
    public class EmailMessage : IEmailMessage
    {
        public string Body { get; set; }

        public string Destination { get; set; }

        public string Subject { get; set; }

        public string[] Cc { get; set; }

        public string[] Bcc { get; set; }
    }
}