using Application.Config;
using Domain.Core;
using Domain.Entities.StoreModule;
using Domain.Events;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Framework;
using System;
using System.Globalization;
using System.Linq;
using V2.Application.Messages;
using V2.Application.Modules.StoreRequest.Messages;
using V2.Application.Modules.StoreRequest.Messages.Specs;

namespace Application.Modules.Products.Orders.EventHandlers
{
    public class SendEmailToAdminOnOrderPaymentCompleted : IDomainEventHandler<OrderStatusChangedEvent>
    {
        private readonly IPersonRepository personRepository;
        private readonly ITimeZoneProvider userTimeZoneProvider;

        public SendEmailToAdminOnOrderPaymentCompleted(IPersonRepository personRepository, ITimeZoneProvider userTimeZoneProvider)
        {
            this.personRepository = personRepository;
            this.userTimeZoneProvider = userTimeZoneProvider;
        }

        public void Handle(OrderStatusChangedEvent domainEvent)
        {
            if (domainEvent.NewStatus != OrderStatus.CompletedPayment)
                return;

            var adminResult = SendToAdminIfEnabled(domainEvent);

            if (!adminResult.OK)
                throw new DomainEventHandlerFailedException(GetType(), domainEvent, "Failed on MessagesContext.Current.GenerateAndSend. Details: " + adminResult.Errors.ToSimpleString());
        }

        private OperationResult SendToAdminIfEnabled(OrderStatusChangedEvent domainEvent)
        {
            if (!MaaSConfig.Current.Product.Order.SendOrderCopyToAdmins)
                return OperationResult.Success;

            var admins = personRepository.ListAdminPersons();

            if (!admins.Any())
                return OperationResult.Success;

            var model = GetModel(domainEvent, true);
            return MessagesContext.Current
                .GenerateAndSend<OrderCompletedPaymentMessageToAdminSpecification, OrderCompletedPaymentMessageModel>(
                    model,
                    MaaSConfig.Current.Environment.Smtp.DefaultFromAddress,
                    admins.Select(admin => admin.Email), admins.First().Lang);
        }

        private OrderCompletedPaymentMessageModel GetModel(OrderStatusChangedEvent domainEvent, bool forAdmin)
        {
            var config = MaaSConfig.Current;
            var order = domainEvent.Order;
            var lang = order.Seller.Lang;

            var timeZone = userTimeZoneProvider.GetTimeZone();

            var rentalInfo = order.Products.First().GetRentalInfo().Result;
            DateTime? rentalStart = DateTime.Now;
            DateTime? rentalEnd =  DateTime.Now;

            if (rentalInfo != null)
            {
                 rentalStart = rentalInfo.StartDate;
                 rentalEnd = rentalInfo.EndDate;
            }

            return new OrderCompletedPaymentMessageModel
            {
                OrderId = order.Id.Substring(),
                SellerName = order.Seller.AlternativeName ?? order.Seller.Name,
                BuyerName = order.Buyer.Name,
                Amount = config.DefaultCurrency + " " + order.Amount.ToStringLocalized(lang),
                IsOfflinePayment = order.PaymentMethod == PaymentIntegrator.Core.PaymentMethods.PaymentMethod.Offline,
                ExtraInfo = order.ExtraInfo,
                Products = order.Products.Select(i => new OrderProductMessageModel
                {
                    Item = i.FullItemName,
                    Quantity = i.Quantity.ToStringLocalized(lang, "N0"),
                    Amount = config.DefaultCurrency + " " + i.Amount.ToStringLocalized(lang)
                }),
                RentalStartDate = FormatHelper.DisplayDateTime(rentalStart, timeZone),
                RentalEndDate = FormatHelper.DisplayDateTime(rentalEnd, timeZone),
                Link = forAdmin
                    ? MaaSConfig.Current.Environment.AppUrl + "admin/orders/details/" + order.Id + "/?lang=" + lang
                    : MaaSConfig.Current.Environment.AppUrl + "mysales/details/" + order.Id + "/?lang=" + lang
            };
        }
    }
}
