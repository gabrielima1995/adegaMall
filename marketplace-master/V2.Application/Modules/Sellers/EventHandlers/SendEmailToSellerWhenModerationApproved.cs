using Application.Config;
using Domain.Events;
using Domain.Interfaces.Repositories;
using Infra.Crosscutting.Globalization;
using V2.Application.Messages;
using V2.Application.Modules.PushNotifications;
using V2.Application.Modules.UserAccounts.Messages.Model;
using V2.Application.Modules.UserAccounts.Messages.Specs;

namespace V2.Application.Modules.Sellers.EventHandlers
{
    public class SendEmailToSellerWhenModerationApproved : IDomainEventHandler<SellerModerationApprovedEvent>
    {
        private readonly IPushNotificationService notificationService;
        private readonly IPersonRepository personRepository;

        public SendEmailToSellerWhenModerationApproved(
            IPushNotificationService notificationService,
            IPersonRepository personRepository)
        {
            this.notificationService = notificationService;
            this.personRepository = personRepository;
        }

        public void Handle(SellerModerationApprovedEvent domainEvent)
        {
            var messageModel = new ProfileActivatedMessageModel()
            {
                UserName = domainEvent.Seller.PublicProfileName ?? domainEvent.Seller.Name
            };

            var seller = personRepository.GetById(domainEvent.SellerId);

            notificationService.NotifyUser(seller.UserId, new PushNotification()
            {
                Title = GlobalizationHelper.GetString("NOTIFICATION_MODERATED_TITLE", MaaSConfig.Current.Localization.DefaultLang),
                Body = GlobalizationHelper.GetString("NOTIFICATION_MODERATED_DESC", MaaSConfig.Current.Localization.DefaultLang),
                Link = "manage/"
            });

            MessagesContext.Current.GenerateAndSend<ProfileActivatedMessageSpecification, ProfileActivatedMessageModel>(
                messageModel,
                MaaSConfig.Current.Environment.Smtp.DefaultFromAddress,
                new[] { domainEvent.Seller.Email }, domainEvent.Seller.Lang);
        }
    }
}