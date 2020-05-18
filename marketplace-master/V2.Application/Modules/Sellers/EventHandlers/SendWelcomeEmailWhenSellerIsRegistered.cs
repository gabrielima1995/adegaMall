using Application.Config;
using Domain.Events;
using V2.Application.Messages;
using V2.Application.Modules.UserAccounts.Messages.Model;
using V2.Application.Modules.UserAccounts.Messages.Specs;

namespace V2.Application.Modules.Sellers.EventHandlers
{
    public class SendWelcomeEmailWhenSellerIsRegistered : IDomainEventHandler<SellerRegisteredEvent>
    {
        public void Handle(SellerRegisteredEvent domainEvent)
        {
            var messageModel = new WelcomeMessageModel()
            {
                UserName = domainEvent.SellerPerson.Name,
                Url = MaaSConfig.Current.Environment.AppUrl + "account/forgotpassword/",
            };

            MessagesContext.Current.GenerateAndSend<WelcomeMessageToSellerSpecification, WelcomeMessageModel>(
                messageModel,
                MaaSConfig.Current.Environment.Smtp.DefaultFromAddress,
                new[] { domainEvent.SellerPerson.Email }, domainEvent.SellerPerson.Lang);
        }
    }
}
