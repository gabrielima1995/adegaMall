namespace Infra.Crosscutting.Email
{
    public interface IEmailConfig
    {
        string FromAddress { get; }

        string FromDisplayName { get; }

        string Password { get; }

        string UserName { get; }
    }
}