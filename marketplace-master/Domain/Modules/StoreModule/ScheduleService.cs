using Domain.StoreModule.Commands;
using Domain.StoreModule.Entities;
using Domain.StoreModule.Interfaces;
using Framework;

namespace Domain.Services.StoreModule
{
    public class ScheduleService
    {
        private readonly IRentalLockRepository _rentalLockRepository;

        public ScheduleService(
            IRentalLockRepository rentalLockRepository)
        {
            _rentalLockRepository = rentalLockRepository;
        }

        public OperationResult NewLock(NewLockByDateCommand command)
        {
            var entity = RentalLock.New(command);

            return _rentalLockRepository.Add(entity, true);
        }

        public OperationResult NewLock(NewLockByDayOfWeekCommand command, bool saveChanges = true)
        {
            var entity = RentalLock.New(command);

            return _rentalLockRepository.Add(entity, saveChanges);
        }

        public OperationResult DeleteLock(DeleteLockCommand command, bool saveChanges = true)
        {
            var @lock = _rentalLockRepository.GetById(command.LockId);

            if (@lock == null) return OperationResult.Fail("Lock not found");

            return DeleteLock(@lock, saveChanges);
        }

        public OperationResult DeleteLock(RentalLock @lock, bool saveChanges = true)
        {
            @lock.Delete();

            return _rentalLockRepository.Update(@lock, saveChanges);
        }

        public OperationResult Commit()
        {
            return _rentalLockRepository.SaveChanges();
        }
    }
}