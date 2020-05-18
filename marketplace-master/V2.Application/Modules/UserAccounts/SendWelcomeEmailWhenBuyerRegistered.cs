using Application.Config;
using Domain.Events;
using V2.Application.Messages;
using V2.Application.Modules.UserAccounts.Messages.Model;
using V2.Application.Modules.UserAccounts.Messages.Specs;

namespace V2.Application.Modules.UserAccounts
{
    public class SendWelcomeEmailWhenBuyerRegistered : IDomainEventHandler<BuyerRegisteredEvent>
    {
        public void Handle(BuyerRegisteredEvent domainEvent)
        {
            MessagesContext.Current.GenerateAndSend<WelcomeMessageSpecification, WelcomeMessageModel>(new WelcomeMessageModel()
            {
                UserName = domainEvent.Buyer.Name,
                Url = MaaSConfig.Current.Environment.AppUrl + "account/forgotpassword/",
            }, MaaSConfig.Current.Environment.Smtp.DefaultFromAddress, new[] { domainEvent.Buyer.Email }, domainEvent.Buyer.Lang);
        }
    }
}
