using Application.Config;
using Domain.Entities;
using Domain.Events;
using Domain.Events.Subscriptions;
using Domain.Interfaces.Repositories;
using V2.Application.Messages;
using V2.Application.Modules.Subscriptions.Messages;
using V2.Application.Modules.Subscriptions.Messages.Specs;

namespace V2.Application.Modules.Subscriptions.EventHandlers
{
    public class SendEmailToSellerOnSubscriptionAdded : IDomainEventHandler<SubscriptionAddedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;

        public SendEmailToSellerOnSubscriptionAdded(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Handle(SubscriptionAddedEvent domainEvent)
        {
            var seller = _unitOfWork.SellerRepository.GetById(domainEvent.PersonId);

            var messageModel = new NewSubscriptionMessageModel
            {
                SellerName = seller.FullName,
                ProfileUrl = GetProfileUrl(seller)
            };

            MessagesContext.Current
                .GenerateAndSend<NewSubscriptionMessageToSellerSpecification, NewSubscriptionMessageModel>(
                    messageModel,
                    MaaSConfig.Current.Environment.Smtp.DefaultFromAddress,
                    new[] { seller.Email },
                    seller.Lang);
        }

        private string GetProfileUrl(Seller seller)
        {
            return $"{MaaSConfig.Current.Environment.AppUrl}p/{seller.FriendlyUrl ?? seller.Nickname}/";
        }
    }
}
