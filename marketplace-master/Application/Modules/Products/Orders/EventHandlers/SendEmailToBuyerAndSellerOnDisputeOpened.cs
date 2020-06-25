using Application.Config;
using Domain.Entities;
using Domain.Entities.DisputeModule;
using Domain.Events;
using Domain.Interfaces.Repositories;
using Infra.Crosscutting.Globalization;
using System;
using System.Linq;
using V2.Application.Messages;
using V2.Application.Modules.Disputes.Messages;
using V2.Application.Modules.Disputes.Messages.Specs;
using V2.Application.Modules.PushNotifications;

namespace Application.Modules.Products.Orders.EventHandlers
{
    internal class SendEmailToBuyerAndSellerOnDisputeOpened : IDomainEventHandler<DisputeOpenedEvent>
    {
        private readonly IPersonRepository personRepository;
        private readonly IPushNotificationService notificationService;

        public SendEmailToBuyerAndSellerOnDisputeOpened(IPersonRepository personRepository, IPushNotificationService notificationService)
        {
            this.personRepository = personRepository;
            this.notificationService = notificationService;
        }

        public void Handle(DisputeOpenedEvent domainEvent)
        {
            var buyer = personRepository.GetById(domainEvent.Dispute.BuyerId);
            var seller = personRepository.GetById(domainEvent.Dispute.SellerId);

            NotifyBuyer(domainEvent, buyer, seller);
            NotifySeller(domainEvent, buyer, seller);
        }

        private void NotifyBuyer(DisputeOpenedEvent domainEvent, Person buyer, Person seller)
        {
            MessagesContext.Current.GenerateAndSend<DisputeOpenedMessageSpecification, DisputeMessageModel>(
                new DisputeMessageModel()
                {
                    RecipientName = buyer.Name,
                    OrderId = new Guid(domainEvent.Dispute.OrderId),
                    BuyerName = buyer.Name,
                    SellerName = seller.AlternativeName ?? seller.Name,
                },
                MaaSConfig.Current.Environment.Smtp.DefaultFromAddress,
                new[] { buyer.Email });

            if (domainEvent.Dispute.Status == DisputeStatus.OpenedBySeller)
            {
                notificationService.NotifyUser(buyer.UserId, new PushNotification()
                {
                    Title = GlobalizationHelper.GetString("NOTIFICATION_DISPUTE_OPENED_TITLE", Lang),
                    Body = GlobalizationHelper.FormatStringWithLanguage("NOTIFICATION_DISPUTE_OPENED_TITLE", Lang, "Vendedor"),
                    Link = $"/dispute/details/?orderId={domainEvent.Dispute.OrderId}"
                });
            }
        }

        private void NotifySeller(DisputeOpenedEvent domainEvent, Person buyer, Person seller)
        {
            MessagesContext.Current.GenerateAndSend<DisputeOpenedMessageSpecification, DisputeMessageModel>(
                new DisputeMessageModel()
                {
                    RecipientName = seller.Name,
                    OrderId = new Guid(domainEvent.Dispute.OrderId),
                    BuyerName = buyer.Name,
                    SellerName = seller.AlternativeName ?? seller.Name,
                },
                MaaSConfig.Current.Environment.Smtp.DefaultFromAddress,
                new[] { seller.Email });

            if (domainEvent.Dispute.Status == DisputeStatus.OpenedByBuyer)
            {
                notificationService.NotifyUser(seller.UserId, new PushNotification()
                {
                    Title = GlobalizationHelper.GetString("NOTIFICATION_DISPUTE_OPENED_TITLE", Lang),
                    Body = GlobalizationHelper.FormatStringWithLanguage("NOTIFICATION_DISPUTE_OPENED_TITLE", Lang, GetFirstName(buyer)),
                    Link = $"/dispute/details/?orderId={domainEvent.Dispute.OrderId}"
                });
            }
        }

        private string GetFirstName(Person person)
        {
            return person.FullName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).First();
        }

        private string Lang => MaaSConfig.Current.Localization.DefaultLang;
    }
}