using Application.Config;
using Domain.Entities.StoreModule;
using Domain.Interfaces.Repositories;
using System;
using System.Globalization;
using System.Linq;
using V2.Application.Messages;
using V2.Application.Modules.StoreRequest.Messages;
using V2.Application.Modules.StoreRequest.Messages.Specs;

namespace V2.Application.Modules.StoreModule
{
    public class ReminderNotificationRentalRecurrentJob
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReminderNotificationRentalRecurrentJob(
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Run()
        {
            var rentalConfig = MaaSConfig.Current.Product.BusinessModel.Rental;

            if (!rentalConfig.Enabled) return;
            if (!rentalConfig.ReminderNotification.Enabled) return;

            var periodInHours = rentalConfig.ConvertRentalPeriodInHours(
                rentalConfig.ReminderNotification.Unit,
                rentalConfig.ReminderNotification.UnitValue);

            if (periodInHours < 1 / 60D) return;

            periodInHours++;

            #region Lembrete de início de locação

            // locações prestes a iniciar
            var orders = _unitOfWork.OrderRepository.ListForRentalStartReminderNotification(periodInHours);

            foreach (var order in orders)
            {
                if (DateTime.UtcNow > order.Products.First().GetRentalInfo().Result.StartDate)
                    return;

                SendEmailToBuyerOnRentalStart(order);

                SendEmailToSellerOnRentalStart(order);

                order.RentalInfo.StartReminderSent = true;

                _unitOfWork.OrderRepository.Update(order, true);
            }

            #endregion Lembrete de início de locação

            #region Lembrete de fim de locação

            // locações prestes a finalizar
            orders = _unitOfWork.OrderRepository.ListForRentalEndReminderNotification(periodInHours);

            foreach (var order in orders)
            {
                SendEmailToBuyerOnRentalEnd(order);

                SendEmailToSellerOnRentalEnd(order);

                order.RentalInfo.EndReminderSent = true;

                _unitOfWork.OrderRepository.Update(order, true);
            }

            #endregion Lembrete de fim de locação
        }


        public void SendEmailToBuyerOnRentalStart(Order order)
        {
            var lang = order.Buyer.Lang;
            var config = MaaSConfig.Current;

            var rentalInfo = order.Products.First().GetRentalInfo().Result;

            var rentalStart = rentalInfo.StartDate;
            var rentalEnd = rentalInfo.EndDate;
            var timeZone = order.Buyer.PreferredTimeZone;
            var culture = order.Buyer.PreferredCulture;

            var result = MessagesContext.Current
                .GenerateAndSend<RentalStartReminderMessageToBuyerSpecification, RentalStartReminderMessageModel>(
                    new RentalStartReminderMessageModel
                    {
                        BuyerName = order.Buyer.Name,
                        OrderId = order.Id.DisplayShortGuid(),
                        OrderStatus = order.Status.GetDescriptionLocalized(lang),
                        ShippingMethod = order.ShippingMethod.GetDescriptionLocalized(lang),
                        Products = order.Products.Select(i => new OrderProductMessageModel
                        {
                            Item = i.FullItemName,
                            Quantity = i.Quantity.ToStringLocalized(lang, "N0"),
                            Amount = config.DefaultCurrency + " " + i.Amount.ToStringLocalized(lang)
                        }),
                        Link = MaaSConfig.Current.Environment.AppUrl + "myorders/details/" + order.Id + "/?lang=" + lang,
                        SellerName = order.Seller.AlternativeName ?? order.Seller.Name,
                        SellerEmail = order.Seller.Email,
                        SellerPhone = order.Seller.Phone,
                        RentalStartDate = FormatHelper.DisplayDateTime(rentalStart, timeZone, culture),
                        RentalEndDate = FormatHelper.DisplayDateTime(rentalEnd, timeZone, culture)
                    },
                    MaaSConfig.Current.Environment.Smtp.DefaultFromAddress,
                    new[] { order.Buyer.Email },
                    lang);

            if (result.IsFailure)
                throw new Exception($"Failed on {nameof(SendEmailToBuyerOnRentalStart)}. Details: {result.Errors.ToSimpleString()}");
        }

        public void SendEmailToBuyerOnRentalEnd(Order order)
        {
            var lang = order.Buyer.Lang;
            var config = MaaSConfig.Current;

            var rentalInfo = order.Products.First().GetRentalInfo().Result;

            var rentalStart = rentalInfo.StartDate;
            var rentalEnd = rentalInfo.EndDate;
            var timeZone = order.Buyer.PreferredTimeZone;
            var culture = order.Buyer.PreferredCulture;

            var result = MessagesContext.Current
                .GenerateAndSend<RentalEndReminderMessageToBuyerSpecification, RentalEndReminderMessageModel>(
                    new RentalEndReminderMessageModel
                    {
                        BuyerName = order.Buyer.Name,
                        OrderId = order.Id.DisplayShortGuid(),
                        ShippingMethod = order.ShippingMethod.GetDescriptionLocalized(lang),
                        Products = order.Products.Select(i => new OrderProductMessageModel
                        {
                            Item = i.FullItemName,
                            Quantity = i.Quantity.ToStringLocalized(lang, "N0"),
                            Amount = config.DefaultCurrency + " " + i.Amount.ToStringLocalized(lang)
                        }),
                        Link = MaaSConfig.Current.Environment.AppUrl + "myorders/details/" + order.Id + "/?lang=" + lang,
                        SellerName = order.Seller.AlternativeName ?? order.Seller.Name,
                        SellerEmail = order.Seller.Email,
                        SellerPhone = order.Seller.Phone,
                        RentalStartDate = FormatHelper.DisplayDateTime(rentalStart, timeZone, culture),
                        RentalEndDate = FormatHelper.DisplayDateTime(rentalEnd, timeZone, culture)
                    },
                    MaaSConfig.Current.Environment.Smtp.DefaultFromAddress,
                    new[] { order.Buyer.Email },
                    lang);

            if (!result.OK)
                throw new Exception($"Failed on {nameof(SendEmailToBuyerOnRentalEnd)}. Details: {result.Errors.ToSimpleString()}");
        }


        private void SendEmailToSellerOnRentalStart(Order order)
        {
            var lang = order.Buyer.Lang;
            var config = MaaSConfig.Current;

            var rentalInfo = order.Products.First().GetRentalInfo().Result;

            var rentalStart = rentalInfo.StartDate;
            var rentalEnd = rentalInfo.EndDate;
            var timeZone = order.Buyer.PreferredTimeZone;
            var culture = order.Buyer.PreferredCulture;

            var result = MessagesContext.Current
                .GenerateAndSend<RentalStartReminderMessageToSellerSpecification, RentalStartReminderMessageModel>(
                    new RentalStartReminderMessageModel
                    {
                        BuyerName = order.Buyer.Name,
                        OrderId = order.Id.DisplayShortGuid(),
                        OrderStatus = order.Status.GetDescriptionLocalized(lang),
                        ShippingMethod = order.ShippingMethod.GetDescriptionLocalized(lang),
                        Products = order.Products.Select(i => new OrderProductMessageModel
                        {
                            Item = i.FullItemName,
                            Quantity = i.Quantity.ToStringLocalized(lang, "N0"),
                            Amount = config.DefaultCurrency + " " + i.Amount.ToStringLocalized(lang)
                        }),
                        Link = MaaSConfig.Current.Environment.AppUrl + "myorders/details/" + order.Id + "/?lang=" + lang,
                        SellerName = order.Seller.AlternativeName ?? order.Seller.Name,
                        SellerEmail = order.Seller.Email,
                        SellerPhone = order.Seller.Phone,
                        RentalStartDate = FormatHelper.DisplayDateTime(rentalStart, timeZone, culture),
                        RentalEndDate = FormatHelper.DisplayDateTime(rentalEnd, timeZone, culture)
                    },
                    MaaSConfig.Current.Environment.Smtp.DefaultFromAddress,
                    new[] { order.Buyer.Email },
                    lang);

            if (result.IsFailure)
                throw new Exception($"Failed on {nameof(SendEmailToSellerOnRentalStart)}. Details: {result.Errors.ToSimpleString()}");
        }

        private void SendEmailToSellerOnRentalEnd(Order order)
        {
            var lang = order.Buyer.Lang;
            var config = MaaSConfig.Current;

            var rentalInfo = order.Products.First().GetRentalInfo().Result;

            var rentalStart = rentalInfo.StartDate;
            var rentalEnd = rentalInfo.EndDate;
            var timeZone = order.Buyer.PreferredTimeZone;
            var culture = order.Buyer.PreferredCulture;

            var result = MessagesContext.Current
                .GenerateAndSend<RentalEndReminderMessageToSellerSpecification, RentalEndReminderMessageModel>(
                    new RentalEndReminderMessageModel
                    {
                        BuyerName = order.Buyer.Name,
                        OrderId = order.Id.DisplayShortGuid(),
                        ShippingMethod = order.ShippingMethod.GetDescriptionLocalized(lang),
                        Products = order.Products.Select(i => new OrderProductMessageModel
                        {
                            Item = i.FullItemName,
                            Quantity = i.Quantity.ToStringLocalized(lang, "N0"),
                            Amount = config.DefaultCurrency + " " + i.Amount.ToStringLocalized(lang)
                        }),
                        Link = MaaSConfig.Current.Environment.AppUrl + "myorders/details/" + order.Id + "/?lang=" + lang,
                        SellerName = order.Seller.AlternativeName ?? order.Seller.Name,
                        SellerEmail = order.Seller.Email,
                        SellerPhone = order.Seller.Phone,
                        RentalStartDate = FormatHelper.DisplayDateTime(rentalStart, timeZone, culture),
                        RentalEndDate = FormatHelper.DisplayDateTime(rentalEnd, timeZone, culture)
                    },
                    MaaSConfig.Current.Environment.Smtp.DefaultFromAddress,
                    new[] { order.Buyer.Email },
                    lang);

            if (!result.OK)
                throw new Exception($"Failed on {nameof(SendEmailToSellerOnRentalEnd)}. Details: {result.Errors.ToSimpleString()}");
        }
    }
}