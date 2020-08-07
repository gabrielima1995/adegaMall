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
    public class SendEmailToSellerAndAdminOnOrderPaymentPending : IDomainEventHandler<OrderStatusChangedEvent>
    {
        private readonly IPersonRepository personRepository;
        private readonly ITimeZoneProvider userTimeZoneProvider;

        public SendEmailToSellerAndAdminOnOrderPaymentPending(IPersonRepository personRepository, ITimeZoneProvider userTimeZoneProvider)
        {
            this.personRepository = personRepository;
            this.userTimeZoneProvider = userTimeZoneProvider;
        }

        public void Handle(OrderStatusChangedEvent domainEvent)
        {
            if (domainEvent.NewStatus != OrderStatus.PendingPayment)
                return;

            var sellerResult = SendToSeller(domainEvent);
            SendToAdminIfEnabled(domainEvent);

            if (!sellerResult.OK)
                throw new DomainEventHandlerFailedException(GetType(), domainEvent, "Failed on MessagesContext.Current.GenerateAndSend. Details: " + sellerResult.Errors.ToSimpleString());
        }

        private OperationResult SendToSeller(OrderStatusChangedEvent domainEvent)
        {
            var model = GetModel(domainEvent, false);
            return MessagesContext.Current
                .GenerateAndSend<OrderProcessingPaymentMessageToSellerSpecification, OrderProcessingPaymentMessageModel>(
                    model,
                    MaaSConfig.Current.Environment.Smtp.DefaultFromAddress,
                    new[] { domainEvent.Order.Seller.Email }, domainEvent.Order.Seller.Lang);
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
                .GenerateAndSend<OrderProcessingPaymentMessageToAdminSpecification, OrderProcessingPaymentMessageModel>(
                    model,
                    MaaSConfig.Current.Environment.Smtp.DefaultFromAddress,
                    admins.Select(admin => admin.Email), admins.First().Lang);
        }

        private OrderProcessingPaymentMessageModel GetModel(OrderStatusChangedEvent domainEvent, bool forAdmin)
        {
            var config = MaaSConfig.Current;
            var order = domainEvent.Order;
            var lang = order.Seller.Lang;

            var timeZone = userTimeZoneProvider.GetTimeZone();

            var rentalInfo = order.Products.First().GetRentalInfo().Result;
            DateTime? rentalStart = DateTime.Now;
            DateTime? rentalEnd = DateTime.Now;

            if (rentalInfo != null)
            {
                rentalStart = rentalInfo.StartDate;
                rentalEnd = rentalInfo.EndDate;
            }

            return new OrderProcessingPaymentMessageModel
            {
                OrderId = order.Id.Substring(),
                OrderStatus = domainEvent.NewStatus.GetDescriptionLocalized(lang),
                SellerName = order.Seller.AlternativeName ?? order.Seller.Name,
                BuyerName = order.Buyer.Name,
                BuyerPhone = order.Buyer.Phone,
                Amount = config.DefaultCurrency + " " + order.Amount.ToStringLocalized(lang),
                ExtraInfo = order.ExtraInfo,
                Products = order.Products.Select(i => new OrderProductMessageModel
                {
                    Item = i.FullItemName,
                    Quantity = i.Quantity.ToStringLocalized(lang, "N0"),
                    Amount = config.DefaultCurrency + " " + i.Amount.ToStringLocalized(lang)
                }),
                RentalStartDate = FormatHelper.DisplayDateTime(rentalStart, timeZone),
                RentalEndDate = FormatHelper.DisplayDateTime(rentalEnd, timeZone),
                IsOfflinePayment = order.PaymentMethod == PaymentIntegrator.Core.PaymentMethods.PaymentMethod.Offline,
                Link = forAdmin
                    ? MaaSConfig.Current.Environment.AppUrl + "admin/orders/details/" + order.Id + "/?lang=" + lang
                    : MaaSConfig.Current.Environment.AppUrl + "mysales/details/" + order.Id + "/?lang=" + lang
            };
        }
    }
}