using Domain.Modules.StoreModule;
using Domain.Modules.StoreModule.Commands;
using Domain.Modules.StoreModule.Entities;
using Domain.Services;
using Domain.Services.StoreModule;
using Framework;
using Infra.Crosscutting.Globalization;
using System;

namespace V2.Application.Modules.StoreModule.AppCommands
{
    public class NewProductAvailabilityAppCommandHandler
    {
        private readonly ProductAvailabilityService _productAvailabilityService;
        private readonly SellerService _sellerService;
        private readonly ProductService _productService;

        public NewProductAvailabilityAppCommandHandler(
            ProductAvailabilityService productAvailabilityService,
            SellerService sellerService,
            ProductService productService)
        {
            _productAvailabilityService = productAvailabilityService;
            _sellerService = sellerService;
            _productService = productService;
        }

        public OperationResult Handle(NewProductAvailabilityAppCommand appCommand)
        {
            var seller = _sellerService.GetById(appCommand.SellerId);
            var product = _productService.GetById(appCommand.ProductId);

            #region Verifica hora final

            if (product.RentalSettings.IsFixedRental)
            {
                var endTime = product.RentalSettings.CalculateEndTime(appCommand.StartTime);

                if (endTime.HasValue) appCommand.EndTime = endTime.Value;
            }

            #endregion Verifica hora final

            var command = new NewProductAvailabilityCommand(
                Guid.NewGuid(), product, seller,
                appCommand.DayOfWeek, appCommand.StartTime, appCommand.EndTime,
                appCommand.CurrentStock, appCommand.DiscountPercentage);

            #region Verifica disponibilidades conflitantes

            var conflictOperation = _productAvailabilityService.CheckConflict(command);
            if (conflictOperation.IsFailure) return conflictOperation;

            #endregion Verifica disponibilidades conflitantes

            var operation = _productAvailabilityService.NewAvailability(command, false);
            if (operation.IsFailure) return operation;

            _productAvailabilityService.Commit();

            return OperationResult.Success;
        }

        public OperationResult Handle(NewRecurringProductAvailabilityAppCommand appCommand)
        {
            var seller = _sellerService.GetById(appCommand.SellerId);
            var product = _productService.GetById(appCommand.ProductId);

            #region Verifica hora final

            if (product.RentalSettings.IsFixedRental)
            {
                var endTime = product.RentalSettings.CalculateEndTime(appCommand.StartTime);

                if (endTime.HasValue) appCommand.EndTime = endTime.Value;
            }

            #endregion Verifica hora final

            if (appCommand.Unit == RecurringProductAvailabilityUnit.Weeks &&
                (appCommand.DaysOfWeek == null || appCommand.DaysOfWeek.Length == 0))
                return OperationResult.Fail(GlobalizationHelper.GetString("RECURRING_PRODUCT_AVAILABILITY_DAYSOFWEEK_ERROR"));

            var command = new NewRecurringProductAvailabilityCommand(
                Guid.NewGuid(), product, seller,
                appCommand.DaysOfWeek, appCommand.Unit, appCommand.UnitValue,
                appCommand.StartTime, appCommand.EndTime,
                appCommand.CurrentStock, appCommand.DiscountPercentage);

            var operation = _productAvailabilityService.NewAvailability(command, false);
            if (operation.IsFailure) return operation;

            _productAvailabilityService.Commit();

            return OperationResult.Success;
        }
    }
}