namespace Infra.Crosscutting.Email
{
    public interface IEmailMessage
    {
        string Body { get; set; }

        string Destination { get; set; }

        string Subject { get; set; }

        string[] Cc { get; set; }

        string[] Bcc { get; set; }
    }
}