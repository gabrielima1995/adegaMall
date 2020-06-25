using Domain.Services;
using Domain.Services.StoreModule;
using Domain.StoreModule.Commands;
using Domain.StoreModule.Entities;
using Framework;
using Infra.Crosscutting.Globalization;
using System;

namespace V2.Application.Modules.StoreModule.AppCommands
{
    public class NewLockAppCommandHandler
    {
        private readonly ScheduleService _scheduleService;
        private readonly SellerService _sellerService;
        private readonly ProductService _productService;

        public NewLockAppCommandHandler(
            ScheduleService scheduleService,
            SellerService sellerService,
            ProductService productService)
        {
            _scheduleService = scheduleService;
            _sellerService = sellerService;
            _productService = productService;
        }

        public OperationResult Handle(NewLockAppCommand command)
        {
            var seller = _sellerService.GetById(command.SellerId);

            var product = command.ProductId.HasValue ? _productService.GetById(command.ProductId.Value) : null;

            switch (command.Type)
            {
                case LockType.ByDate:
                    if (command.StartDate.Value > command.EndDate.Value)
                    {
                        return OperationResult.Fail(GlobalizationHelper.GetString("NEW_LOCK_DATES_INVALID_MESSAGE"));
                    }

                    var byDateCommand = new NewLockByDateCommand(Guid.NewGuid(), seller,
                        command.StartDate.Value, command.EndDate.Value, command.StartTime.Value, command.EndTime.Value,
                        product, command.TimeZone.TimeZoneId, command.Mode);

                    return _scheduleService.NewLock(byDateCommand);

                case LockType.ByDayOfWeek:
                    var byDateOfWeekCommand = new NewLockByDayOfWeekCommand(Guid.NewGuid(), seller,
                        command.DayOfWeek.Value, command.StartTime.Value, command.EndTime.Value,
                        product, command.TimeZone.TimeZoneId, command.Mode);

                    return _scheduleService.NewLock(byDateOfWeekCommand);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}