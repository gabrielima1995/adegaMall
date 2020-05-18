using System.Threading.Tasks;

namespace Infra.Crosscutting.Email
{
    public interface IEmailService
    {
        Task DeliverAsync(EmailMessage message);

        Task DeliverAsync(EmailMessage message, IEmailConfig config);

        void Deliver(EmailMessage message);

        void Deliver(EmailMessage message, IEmailConfig config);
    }
}