using Application.Config;
using Domain.Events;
using Domain.Interfaces.Repositories;
using Infra.Crosscutting.Globalization;
using Infrastructure.Data.Repositories;
using V2.Application.Messages;
using V2.Application.Modules.PushNotifications;
using V2.Application.Modules.UserAccounts.Messages.Model;
using V2.Application.Modules.UserAccounts.Messages.Specs;

namespace V2.Application.Modules.Buyers.EventHandlers
{
    public class SendEmailToBuyerWhenModerationApproved : IDomainEventHandler<BuyerModerationApprovedEvent>
    {
        private readonly IPushNotificationService notificationService;
        private readonly IPersonRepository personRepository;

        public SendEmailToBuyerWhenModerationApproved(
            IPushNotificationService notificationService,
            IPersonRepository personRepository)
        {
            this.notificationService = notificationService;
            this.personRepository = personRepository;
        }

        public void Handle(BuyerModerationApprovedEvent domainEvent)
        {
            var messageModel = new ProfileActivatedMessageModel()
            {
                UserName = domainEvent.Buyer.Name,
            };

            var buyer = personRepository.GetById(domainEvent.BuyerId);

            notificationService.NotifyUser(buyer.UserId, new PushNotification()
            {
                Title = GlobalizationHelper.GetString("NOTIFICATION_MODERATED_TITLE", MaaSConfig.Current.Localization.DefaultLang),
                Body = GlobalizationHelper.GetString("NOTIFICATION_MODERATED_DESC", MaaSConfig.Current.Localization.DefaultLang),
                Link = "manage/"
            });

            MessagesContext.Current.GenerateAndSend<BuyerProfileActivatedMessageSpecification, ProfileActivatedMessageModel>(
                messageModel,
                MaaSConfig.Current.Environment.Smtp.DefaultFromAddress,
                new[] { domainEvent.Buyer.Email }, domainEvent.Buyer.Lang);
        }
    }
}