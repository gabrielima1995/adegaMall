using Domain.Services;
using Domain.Services.StoreModule;
using Domain.StoreModule.Commands;
using Domain.StoreModule.Interfaces;
using Framework;
using System.Collections.Generic;
using System.Linq;

namespace V2.Application.Modules.StoreModule.AppCommands
{
    public class DeleteLockAppCommandHandler
    {
        private readonly ScheduleService _scheduleService;
        private readonly SellerService _sellerService;
        private readonly IRentalLockRepository _rentalLockRepository;

        public DeleteLockAppCommandHandler(
            ScheduleService scheduleService,
            SellerService sellerService,
            IRentalLockRepository rentalLockRepository)
        {
            _scheduleService = scheduleService;
            _sellerService = sellerService;
            _rentalLockRepository = rentalLockRepository;
        }

        public OperationResult Handle(DeleteLockAppCommand command)
        {
            var @lock = _rentalLockRepository.GetById(command.LockId);

            var seller = _sellerService.GetById(command.SellerId);

            return _scheduleService.DeleteLock(new DeleteLockCommand(@lock, seller));
        }

        public OperationResult Handle(DeleteLockForProductAppComand command)
        {
            var @locks = _rentalLockRepository.GetDayOfWeeksLocks(command.SellerId, command.ProductId);

            var seller = _sellerService.GetById(command.SellerId);

            OperationResult result = new OperationResult();
            List<ErrorDictionary> errors = new List<ErrorDictionary>();

            foreach (var item in locks)
            {
                result = _scheduleService.DeleteLock(new DeleteLockCommand(item, seller));
                if (result.IsFailure)
                    errors.Add(result.Errors);
            }

            if (errors.Any())
                return errors.First();

            return result;
        }
    }
}