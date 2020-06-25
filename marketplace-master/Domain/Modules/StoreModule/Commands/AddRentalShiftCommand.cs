using Domain.Core;
using Domain.Events.StoreModule;
using Domain.StoreModule.Entities;
using Domain.StoreModule.Interfaces;
using Framework;
using System;

namespace Domain.StoreModule.Commands
{
    public class AddRentalShiftCommand : Command
    {
        public AddRentalShiftCommand(Guid id, string name, int startHours, int startMinutes, int endHours, int endMinutes) : base(id.ToString())
        {
            Id = id;
            Name = name;
            StartHours = startHours;
            StartMinutes = startMinutes;
            EndHours = endHours;
            EndMinutes = endMinutes;
        }

        public Guid Id { get; }
        public string Name { get; }
        public int StartHours { get; }
        public int StartMinutes { get; }
        public int EndHours { get; }
        public int EndMinutes { get; }
    }

    public class AddRentalShiftCommandHandler
    {
        private readonly IRentalShiftRepository rentalShiftRepository;

        public AddRentalShiftCommandHandler(IRentalShiftRepository rentalShiftRepository)
        {
            this.rentalShiftRepository = rentalShiftRepository;
        }

        public OperationResult Handle(AddRentalShiftCommand command)
        {
            var errors = new ErrorDictionary();

            var startTimeResult = RentalShiftTime.Create(command.StartHours, command.StartMinutes);
            if (startTimeResult.IsFailure)
                errors.Merge(startTimeResult.Errors);

            var endTimeResult = RentalShiftTime.Create(command.EndHours, command.EndMinutes);
            if (endTimeResult.IsFailure)
                errors.Merge(endTimeResult.Errors);

            if (errors.HasErrors)
                return new OperationResult(errors);

            var rentalShiftResult = RentalShift.Create(command.Id, command.Name, startTimeResult.Result, endTimeResult.Result);

            if (rentalShiftResult.IsFailure)
                return rentalShiftResult;

            rentalShiftResult.Result.AddDomainEvent(new RentalShiftAddedEvent(rentalShiftResult.Result));

            return rentalShiftRepository.AddRentalShift(rentalShiftResult.Result);
        }
    }
}
