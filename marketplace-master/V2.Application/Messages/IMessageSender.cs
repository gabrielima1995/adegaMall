using Domain.Messages;
using Framework;

namespace V2.Application.Messages
{
    public interface IMessageSender
    {
        OperationResult Send(EmailMessage message);
    }
}
