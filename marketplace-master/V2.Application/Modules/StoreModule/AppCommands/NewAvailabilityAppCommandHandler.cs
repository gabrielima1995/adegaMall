using Domain.Services;
using Domain.Services.StoreModule;
using Domain.StoreModule.Commands;
using Domain.StoreModule.Entities;
using Domain.StoreModule.Interfaces;
using Framework;
using System;
using System.Collections.Generic;

namespace V2.Application.Modules.StoreModule.AppCommands
{
    public class NewAvailabilityAppCommandHandler
    {
        private readonly ScheduleService _scheduleService;
        private readonly SellerService _sellerService;
        private readonly ProductService _productService;
        private readonly IRentalLockRepository _rentalLockRepository;

        private readonly TimeSpan _minTime = new TimeSpan(0, 0, 0);
        private readonly TimeSpan _maxTime = new TimeSpan(23, 59, 0);
        private readonly TimeSpan _oneMinute = new TimeSpan(0, 1, 0);

        public NewAvailabilityAppCommandHandler(
            ScheduleService scheduleService,
            SellerService sellerService,
            ProductService productService,
            IRentalLockRepository rentalLockRepository)
        {
            _scheduleService = scheduleService;
            _sellerService = sellerService;
            _productService = productService;
            _rentalLockRepository = rentalLockRepository;
        }

        public OperationResult Handle(NewAvailabilityAppCommand command)
        {
            var seller = _sellerService.GetById(command.SellerId);
            var product = command.ProductId.HasValue ? _productService.GetById(command.ProductId.Value) : null;
            var commands = new List<NewLockByDayOfWeekCommand>();

            void AddNewCommand(DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime)
            {
                commands.Add(new NewLockByDayOfWeekCommand(Guid.NewGuid(), seller,
                    dayOfWeek, startTime, endTime,
                    product, command.TimeZone.TimeZoneId, LockMode.FullyUnavailable));
            }

            #region Cria os bloqueios

            foreach (var item in command.DaysOfWeek)
            {
                // bloqueio do dia todo
                if (!item.Time1Setted)
                {
                    AddNewCommand(item.DayOfWeek, _minTime, _maxTime);
                    continue;
                }

                var startTime1 = item.StartTime1.Value;
                var endTime1 = item.EndTime1.Value;

                if (startTime1 >= endTime1) return OperationResult.Fail("EndTime1 must be greater than StartTime1");

                // bloqueio dos horários anteriores a hora inicial 1
                if (startTime1 > _minTime)
                {
                    var startLock1 = _minTime;
                    var endLock1 = startTime1.Subtract(_oneMinute);

                    AddNewCommand(item.DayOfWeek, startLock1, endLock1);
                }

                // bloqueio dos horários posteriores a hora final 1
                if (endTime1 < _maxTime)
                {
                    var startLock2 = endTime1;
                    var endLock2 = _maxTime;

                    // horário de disponibilidade 2 informados
                    if (item.Time2Setted)
                    {
                        // horário inicial 2 é o horário final do bloqueio 2
                        endLock2 = item.StartTime2.Value.Subtract(_oneMinute);
                    }

                    AddNewCommand(item.DayOfWeek, startLock2, endLock2);
                }

                // horário de disponibilidade 2 informados
                if (item.Time2Setted)
                {
                    var startTime2 = item.StartTime2.Value;
                    var endTime2 = item.EndTime2.Value;

                    if (endTime2 <= startTime2) return OperationResult.Fail("EndTime2 must be greater than StartTime2");
                    if (startTime2 <= endTime1) return OperationResult.Fail("StartTime2 must be greater than EndTime1");

                    // bloqueio dos horários posteriores a hora final 2
                    if (endTime2 < _maxTime)
                    {
                        var startLock2 = endTime2;
                        var endLock2 = _maxTime;

                        AddNewCommand(item.DayOfWeek, startLock2, endLock2);
                    }
                }
            }

            #endregion Cria os bloqueios

            #region Remove os bloqueios anteriores

            var oldLocks = _rentalLockRepository.GetDayOfWeeksLocks(command.SellerId, command.ProductId);

            foreach (var oldLock in oldLocks)
            {
                _scheduleService.DeleteLock(oldLock, false);
            }

            #endregion Remove os bloqueios anteriores

            #region Salva os bloqueios

            foreach (var item in commands)
            {
                var operation = _scheduleService.NewLock(item, false);
                if (operation.IsFailure) return operation;
            }

            #endregion Salva os bloqueios

            _scheduleService.Commit();

            return OperationResult.Success;
        }
    }
}