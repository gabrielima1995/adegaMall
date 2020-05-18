using Domain.Messages;
using Framework;

namespace Domain.Interfaces
{
    public interface IEmailSender
    {
        OperationResult Send(EmailMessage emailMessage);
    }
}