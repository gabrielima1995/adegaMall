using Application.Config;
using Domain.Events;
using V2.Application.Messages;
using V2.Application.Modules.UserAccounts.Messages.Model;
using V2.Application.Modules.UserAccounts.Messages.Specs;

namespace V2.Application.Modules.Sellers.EventHandlers
{
    public class SendEmailToSellerWhenEnabled : IDomainEventHandler<SellerEnabledEvent>
    {
        public void Handle(SellerEnabledEvent domainEvent)
        {
            if (!MaaSConfig.Current.Seller.Moderation.Enabled) return;

            var messageModel = new ProfileActivatedMessageModel()
            {
                UserName = domainEvent.Seller.PublicProfileName ?? domainEvent.Seller.Name
            };

            MessagesContext.Current.GenerateAndSend<ProfileActivatedMessageSpecification, ProfileActivatedMessageModel>(
                messageModel,
                MaaSConfig.Current.Environment.Smtp.DefaultFromAddress,
                new[] { domainEvent.Seller.Email }, domainEvent.Seller.Lang);
        }
    }
}