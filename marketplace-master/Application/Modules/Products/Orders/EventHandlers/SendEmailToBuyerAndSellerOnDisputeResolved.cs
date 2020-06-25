using Application.Config;
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
    internal class SendEmailToBuyerAndSellerOnDisputeResolved : IDomainEventHandler<DisputeStatusChangedEvent>
    {
        private readonly IPersonRepository _personRepository;
        private readonly IPushNotificationService _notificationService;

        public SendEmailToBuyerAndSellerOnDisputeResolved(IPersonRepository personRepository, IPushNotificationService notificationService)
        {
            _personRepository = personRepository;
            _notificationService = notificationService;
        }

        public void Handle(DisputeStatusChangedEvent domainEvent)
        {
            //Dispute closed status are 4 ~ 7
            if ((int)domainEvent.NewStatus <= 3)
                return;

            var buyer = _personRepository.GetById(domainEvent.Dispute.BuyerId);
            var seller = _personRepository.GetById(domainEvent.Dispute.SellerId);

            //Push notification to buyer
            if (domainEvent.Dispute.Status == DisputeStatus.ClosedBySeller)
            {
                _notificationService.NotifyUser(buyer.UserId, new PushNotification()
                {
                    Title = GlobalizationHelper.GetString("NOTIFICATION_DISPUTE_RESOLVED_TITLE", MaaSConfig.Current.Localization.DefaultLang),
                    Body = string.Format(GlobalizationHelper.GetString("NOTIFICATION_DISPUTE_RESOLVED_DESC", MaaSConfig.Current.Localization.DefaultLang), "Vendedor"),
                    Link = $"/dispute/details/?orderId={domainEvent.Dispute.OrderId}"
                });
            }
            //Push notification to seller
            else if (domainEvent.Dispute.Status == DisputeStatus.ClosedByBuyer)
            {
                _notificationService.NotifyUser(seller.UserId, new PushNotification()
                {
                    Title = GlobalizationHelper.GetString("NOTIFICATION_DISPUTE_RESOLVED_TITLE", MaaSConfig.Current.Localization.DefaultLang),
                    Body = string.Format(GlobalizationHelper.GetString("NOTIFICATION_DISPUTE_RESOLVED_DESC", MaaSConfig.Current.Localization.DefaultLang), buyer.FullName.Split(' ').First(x => x.Length > 0)),
                    Link = $"/dispute/details/?orderId={domainEvent.Dispute.OrderId}"
                });
            }

            //Email to Buyer
            var result = MessagesContext.Current.GenerateAndSend<DisputeResolvedMessageSpecification, DisputeMessageModel>(
            new DisputeMessageModel()
            {
                RecipientName = buyer.Name,
                OrderId = new Guid(domainEvent.Dispute.OrderId),
                BuyerName = buyer.Name,
                SellerName = seller.AlternativeName ?? seller.Name,
            }, MaaSConfig.Current.Environment.Smtp.DefaultFromAddress,
            new[] { buyer.Email });

            //Email to Seller
            var result2 = MessagesContext.Current.GenerateAndSend<DisputeResolvedMessageSpecification, DisputeMessageModel>(
            new DisputeMessageModel()
            {
                RecipientName = seller.Name,
                OrderId = new Guid(domainEvent.Dispute.OrderId),
                BuyerName = buyer.Name,
                SellerName = seller.AlternativeName ??seller.Name,
            }, MaaSConfig.Current.Environment.Smtp.DefaultFromAddress,
            new[] { seller.Email });
        }
    }
}