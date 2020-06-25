using Domain.Core;
using Domain.Events.StoreModule;
using Domain.StoreModule.Interfaces;
using Framework;
using System;

namespace Domain.StoreModule.Commands
{
    public class RemoveRentalShiftCommand : Command
    {
        public RemoveRentalShiftCommand(Guid id) : base(id.ToString())
        {
            Id = id;
        }

        public Guid Id { get; }
    }

    public class RemoveRentalShiftCommandHandler
    {
        private readonly IRentalShiftRepository rentalShiftRepository;

        public RemoveRentalShiftCommandHandler(IRentalShiftRepository rentalShiftRepository)
        {
            this.rentalShiftRepository = rentalShiftRepository;
        }

        public OperationResult Handle(RemoveRentalShiftCommand command)
        {
            var rentalShift = rentalShiftRepository.Get(command.Id);

            if (rentalShift == null)
                return OperationResult.Fail("", $"Rental shift id '{command.Id}' was not found.");

            rentalShift.AddDomainEvent(new RentalShiftRemovedEvent(rentalShift));

            return rentalShiftRepository.RemoveRentalShift(rentalShift);
        }
    }
}
